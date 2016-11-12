namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;
    using PS3DumpChecker.Patches;
    using PS3DumpChecker.Properties;

    internal sealed partial class MainForm : Form {
        private static string _version;
        public static readonly string Wrkdir = Path.GetDirectoryName(Application.ExecutablePath);
        private readonly bool _autoCheck;
        private readonly string _autoCheckFile;
        private UpdateForm _updateForm = new UpdateForm();

        public MainForm(ICollection <string> args) {
            InitializeComponent();
            Common.StatusUpdate += StatusUpdate;
            Common.ListUpdate += CommonOnListUpdate;
            var app = Assembly.GetExecutingAssembly();
            _version = string.Format("PS3 Dump Checker v{0}.{1} (Build: {2})", app.GetName().Version.Major, app.GetName().Version.Minor, app.GetName().Version.Build);
            Text = _version;
            Icon = Program.AppIcon;
            actdatabox.Font = new Font(FontFamily.GenericMonospace, actdatabox.Font.Size);
            expdatabox.Font = new Font(FontFamily.GenericMonospace, actdatabox.Font.Size);
            //partslist.Font = new Font(FontFamily.GenericMonospace, partslist.Font.Size);  //font too wide for the listbox
            foreach (var control in imginfo.Controls)
            {
                if (control is Label)
                    continue;
                var rtbox = control as RichTextBox;
                if (rtbox == null)
                    continue;
                rtbox.Font = new Font(FontFamily.GenericMonospace, rtbox.Font.Size);
            }
            if (args.Count < 1)
                return;
            foreach (var s in args) {
                if (!File.Exists(s))
                    continue;
				if(s.EndsWith("updatehelper.exe", StringComparison.CurrentCultureIgnoreCase)) {
                    try {
                        File.Delete(s);
                    }
                    catch(Exception ex) { }
                    continue;
                }
                if (_autoCheck)
                    return;
                _autoCheck = true;
                _autoCheckFile = s;
            }
        }

        public void DoParseHashList() {
            var fi = new FileInfo("default.hashlist");
            if (fi.Exists && fi.Length > 0)
                Common.Hashes = new HashCheck("default.hashlist");
            else {
                Program.ExtractResource(fi, "hashlist.xml", false);
                fi = new FileInfo("default.hashlist");
                if (fi.Exists && fi.Length > 0)
                    Common.Hashes = new HashCheck("default.hashlist");
            }
        }

        private void CommonOnListUpdate(object sender, EventArgs eventArgs) {
            if (InvokeRequired) {
                BeginInvoke(new EventHandler <EventArgs>(CommonOnListUpdate), new[] {
                    sender,
                    eventArgs
                });
                return;
            }
            try {
                partslist.Items.Clear();
                if (Common.PartList.Keys.Count <= 0)
                    return;
                foreach (var key in Common.PartList.Keys)
                    partslist.Items.Add(new ListBoxItem(key, Common.PartList[key].Name));
            }
            catch {
            }
        }

        private void StatusUpdate(object sender, StatusEventArgs e) {
            if (InvokeRequired) {
                BeginInvoke(new EventHandler <StatusEventArgs>(StatusUpdate), new[] {
                    sender,
                    e
                });
                return;
            }
            status.Text = e.Status;
        }

        private void SetTitle(string title) {
            if (InvokeRequired) {
                Invoke(new Action <string>(SetTitle), new object[] {
                    title
                });
                return;
            }
            Text = title;
        }

        private void DoWork(object sender, DoWorkEventArgs e) {
            var file = e.Argument.ToString();
            if (string.IsNullOrEmpty(file))
                return;
            var sw = new Stopwatch();
            var fi = new FileInfo(file);
            if (Common.Types.ContainsKey(fi.Length)) {
                SetTitle(string.Format("{0} Type: {2} File: {1}", _version, Path.GetFileName(file), Common.Types[fi.Length].Name.Value));
                Common.SendStatus("Reading image into memory and checking statistics...");
                Logger.LogPath = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "_PS3Check.log";
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

        private void CheckbtnClick(object sender, EventArgs e) {
            var ofd = new OpenFileDialog {
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

        private void StartCheck(string file) {
            Common.PartList.Clear();
            partslist.Items.Clear();
            imgstatus.Text = Resources.N_A;
            reversed.Text = Resources.N_A;
            idmatchbox.Text = Resources.N_A;
            minverbox.Text = Resources.N_A;
            rosver0box.Text = Resources.N_A;
            rosver1box.Text = Resources.N_A;
            isprepatchedbox.Text = Resources.N_A;
            statuslabel.Visible = false;
            actdatabox.Text = "";
            expdatabox.Text = "";
            checkbtn.Enabled = false;
            forcePatchToolStripMenuItem.Enabled = false;
            Logger.Enabled = logstate.Checked;
            worker.RunWorkerAsync(file);
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Result == null && Common.Types.Count > 0)
                MessageBox.Show(Resources.badsize, Resources.error_bad_size, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (Common.Types.Count <= 0)
                MessageBox.Show(Resources.error_noconfig, Resources.error_noconfig_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                try {
                    if (e.Result != null) {
                        var res = (Common.ImgInfo) e.Result;
                        imgstatus.Text = res.Status;
                        reversed.Text = res.Reversed ? "Yes" : "No";
                        isprepatchedbox.Text = res.IsPatched ? "Yes" : "No";
                        idmatchbox.Text = res.SKUModel ?? "No matching SKU model found!";
                        minverbox.Text = res.MinVer ?? Resources.N_A;
                        statuslabel.Text = res.IsOk ? "OK" : "BAD";
                        rosver0box.Text = res.ROS0Version ?? Resources.N_A;
                        rosver1box.Text = res.ROS1Version ?? Resources.N_A;
                        statuslabel.ForeColor = res.IsOk ? Color.Green : Color.Red;
                        statuslabel.Visible = true;
                        forcePatchToolStripMenuItem.Enabled = true;
                        Common.dmpname = res.FileName;
                        Common.swapstate = res.Reversed;
                        Common.chkresult = res.IsOk;
                        if (res.IsOk && !res.DisablePatch && !res.IsPatched && Program.GetRegSetting("autopatch"))
                            Patch(res.FileName, res.Reversed);
                    }
                }
                catch {
                }
            }
            if (Common.Types.Count > 0)
                checkbtn.Enabled = true;
        }

        private void Patch(string fileName, bool swap) {
            var useint = Program.GetRegSetting("UseInternalPatcher");
            if (!useint && !File.Exists("patcher.exe"))
                return;
            if (Common.chkresult) {
                if (MessageBox.Show(Resources.autopatchmsg, Resources.autopatch, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }
            else {
                if (MessageBox.Show(Resources.warningforcepatch, Resources.autopatch, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            if (useint)
                Patcher.PatchImage(fileName, swap);
            else {
                var proc = new Process {
                    StartInfo = {
                        Arguments = string.Format("\"{0}\"", fileName),
                        FileName = "patcher.exe",
                        WorkingDirectory = Wrkdir
                    }
                };
                proc.Start();
            }
            if (Program.GetRegSetting("autoexit"))
                Close();
        }

        private void PartslistSelectedIndexChanged(object sender, EventArgs e) {
            if (partslist.SelectedItems.Count == 0 || partslist.SelectedIndex < 0)
                return;
            if (!(partslist.SelectedItems[0] is ListBoxItem))
                return;
            var tmp = partslist.SelectedItems[0] as ListBoxItem;
            if (!Common.PartList.ContainsKey(tmp.Value))
                return;
            var obj = Common.PartList[tmp.Value];
            expdatabox.Text = string.Format("Result of the check: {1}{0}{0}", Environment.NewLine, obj.Result);
            expdatabox.Text += obj.ExpectedString;
            actdatabox.Text = obj.ActualString;
        }

        private void LoadConfigurationToolStripMenuItemClick(object sender, EventArgs e) {
            var ofd = new OpenFileDialog {
                Title = Resources.select_conf,
                FileName = "Default.cfg",
                DefaultExt = "cfg",
                Filter = Resources.conf_filter,
                AutoUpgradeEnabled = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                ParseConfig(ofd.FileName);
        }

        public void ParseConfig(string file) {
            Common.SendStatus(string.Format("Parsing {0}", file));
            Common.Types.Clear();
            using (var xml = XmlReader.Create(file)) {
                var dataCheckKey = 0;
                var dataCheckOk = false;
                var skUkey = 0;
                long size = 0;
                var skuWarn = false;
                var skuName = "";
                var skuWarnMsg = "";
                var skuMinVer = "";
                while (xml.Read()) {
                    if (!xml.IsStartElement())
                        continue;
                    switch (xml.Name.ToLower()) {
                        case "type":
                            if (long.TryParse(xml["size"], out size)) {
                                if (!Common.Types.ContainsKey(size))
                                    Common.Types.Add(size, new Common.TypeData());
                                Common.Types[size].Name.Value = xml["name"];
                            }
                            break;

                            #region Statistics part

                        case "stats":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            xml.Read();
                            Common.Types[size].StatDescription.Value = xml.Value;
                            if (!string.IsNullOrEmpty(Common.Types[size].StatDescription.Value)) {
                                Common.Types[size].StatDescription.Value = Common.Types[size].StatDescription.Value.Trim();
                                Common.Types[size].StatDescription.Value += Environment.NewLine;
                                Common.Types[size].StatDescription.Value.Replace("\\n", Environment.NewLine);
                            }
                            break;
                        case "statspart":
                            var key = xml["key"];
                            if (key == null)
                                break;
                            if (string.IsNullOrEmpty(key))
                                key = "*";
                            key = key.ToUpper();
                            if (!Common.Types[size].Statlist.Value.ContainsKey(key)) {
                                double low;
                                double high;
                                var lowtxt = xml["low"];
                                if (lowtxt != null) {
                                    lowtxt = Regex.Replace(lowtxt, @"\s+", "");
                                    lowtxt = lowtxt.Replace('.', ',');
                                }
                                var hightxt = xml["high"];
                                if (hightxt != null) {
                                    hightxt = Regex.Replace(hightxt, @"\s+", "");
                                    hightxt = hightxt.Replace('.', ',');
                                }
                                if (!double.TryParse(lowtxt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out low))
                                    low = 0;
                                else if (low < 0 || low > 100)
                                    low = 0;
                                if (!double.TryParse(hightxt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out high))
                                    high = 100;
                                else if (high > 100 || low > high || high < 0)
                                    high = 100;
                                Common.Types[size].Statlist.Value.Add(key, new Holder <Common.StatCheck>(new Common.StatCheck(low, high)));
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
                            if (!Common.Types[size].Bincheck.Value.ContainsKey(key)) {
                                if (xml["ismulti"] != null && xml["ismulti"].Equals("true", StringComparison.CurrentCultureIgnoreCase)) {
                                    Common.Types[size].Bincheck.Value.Add(key, new Holder <Common.BinCheck>(new Common.BinCheck(new List <Common.MultiBin>(), true, xml["offset"], xml["description"], xml["ascii"])));
                                    var id = xml["id"];
                                    xml.Read();
                                    Common.Types[size].Bincheck.Value[key].Value.ExpectedList.Value.Add(new Common.MultiBin(Regex.Replace(xml.Value, @"\s+", ""), id));
                                }
                                else {
                                    var offset = xml["offset"];
                                    var description = xml["description"];
                                    var ascii = xml["ascii"];
                                    xml.Read();
                                    Common.Types[size].Bincheck.Value.Add(key, new Holder <Common.BinCheck>(new Common.BinCheck(null, false, offset, description, ascii, xml.Value)));
                                }
                            }
                            else if (xml["ismulti"] != null && xml["ismulti"].Equals("true", StringComparison.CurrentCultureIgnoreCase)) {
                                var id = xml["id"];
                                var disablepatch = !string.IsNullOrEmpty(xml["disablepatch"]) && xml["disablepatch"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                xml.Read();
                                var explist = Common.Types[size].Bincheck.Value[key].Value;
                                explist.ExpectedList.Value.Add(new Common.MultiBin(Regex.Replace(xml.Value, @"\s+", ""), id, disablepatch));
                            }
                            break;

                            #endregion Binary Check Entry

                            #region Data Check list

                        case "datalist":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var dataCheckList = new Common.DataCheck {
                                Name = xml["name"],
                                ThresholdList = new Dictionary <string, double>()
                            };
                            if (long.TryParse(xml["offset"], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out dataCheckList.Offset)) {
                                dataCheckOk = long.TryParse(xml["size"], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out dataCheckList.Size);
                                if (!dataCheckOk)
                                    dataCheckOk = long.TryParse(xml["ldrsize"], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out dataCheckList.LdrSize);
                                if (dataCheckOk) {
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
                            foreach (var entry in Common.Types[size].DataCheckList.Value) {
                                if (entry.DataKey != dataCheckKey)
                                    continue;
                                var dkey = xml["key"];
                                if (dkey == null)
                                    break;
                                if (string.IsNullOrEmpty(dkey))
                                    dkey = "*";
                                dkey = dkey.ToUpper();
                                xml.Read();
                                var tmptxt = xml.Value;
                                if (tmptxt != null) {
                                    tmptxt = Regex.Replace(tmptxt, @"\s+", "");
                                    tmptxt = tmptxt.Replace('.', ',');
                                }
                                double tmpval;
                                if (!double.TryParse(tmptxt, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out tmpval))
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
                            var skuoffset = xml["offset"];
                            var isok = (uint.TryParse(skuoffset, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out skuDataEntry.Offset));
                            if (isok) {
                                var skusize = xml["size"];
                                isok = (uint.TryParse(skusize, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out skuDataEntry.Size));
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
                            skuWarn = xml["warn"] != null && xml["warn"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
                            skuName = xml["name"];
                            skuWarnMsg = xml["warnmsg"];
                            skuMinVer = xml["minver"];
                            break;
                        case "skuentry":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var exists = false;
                            foreach (var entry in Common.Types[size].SKUList.Value) {
                                if (entry.SKUKey != skUkey)
                                    continue;
                                exists = true;
                                break;
                            }
                            var skuEntry = new Common.SKUEntry();
                            if (!exists) {
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

                            #region RepCheck Entry

                        case "repcheck":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var tmp = new Common.RepCheckData {
                                Name = xml["name"]
                            };
                            if (!int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out tmp.Offset))
                                break; // It's broken!
                            xml.Read();
                            if (!string.IsNullOrEmpty(xml.Value)) {
                                var data = Encoding.Unicode.GetString(Common.HexToArray(Regex.Replace(xml.Value, @"\s+", "").ToUpper()));
                                Common.Types[size].RepCheck.Value.Add(data, new Holder <Common.RepCheckData>(tmp));
                            }
                            break;

                            #endregion

                            #region Datamatches Entries

                        case "datamatchid":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var tmpid = xml["id"];
                            if (string.IsNullOrEmpty(tmpid))
                                break;
                            if (!Common.Types[size].DataMatchList.Value.ContainsKey(tmpid))
                                Common.Types[size].DataMatchList.Value.Add(tmpid, new Holder <Common.DataMatchID>(new Common.DataMatchID()));
                            xml.Read();
                            if (string.IsNullOrEmpty(xml.Value))
                                break;
                            Common.Types[size].DataMatchList.Value[tmpid].Value.Name = xml.Value;
                            break;

                        case "datamatch":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var tmpid2 = xml["id"];
                            if (string.IsNullOrEmpty(tmpid2))
                                break;
                            if (!Common.Types[size].DataMatchList.Value.ContainsKey(tmpid2))
                                break;
                            var tmpmatch = new Common.DataMatch();
                            if (!int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out tmpmatch.Offset))
                                break;
                            if (!int.TryParse(xml["length"], NumberStyles.HexNumber, null, out tmpmatch.Length))
                                break;
                            if (!int.TryParse(xml["seqrep"], NumberStyles.HexNumber, null, out tmpmatch.SequenceRepetitions))
                                tmpmatch.SequenceRepetitions = 0;
                            tmpmatch.DisableDisplay = !string.IsNullOrEmpty(xml["nodisp"]) && xml["nodisp"].Equals("true", StringComparison.CurrentCultureIgnoreCase);                            xml.Read();
                            if (string.IsNullOrEmpty(xml.Value))
                                break;
                            tmpmatch.Name = xml.Value;
                            Common.Types[size].DataMatchList.Value[tmpid2].Value.Data.Add(tmpmatch);
                            break;

                            #endregion

                            #region Datafill Entry

                        case "datafill":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            var datafill = new Common.DataFillEntry
                            {
                                Name = xml["name"]
                            };
                            int.TryParse(xml["offset"], NumberStyles.HexNumber, null, out datafill.Offset);
                            int.TryParse(xml["size"], NumberStyles.HexNumber, null, out datafill.Length);
                            int.TryParse(xml["sizefrom"], NumberStyles.HexNumber, null, out datafill.Sizefrom);
                            int.TryParse(xml["ldrsize"], NumberStyles.HexNumber, null, out datafill.LdrSize);
                            int.TryParse(xml["regionsize"], NumberStyles.HexNumber, null, out datafill.RegionSize);
                            int.TryParse(xml["regionstart"], NumberStyles.HexNumber, null, out datafill.RegionStart);
                            int.TryParse(xml["vtrmentrycount_offset"], NumberStyles.HexNumber, null, out datafill.vtrmentrycount_offset);
                            xml.Read();
                            if (!byte.TryParse(xml.Value, NumberStyles.HexNumber, null, out datafill.Data))
                                break;
                            Common.Types[size].DataFillEntries.Value.Add(datafill);
                            break;

                            #endregion

                            #region ROS#Offset

                        case "ros0offset":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            xml.Read();
                            if (!int.TryParse(xml.Value, NumberStyles.HexNumber, null, out Common.Types[size].ROS0Offset))
                                Common.Types[size].ROS0Offset = -1;
                            break;
                        case "ros1offset":
                            if (!Common.Types.ContainsKey(size))
                                break;
                            xml.Read();
                            if (!int.TryParse(xml.Value, NumberStyles.HexNumber, null, out Common.Types[size].ROS1Offset))
                                Common.Types[size].ROS1Offset = -1;
                            break;

                            #endregion
                    }
                }
            }
            Common.SendStatus("Parsing done!");
            checkbtn.Enabled = true;
        }

        private void MainDragEnter(object sender, DragEventArgs e) { e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; }

        private void MainDragDrop(object sender, DragEventArgs e) {
            if (worker.IsBusy)
                return;
            var fileList = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            foreach (var s in fileList) {
                if (s.EndsWith(".cfg", StringComparison.CurrentCultureIgnoreCase))
                    ParseConfig(s);
                else if (s.EndsWith(".hashlist", StringComparison.CurrentCultureIgnoreCase))
                    Common.Hashes = new HashCheck(s);
                else
                    StartCheck(s);
            }
        }

        private void UpdateClick(object sender, EventArgs e) {
            if (_updateForm == null)
                _updateForm = new UpdateForm();
            _updateForm.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case (Keys.F12 | Keys.Control):
                    SettingsClick(null, null);
                    return true;
                case (Keys.F1 | Keys.Control):
                    Program.HasAcceptedTerms(true);
                    return true;
                case (Keys.F9 | Keys.Control):
                    new Simulation().ShowDialog();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void LoadHashlistToolStripMenuItemClick(object sender, EventArgs e) {
            var ofd = new OpenFileDialog {
                Title = Resources.select_conf,
                FileName = "Default.hashlist",
                DefaultExt = "hashlist",
                Filter = Resources.hashlist_filter,
                AutoUpgradeEnabled = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                Common.Hashes = new HashCheck(ofd.FileName);
        }

        private void MainLoad(object sender, EventArgs e) {
  
            #region Set Some defaults
            if (Program.GetRegSetting("logstate", true))
                Program.SetRegSetting("logstate");
            if (Program.GetRegSetting("autopatch", true))
                Program.SetRegSetting("autopatch");
            if (Program.GetRegSetting("dohashcheck", true))
                Program.SetRegSetting("dohashcheck");
            if (Program.GetRegSetting("dorepcheck", true))
                Program.SetRegSetting("dorepcheck");
            if (Program.GetRegSetting("dorosvercheck", true))
                Program.SetRegSetting("dorosvercheck");
            if (Program.GetRegSetting("UseInternalPatcher", true))
                Program.SetRegSetting("UseInternalPatcher");
            #endregion

            logstate.Checked = Program.GetRegSetting("logstate");
            forcePatchToolStripMenuItem.Visible = Program.GetRegSetting("forcepatch");

#if DEBUG
            File.Delete("default.cfg");
            File.Delete("default.hashlist");
#endif
#if !DEBUG
            if(Program.GetRegSetting("AutoDLcfg"))
                _updateForm.CfgbtnClick(false);
#endif
            var fi = new FileInfo("default.cfg");
            if (fi.Exists && fi.Length > 0)
                ParseConfig("default.cfg");
            else {
                Program.ExtractResource(fi, "config.xml", false);
                fi = new FileInfo("default.cfg");
                if (fi.Exists && fi.Length > 0)
                    ParseConfig("default.cfg");
            }
            if (Program.GetRegSetting("dohashcheck", true)) {
#if !DEBUG
                if(Program.GetRegSetting("AutoDLhashlist"))
                    _updateForm.HashlistbtnClick(false);
#endif
                DoParseHashList();
            }
            if (_autoCheck)
                StartCheck(_autoCheckFile);
            if (Screen.FromControl(this).Bounds.Height >= Height)
                return;
            var diff = Height - Screen.FromControl(this).Bounds.Height;
            Height = Height - diff - 45;
            expdatabox.Height = expdatabox.Height - diff;
            actdatabox.Height = actdatabox.Height - diff;
            actdatabox.Location = new Point(actdatabox.Location.X, actdatabox.Location.Y + diff);
            statuslabel.Location = new Point(statuslabel.Location.X, statuslabel.Location.Y + diff - 23);
            statuslabel.Font = new Font(statuslabel.Font.FontFamily, 30, FontStyle.Bold);
            advbox.Height = advbox.Height - diff - 10;
        }

        private void SettingsClick(object sender, EventArgs e) {
            using (var set = new Settings())
                set.ShowDialog();
        }

        private void logstate_CheckedChanged(object sender, EventArgs e) { Program.SetRegSetting("logstate", logstate.Checked); }

        private void forcePatchToolStripMenuItem_Click(object sender, EventArgs e) { Patch(Common.dmpname, Common.swapstate); }

        public void forcepatchstate() { forcePatchToolStripMenuItem.Visible = Program.GetRegSetting("forcepatch"); }
    }
}