using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using PS3DumpChecker.Properties;

namespace PS3DumpChecker
{
    internal sealed partial class Main : Form
    {
        private static string _version;
        private readonly string _wrkdir = Path.GetDirectoryName(Application.ExecutablePath);

        public Main(ICollection<string> args)
        {
            InitializeComponent();
            Common.StatusUpdate += StatusUpdate;
            Common.ListUpdate += CommonOnListUpdate;
            Assembly app = Assembly.GetExecutingAssembly();
            _version = string.Format("PS3 Dump Checker v{0}.{1} (Build: {2})", app.GetName().Version.Major,
                                     app.GetName().Version.Minor, app.GetName().Version.Build);
            Text = _version;
            Icon = Resources.logo;
            if (_wrkdir != null)
                Directory.SetCurrentDirectory(_wrkdir);
            string dir = Path.GetDirectoryName(Application.ExecutablePath) + "\\default.cfg";
            var fi = new FileInfo(dir);
            if (fi.Exists && fi.Length > 0)
                ParseConfig(dir);
            else
            {
                ExtractResource(fi, "PS3DumpChecker.default.cfg");
                fi = new FileInfo(dir);
                if (fi.Exists && fi.Length > 0)
                    ParseConfig(dir);
            }
            if (args.Count < 1)
                return;
            foreach (string s in args)
            {
                if (!File.Exists(s))
                    continue;
                if (worker.IsBusy)
                    return;
                StartCheck(s);
            }
        }

        private static bool GetRegSetting(string setting, bool value = false)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software");
            if (key == null)
                return value;
            key = key.CreateSubKey("Swizzy");
            if (key == null)
                return value;
            key = key.CreateSubKey("PS3 Dump Checker");
            if (key == null)
                return value;
            return key.GetValue(setting, -1) is int ? (int) key.GetValue(setting, value ? 1 : 0) > 0 : value;
        }

        private void CommonOnListUpdate(object sender, EventArgs eventArgs)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<EventArgs>(CommonOnListUpdate), new[]
                                                                                 {
                                                                                     sender, eventArgs
                                                                                 });
                return;
            }
            partslist.Items.Clear();
            if (Common.PartList.Keys.Count <= 0)
                return;
            foreach (var key in Common.PartList.Keys)
                partslist.Items.Add(new ListBoxItem(key, Common.PartList[key].Name));
        }

        private void StatusUpdate(object sender, StatusEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<StatusEventArgs>(StatusUpdate), new[]
                                                                                 {
                                                                                     sender, e
                                                                                 });
                return;
            }
            status.Text = e.Status;
        }

        private void SetTitle(string title)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetTitle), new object[]
                                                         {
                                                             title
                                                         });
                return;
            }
            Text = title;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            string file = e.Argument.ToString();
            if (string.IsNullOrEmpty(file))
                return;
            var sw = new Stopwatch();
            var fi = new FileInfo(file);
            if (Common.Types.ContainsKey(fi.Length))
            {
                SetTitle(string.Format("{0} Type: {2} File: {1}", _version, Path.GetFileName(file),
                                       Common.Types[fi.Length].Name.Value));
                Common.SendStatus("Reading image into memory and checking statistics...");
                Logger.LogPath = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) +
                                 "_PS3Check.log";
                Logger.WriteLine2("********************************************");
                Logger.WriteLine2(_version);
                Logger.WriteLine2(string.Format("Check started: {0}", DateTime.Now));
                Logger.WriteLine2("********************************************");
                sw.Start();
                Logger.WriteLine("Reading image into memory and checking statistics...");
                e.Result = Checks.StartCheck(file, ref sw);
            }
            else
                Common.SendStatus("ERROR: Bad file size! Check aborted...");
        }

        private void CheckbtnClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
                          {
                              Title = Resources.seldump,
                              FileName = "dump.bin",
                              Filter = Resources.ofdfilter,
                              DefaultExt = "bin",
                              AutoUpgradeEnabled = true,
                              AddExtension = true
                          };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            StartCheck(ofd.FileName);
        }

        private void StartCheck(string file)
        {
            Common.PartList.Clear();
            partslist.Items.Clear();
            imgstatus.Text = Resources.N_A;
            reversed.Text = Resources.N_A;
            idmatchbox.Text = Resources.N_A;
            statuslabel.Visible = false;
            actdatabox.Text = "";
            expdatabox.Text = "";
            checkbtn.Enabled = false;
            Logger.Enabled = logstate.Checked;
            worker.RunWorkerAsync(file);
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null && Common.Types.Count > 0)
                MessageBox.Show(Resources.badsize, Resources.error_bad_size, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (Common.Types.Count <= 0)
                MessageBox.Show(Resources.error_noconfig, Resources.error_noconfig_title, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            else
            {
                try
                {
                    if (e.Result != null)
                    {
                        var res = (Common.ImgInfo) e.Result;
                        imgstatus.Text = res.Status;
                        reversed.Text = res.Reversed ? "Yes" : "No";
                        idmatchbox.Text = res.SKUModel ?? "No matching SKU model found!";
                        minverbox.Text = res.MinVer ?? "N/A";
                        statuslabel.Text = res.IsOk ? "OK" : "BAD";
                        statuslabel.ForeColor = res.IsOk ? Color.Green : Color.Red;
                        statuslabel.Visible = true;
                        if (res.IsOk && File.Exists("patcher.exe") &&
                            (GetRegSetting("autopatch") ||
                             MessageBox.Show(Resources.autopatchmsg, Resources.autopatch, MessageBoxButtons.YesNoCancel) ==
                             DialogResult.Yes))
                        {
                            var proc = new Process
                                           {
                                               StartInfo =
                                                   {
                                                       Arguments = string.Format("\"{0}\"", res.FileName),
                                                       FileName = "patcher.exe",
                                                       WorkingDirectory = _wrkdir
                                                   }
                                           };
                            proc.Start();
                            if (GetRegSetting("autoexit"))
                                Close();
                        }
                    }
                }
                catch (Exception) { }
            }
            if (Common.Types.Count > 0)
                checkbtn.Enabled = true;
        }

        private void PartslistSelectedIndexChanged(object sender, EventArgs e)
        {
            if (partslist.SelectedItems.Count == 0)
                return;
            if (!(partslist.SelectedItems[0] is ListBoxItem))
                return;
            var tmp = partslist.SelectedItems[0] as ListBoxItem;
            if (!Common.PartList.ContainsKey(tmp.Value))
                return;
            Common.PartsObject obj = Common.PartList[tmp.Value];
            expdatabox.Text = string.Format("Result of the check: {1}{0}{0}", Environment.NewLine, obj.Result);
            expdatabox.Text += obj.ExpectedString;
            actdatabox.Text = obj.ActualString;
        }

        private void LoadConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
                          {
                              Title = Resources.select_conf,
                              FileName = "Default.cfg",
                              DefaultExt = "cfg",
                              Filter = Resources.conf_filter,
                              AutoUpgradeEnabled = true
                          };
            if (ofd.ShowDialog() == DialogResult.OK)
                ParseConfig(ofd.FileName);
        }

        private void ParseConfig(string file)
        {
            Common.SendStatus(string.Format("Parsing {0}", file));
            Common.Types.Clear();
            using (XmlReader xml = XmlReader.Create(file))
            {
                int dataCheckKey = 0;
                bool dataCheckOk = false;
                int skUkey = 0;
                long size = 0;
                bool skuWarn = false;
                string skuName = "";
                string skuWarnMsg = "";
                string skuMinVer = "";
                while (xml.Read())
                {
                    if (!xml.IsStartElement())
                        continue;
                    switch (xml.Name.ToLower())
                    {
                        case "type":
                            if (long.TryParse(xml["size"], out size))
                            {
                                if (!Common.Types.ContainsKey(size))
                                    Common.Types.Add(size, new Common.TypeData(true));
                                Common.Types[size].Name.Value = xml["name"];
                            }
                            break;

                            #region Statistics part

                        case "stats":
                            if (Common.Types.ContainsKey(size))
                                xml.Read();
                            Common.Types[size].StatDescription.Value = xml.Value;
                            break;
                        case "statspart":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            string key = xml["key"];
                            if (key == null)
                                break;
                            if (string.IsNullOrEmpty(key))
                                key = "*";
                            key = key.ToUpper();
                            if (!Common.Types[size].Statlist.Value.ContainsKey(key))
                            {
                                double low;
                                double high;
                                string lowtxt = xml["low"];
                                if (lowtxt != null)
                                {
                                    lowtxt = Regex.Replace(lowtxt, @"\s+", "");
                                    lowtxt = lowtxt.Replace('.', ',');
                                }
                                string hightxt = xml["high"];
                                if (hightxt != null)
                                {
                                    hightxt = Regex.Replace(hightxt, @"\s+", "");
                                    hightxt = hightxt.Replace('.', ',');
                                }
                                if (
                                    !double.TryParse(lowtxt,
                                                     NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                                                     CultureInfo.CurrentCulture, out low))
                                    low = 0;
                                else if (low < 0 || low > 100)
                                    low = 0;
                                if (
                                    !double.TryParse(hightxt,
                                                     NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                                                     CultureInfo.CurrentCulture, out high))
                                    high = 100;
                                else if (high > 100 || low > high || high < 0)
                                    high = 100;
                                Common.Types[size].Statlist.Value.Add(key,
                                                                      new Holder<Common.StatCheck>(
                                                                          new Common.StatCheck(low, high)));
                            }
                            break;

                            #endregion Statistics part

                            #region Binary Check Entry

                        case "binentry":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            key = xml["name"];
                            if (String.IsNullOrEmpty(key))
                                break;
                            if (!Common.Types[size].Bincheck.Value.ContainsKey(key))
                            {
                                if (xml["ismulti"] != null &&
                                    xml["ismulti"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Common.Types[size].Bincheck.Value.Add(key,
                                                                          new Holder<Common.BinCheck>(
                                                                              new Common.BinCheck(
                                                                                  new List<Common.MultiBin>(), true,
                                                                                  xml["offset"], xml["description"],
                                                                                  xml["ascii"])));
                                    string id = xml["id"];
                                    xml.Read();
                                    Common.Types[size].Bincheck.Value[key].Value.ExpectedList.Value.Add(
                                        new Common.MultiBin(Regex.Replace(xml.Value, @"\s+", ""), id));
                                }
                                else
                                {
                                    string offset = xml["offset"];
                                    string description = xml["description"];
                                    string ascii = xml["ascii"];
                                    xml.Read();
                                    Common.Types[size].Bincheck.Value.Add(key,
                                                                          new Holder<Common.BinCheck>(
                                                                              new Common.BinCheck(null, false, offset,
                                                                                                  description, ascii,
                                                                                                  xml.Value)));
                                }
                            }
                            if (xml["ismulti"] != null &&
                                xml["ismulti"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string id = xml["id"];
                                xml.Read();
                                Common.Types[size].Bincheck.Value[key].Value.ExpectedList.Value.Add(
                                    new Common.MultiBin(Regex.Replace(xml.Value, @"\s+", ""), id));
                            }
                            break;

                            #endregion Binary Check Entry

                            #region Data Check list

                        case "datalist":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var dataCheckList = new Common.DataCheck
                                                    {
                                                        Name = xml["name"],
                                                        ThresholdList = new Dictionary<string, double>()
                                                    };
                            if (long.TryParse(xml["offset"], NumberStyles.HexNumber, CultureInfo.CurrentCulture,
                                              out dataCheckList.Offset))
                            {
                                dataCheckOk = long.TryParse(xml["size"], NumberStyles.HexNumber,
                                                            CultureInfo.CurrentCulture, out dataCheckList.Size);
                                if (!dataCheckOk)
                                    dataCheckOk = long.TryParse(xml["ldrsize"], NumberStyles.HexNumber,
                                                                CultureInfo.CurrentCulture, out dataCheckList.LdrSize);
                                if (dataCheckOk)
                                {
                                    dataCheckKey++;
                                    dataCheckList.DataKey = dataCheckKey;
                                    Common.Types[size].DataCheckList.Value.Add(dataCheckList);
                                }
                            }
                            break;

                            #region Data treshold

                        case "datatreshold":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            if (!dataCheckOk)
                                break;
                            foreach (Common.DataCheck entry in Common.Types[size].DataCheckList.Value)
                            {
                                if (entry.DataKey != dataCheckKey)
                                    continue;
                                string dkey = xml["key"];
                                if (dkey == null)
                                    break;
                                if (string.IsNullOrEmpty(dkey))
                                    dkey = "*";
                                dkey = dkey.ToUpper();
                                xml.Read();
                                string tmptxt = xml.Value;
                                if (tmptxt != null)
                                {
                                    tmptxt = Regex.Replace(tmptxt, @"\s+", "");
                                    tmptxt = tmptxt.Replace('.', ',');
                                }
                                double tmpval;
                                if (
                                    !double.TryParse(tmptxt,
                                                     NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                                                     CultureInfo.CurrentCulture, out tmpval))
                                    tmpval = 49;
                                else if (tmpval < 0 || tmpval > 100)
                                    tmpval = 49;
                                entry.ThresholdList.Add(dkey, tmpval);
                                break;
                            }
                            break;

                            #endregion Data treshold

                            #endregion Data Check List

                            #region SKU Data List

                        case "skudataentry":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var skuDataEntry = new Common.SKUDataEntry();
                            string skuoffset = xml["offset"];
                            bool isok =
                                (uint.TryParse(skuoffset, NumberStyles.HexNumber, CultureInfo.CurrentCulture,
                                               out skuDataEntry.Offset));
                            if (isok)
                            {
                                string skusize = xml["size"];
                                isok =
                                    (uint.TryParse(skusize, NumberStyles.HexNumber, CultureInfo.CurrentCulture,
                                                   out skuDataEntry.Size));
                            }
                            xml.Read();
                            skuDataEntry.Type = xml.Value;
                            if (isok)
                                Common.Types[size].SKUDataList.Value.Add(skuDataEntry);
                            break;

                            #endregion SKU Data List

                            #region SKU List

                        case "skulist":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            skUkey++;
                            skuWarn = (xml["warn"].Equals("true", StringComparison.CurrentCultureIgnoreCase));
                            skuName = xml["name"];
                            skuWarnMsg = xml["warnmsg"];
                            skuMinVer = xml["minver"];
                            break;
                        case "skuentry":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            bool exists = false;
                            foreach (Common.SKUEntry entry in Common.Types[size].SKUList.Value)
                            {
                                if (entry.SKUKey != skUkey)
                                    continue;
                                exists = true;
                                break;
                            }
                            var skuEntry = new Common.SKUEntry();
                            if (!exists)
                            {
                                skuEntry.Warn = skuWarn;
                                skuEntry.WarnMsg = skuWarnMsg;
                            }
                            skuEntry.SKUKey = skUkey;
                            skuEntry.Name = skuName;
                            skuEntry.Type = xml["type"];
                            skuEntry.MinVer = skuMinVer;
                            xml.Read();
                            skuEntry.Data = Regex.Replace(xml.Value, @"\s+", "");
                            Common.Types[size].SKUList.Value.Add(skuEntry);
                            break;

                            #endregion SKU List
                    }
                }
            }
            Common.SendStatus("Parsing done!");
            checkbtn.Enabled = true;
        }

        private void MainDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MainDragDrop(object sender, DragEventArgs e)
        {
            if (worker.IsBusy)
                return;
            var fileList = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string s in fileList)
            {
                if (s.EndsWith(".cfg", StringComparison.CurrentCultureIgnoreCase))
                    ParseConfig(s);
                else
                {
                    if (worker.IsBusy)
                        return;
                    StartCheck(s);
                }
            }
        }

        private static void ExtractResource(FileInfo fi, string resource)
        {
            FileStream toexe = fi.OpenWrite();
            Stream fromexe = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            const int size = 4096;
            var bytes = new byte[size];
            int numBytes;
            while (fromexe != null && (numBytes = fromexe.Read(bytes, 0, size)) > 0)
                toexe.Write(bytes, 0, numBytes);
            toexe.Close();
            if (fromexe != null)
                fromexe.Close();
        }
    }
}