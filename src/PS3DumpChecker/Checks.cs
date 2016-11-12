namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal static class Checks {
        private static int _checkId;
        private static Common.ImgInfo _ret;
        private static int _checkckount;
        private static bool _dohash;
        private static Common.TypeData _checkdata;

        private static string GetAsciiString(byte[] data, int offset = 0, int length = -1) {
            if(length <= 0 && offset > 0)
                length = data.Length - offset;
            else
                length = data.Length;
            return Regex.Replace(Encoding.ASCII.GetString(data, offset, length), "[^\u0020-\u007E]", " ");
            //var ret = new StringBuilder();
            //foreach (var c in Encoding.ASCII.GetString(data, offset, length))
            //{
            //    if (c > 0x1F && c < 0x7F)
            //        ret.Append(c);
            //    else
            //        ret.Append(" ");
            //}
            //return ret.ToString();
        }

        private static void AddItem(Common.PartsObject data) {
            Common.AddItem(_checkId, data);
            _checkId++;
        }

        public static Common.ImgInfo StartCheck(string file, ref Stopwatch sw) {
            _checkId = 0;
            var fi = new FileInfo(file);
            _checkckount = 0;
            _ret = new Common.ImgInfo {
                FileName = file
            };
            _checkdata = Common.Types[fi.Length];
            var data = new byte[fi.Length];

            #region Statistics check

            if(_checkdata.Statlist.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,92}", "Statistics check started..."));
                _checkckount++;
                if(!CheckStatisticsList(GetStatisticsAndFillData(fi, ref data), data.Length))
                    Common.AddBad(ref _ret);
                Common.SendStatus("Statistics check Done!");
            }
            else {
                Common.SendStatus("Skipping Statistics check (nothing to check) Instead: Reading image into memory...");
                data = File.ReadAllBytes(fi.FullName);
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Statistics check skipped!"));
            }

            #endregion Statistics check

            #region Binary check

            if(_checkdata.Bincheck.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,92}", "Binary check Started!"));
                foreach(var key in _checkdata.Bincheck.Value.Keys) {
                    _checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Binary for: {0}", key));
                    var bintmp = string.Format("Binary check for {0} Started...", key);
                    Logger.Write(string.Format("{0,-70} Result: ", bintmp));
                    if(!_checkdata.Bincheck.Value[key].Value.IsMulti) {
                        if(!CheckBinPart(ref data, key, ref _ret.Reversed))
                            Common.AddBad(ref _ret);
                    }
                    else if(!CheckBinPart(ref data, key))
                        Common.AddBad(ref _ret);
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Binary check skipped!"));
            Common.SendStatus("Binary check(s) Done!");

            #endregion Binary check

            #region Data check

            if(_checkdata.DataCheckList.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,90}", "Data check Started!"));
                foreach(var key in _checkdata.DataCheckList.Value) {
                    _checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Data Statistics for: {0}", key.Name));
                    var datatmp = string.Format("Data Statistics check for {0} Started...", key.Name);
                    Logger.Write(string.Format("{0,-70} Result: ", datatmp));
                    if(!CheckDataPart(ref data, key, _ret.Reversed))
                        Common.AddBad(ref _ret);
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Data check skipped!"));
            Common.SendStatus("Data check(s) Done!");

            #endregion Data check

            #region Hash check

            _dohash = Program.GetRegSetting("dohashcheck", true);
            if(_dohash && Common.Hashes != null && Common.Hashes.Offsets.ContainsKey(data.Length) && Common.Hashes.Offsets[data.Length].Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,90}", "Hash check Started!"));
                foreach(var check in Common.Hashes.Offsets[data.Length].Value) {
                    _checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Hash for: {0}", check.Name));
                    var hashtmp = string.Format("Hash check for {0} Started...", check.Name);
                    Logger.Write(string.Format("{0,-70} Result: ", hashtmp));
                    if(!CheckHash(_ret.Reversed, ref data, check))
                        Common.AddBad(ref _ret);
                    if (HashCheck.LastIsPatched)
                        _ret.IsPatched = true;
                }
            }
            else if (_dohash)
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Hash check skipped!"));
            else
                Logger.WriteLine(string.Format("{0,-70} (disabled)", "Hash check skipped!"));
            Common.SendStatus("Hash check(s) Done!");

            #endregion Hash check

            #region ROSVersion check

            if(Program.GetRegSetting("dorosvercheck", true) && _checkdata.ROS0Offset > 0 && _checkdata.ROS1Offset > 0) {
                Logger.WriteLine(string.Format("{0,97}", "ROS Version check Started!"));
                _checkckount++;
                Common.SendStatus("Parsing Image... Checking ROS0 Version");
                Logger.Write(string.Format("{0,-70} Result: ", "ROS Version check for ROS0 Started..."));
                var ret = CheckROSVersion(ref data, _checkdata.ROS0Offset, out _ret.ROS0Version);
                if (!ret)
                    Common.AddBad(ref _ret);
                Logger.WriteLine2(!ret ? "FAILED!" : string.Format("OK! ({0})", _ret.ROS0Version));
                AddItem(new Common.PartsObject { ActualString = _ret.ROS0Version, ExpectedString = "ROS0 version in the format: ###.###", Name = "009.03   ROS0 Version", Result = ret });
                _checkckount++;
                Common.SendStatus("Parsing Image... Checking ROS0 Version");
                Logger.Write(string.Format("{0,-70} Result: ", "ROS Version check for ROS1 Started..."));
                ret = CheckROSVersion(ref data, _checkdata.ROS1Offset, out _ret.ROS1Version);
                if (!ret)
                    Common.AddBad(ref _ret);
                Logger.WriteLine2(!ret ? "FAILED!" : string.Format("OK! ({0})", _ret.ROS1Version));
                AddItem(new Common.PartsObject { ActualString = _ret.ROS1Version, ExpectedString = "ROS1 version in the format: ###.###", Name = "009.06   ROS1 Version", Result = ret });
            }
            else if (Program.GetRegSetting("dorosvercheck", true))
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "ROS Version check skipped!"));
            else
                Logger.WriteLine(string.Format("{0,-70} (disabled)", "ROS Version check skipped!"));
            Common.SendStatus("ROS Version checks Done!");

            #endregion Hash check

            #region Repetitions Check

            if(Program.GetRegSetting("dorepcheck", true) && _checkdata.RepCheck.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,97}", "Repetitions check Started!"));
                Common.SendStatus("Parsing Image... Checking Binary for: Repetitions");
                if(!Repetitions(_ret.Reversed, ref data, ref _checkdata))
                    Common.AddBad(ref _ret);
            }
            else if (Program.GetRegSetting("dorepcheck", true))
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Repetitions check skipped!"));
            else
                Logger.WriteLine(string.Format("{0,-70} (disabled)", "Repetitions check skipped!"));
            Common.SendStatus("Repetitions check(s) Done!");

            #endregion

            #region DataMatch Check

            if(_checkdata.DataMatchList.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,96}", "Data Match check Started!"));
                Common.SendStatus("Parsing Image... Checking Binary for: Data Matches");
                if (!CheckDataMatches(ref data, ref _checkdata))
                    Common.AddBad(ref _ret);
                else
                    Logger.WriteLine("All is OK!");
            }
            else
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Data Match check skipped!"));
            Common.SendStatus("Data Match check(s) Done!");

            #endregion

            #region DataFill Check

            if (_checkdata.DataFillEntries.Value.Count > 0) {
                Logger.WriteLine(string.Format("{0,95}", "Data Fill check Started!"));
                foreach (var dataFillEntry in _checkdata.DataFillEntries.Value) {
                    _checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Data Fill for: {0}", dataFillEntry.Name));
                    Logger.Write(string.Format("{0,-70} Result: ", string.Format("Data Fill check for: {0} Started!", dataFillEntry.Name)));
                    if (!CheckDataFill(ref data, dataFillEntry, _ret.Reversed))
                        Common.AddBad(ref _ret);
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "Data Fill check skipped!"));
            Common.SendStatus("Data Fill check(s) Done!");

            #endregion
            
            //New checks goes here

            #region SKU List check

            if (_checkdata.SKUList.Value.Count > 0)
            {
                Logger.WriteLine(string.Format("{0,94}", "SKU List check Started!"));
                Common.SendStatus("Checking SKU List...");
                var skuCheckDataList = GetSkuCheckData(_ret.Reversed, ref data, ref _checkdata);

                var skuEntryList = new List<Common.SKUEntry>(_checkdata.SKUList.Value);
                foreach (var entry in skuCheckDataList)
                {
                    if (skuEntryList.Count < skuCheckDataList.Count)
                        break;
                    var tmplist = GetFilterList(skuEntryList, entry);
                    var tmplist2 = new List<Common.SKUEntry>(skuEntryList);
                    skuEntryList.Clear();
                    foreach (var skuEntry in tmplist2)
                    {
                        foreach (var tmpentry in tmplist)
                        {
                            if (skuEntry.SKUKey == tmpentry.SKUKey)
                                skuEntryList.Add(skuEntry);
                        }
                    }
                }
                var datamsg = "";
                foreach (var entry in skuCheckDataList)
                    datamsg += entry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase) ? string.Format("{0} = {1:X4}{2}", entry.Type, entry.Size, Environment.NewLine) : string.Format("{0} = {1}{2}", entry.Type, entry.Data, Environment.NewLine);
                if (skuEntryList.Count == skuCheckDataList.Count)
                {
                    _ret.SKUModel = skuEntryList[0].Name;
                    _ret.MinVer = skuEntryList[0].MinVer;
                    Logger.WriteLine(string.Format("SKU Model: {0}", _ret.SKUModel));
                    var msg = "";
                    if (skuEntryList[0].Warn)
                    {
                        foreach (var entry in skuEntryList)
                        {
                            if (string.IsNullOrEmpty(entry.WarnMsg))
                                continue;
                            msg = entry.WarnMsg;
                            break;
                        }
                        MessageBox.Show(msg, Resources.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Logger.WriteLine(msg);
                        Logger.WriteLine("");
                        datamsg += string.Format("{0}{1}", Environment.NewLine, msg);
                    }
                }
                else
                {
                    Common.AddBad(ref _ret);
                    _ret.SKUModel = null;
                    _ret.MinVer = null;
                    Logger.WriteLine("No matching SKU model found!");
                    foreach (var entry in skuCheckDataList)
                        Logger.WriteLine(entry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase) ? string.Format("{0} = {1:X4}", entry.Type, entry.Size) : string.Format("{0} = {1}", entry.Type, entry.Data));
                }
                AddItem(new Common.PartsObject
                {
                    Name = "SKUIdentity Data",
                    ActualString = datamsg.Trim(),
                    ExpectedString = "",
                    Result = (skuEntryList.Count == skuCheckDataList.Count),
                });
            }
            else
                Logger.WriteLine(string.Format("{0,-70} (nothing to check)", "SKU List check skipped!"));

            #endregion SKU List check

            #region Final Output

            var outstring = string.Format("All checks ({2} Checks) have been completed after {0} Second(s) and {1} Millisecond(s)", (int)sw.Elapsed.TotalSeconds, sw.Elapsed.Milliseconds, _checkckount);
            Common.SendStatus(outstring);
            Logger.WriteLine(outstring);
            _ret.IsOk = _ret.BadCount == 0;
            _ret.Status = _ret.IsOk ? "Dump has been validated!" : "Dump is bad!";
            if(!_ret.IsOk)
                MessageBox.Show(string.Format("ERROR: Your dump failed on {0} of {1} Checks\nPlease check the log for more information!", _ret.BadCount, _checkckount), Resources.Checks_StartCheck_ERROR___Bad_dump, MessageBoxButtons.OK, MessageBoxIcon.Error);
            var tmp = _ret.IsOk ? "Pass!" : "Failed!";
            var outtmp = _ret.IsOk ? string.Format("Tests done: {0}", _checkckount) : string.Format("Bad count: {0} of {1} Tests", _ret.BadCount, _checkckount);
            Logger.WriteLine2(string.Format("{0,-81} Result: {1}", outtmp, tmp));
            sw.Stop();

            #endregion Final Output

            return _ret;
        }

        private static bool CheckDataFill(ref byte[] data, Common.DataFillEntry dataFillEntry, bool reversed)
        {
            long Offset;
            long Length;
            if (dataFillEntry.LdrSize != 0 )
            {
                var tmpdata = new byte[2];
                Buffer.BlockCopy(data, (int)dataFillEntry.LdrSize, tmpdata, 0, tmpdata.Length);
                if (reversed)
                    Common.SwapBytes(ref tmpdata);
                long ldrlength = Common.GetLdrSize(ref tmpdata);
                Offset = dataFillEntry.RegionStart + ldrlength;
                Length = dataFillEntry.RegionSize - ldrlength;
            }
            else if (dataFillEntry.Sizefrom != 0)
            {
                var tmpdata = new byte[2];
                Buffer.BlockCopy(data, (int)dataFillEntry.Sizefrom, tmpdata, 0, tmpdata.Length);
                if (reversed)
                    Common.SwapBytes(ref tmpdata);
                long datalength = Common.GetSizefrom(ref tmpdata);
                Offset = dataFillEntry.RegionStart + datalength;
                Length = dataFillEntry.RegionSize - datalength;
            }
            else if (dataFillEntry.vtrmentrycount_offset != 0)
            {
                var vtrmentrycount = new byte[2];
                Buffer.BlockCopy(data, (int)dataFillEntry.vtrmentrycount_offset, vtrmentrycount, 0, vtrmentrycount.Length);
                if (reversed == false)
                    Common.SwapBytes(ref vtrmentrycount);
                var count = BitConverter.ToUInt16(vtrmentrycount, 0);
                var entrieslength = count * 0x60;
                Offset = dataFillEntry.RegionStart + entrieslength;
                Length = dataFillEntry.RegionSize - entrieslength;
            }
            else
            {
                Length = dataFillEntry.Length;
                Offset = dataFillEntry.Offset;
            }
            for (var i = Offset; i < Offset + Length; i++) {
                if (data[i] == dataFillEntry.Data)
                    continue;
                AddItem(new Common.PartsObject {
                    Name = dataFillEntry.Name,
                    ActualString = string.Format("The byte @ offset: 0x{0:X}\r\nhas the value: 0x{1:X2}\r\nPlease check the data further down the line manually...", i, data[i]),
                    ExpectedString = string.Format("The data between offset: 0x{0:X} and 0x{1:X} should be: {2:X2}", Offset, Offset + Length, dataFillEntry.Data),
                    Result = false
                });
                Logger.WriteLine2(string.Format("FAILED!\r\nThe byte @ offset: 0x{0:X}\r\nhas the value: 0x{1:X2}\r\nPlease check the data further down the line manually...", i, data[i]));
                return false;
            }
            AddItem(new Common.PartsObject
            {
                Name = dataFillEntry.Name,
                ActualString = "All is OK!",
                ExpectedString = string.Format("The data between offset: 0x{0:X} and 0x{1:X} should be: {2:X2}", Offset, Offset + Length, dataFillEntry.Data),
                Result = true
            });
            Logger.WriteLine2("OK!");
            return true;
        }

        private static Dictionary<byte, double> GetStatisticsAndFillData(FileInfo fi, ref byte[] data) {
            var count = new Dictionary<byte, ulong>();
            using(var br = new BinaryReader(fi.OpenRead())) {
                for(var i = 0; i < data.Length; i++) {
                    var b = br.ReadByte();
                    if(count.ContainsKey(b))
                        count[b]++;
                    else
                        count.Add(b, 1);
                    data[i] = b;
                }
            }
            var ret = new Dictionary<byte, double>();
            var statlist = string.Format("Stats for {0}\r\n", fi.FullName);
            for(var key = 0; key < 256; key++) {
                if(!count.ContainsKey((byte) key))
                    continue;
                ret.Add((byte) key, ((double) count[(byte) key] / data.Length) * 100);
                statlist += string.Format("0x{0:X2} = {1} bytes of {3} bytes ({2:F2}%)\r\n", key, count[(byte) key], ret[(byte) key], data.Length);
            }
            if(Logger.Enabled)
                File.WriteAllText(Path.GetDirectoryName(fi.FullName) + "\\" + Path.GetFileNameWithoutExtension(fi.FullName) + ".stats", statlist);
            return ret;
        }

        private static bool CheckStatisticsList(Dictionary<byte, double> tmp, int len) {
            var msg = "";
            var statlist = Common.Types[len].Statlist.Value;
            if(statlist == null || statlist.Count == 0)
                return true;
            var isok = true;
            foreach(var d in tmp.Keys) {
                var val = tmp[d];
                val = double.Parse(val.ToString("F2"));
                double low = 0;
                double high = 100;
                if(statlist.ContainsKey(d.ToString("X2"))) {
                    low = statlist[d.ToString("X2")].Value.Low;
                    high = statlist[d.ToString("X2")].Value.High;
                }
                else if(statlist.ContainsKey("*")) {
                    low = statlist["*"].Value.Low;
                    high = statlist["*"].Value.High;
                }
                if(low <= val && high >= val)
                    continue;
                Logger.WriteLine2(string.Format("Statistics check Failed! 0x{0:X2} doesn't match expected percentage: higher then {1}% lower then {2}% Actual value: {3:F2}%", d, low, high, val));
                isok = false;
            }
            var list = new List<byte>(tmp.Keys);
            list.Sort();
            foreach(var key in list)
                msg += String.Format("0x{0:X2} : {1:F2}%{2}", key, tmp[key], Environment.NewLine);
            AddItem(new Common.PartsObject {
                Name = "Statistics",
                ActualString = msg.Trim(),
                ExpectedString = Common.Types[len].StatDescription.Value,
                Result = isok
            });
            Logger.WriteLine2(string.Format("{0,-70} Result: {1}", "Statistics check Completed!", isok ? "OK!" : "FAILED!"));
            return isok;
        }

        private static bool CheckBinPart(ref byte[] data, string name, ref bool reversed) {
            var datareversed = false;
            var checkdata = Common.Types[data.Length].Bincheck.Value[name];
            if(checkdata.Value.Offset >= data.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset)!");
                return false;
            }
            var expmsg = string.Format("{0}Offset: 0x{2:X}{1}", checkdata.Value.Description, Environment.NewLine, checkdata.Value.Offset);
            if(!string.IsNullOrEmpty(checkdata.Value.Expected)) {
                if((checkdata.Value.Expected.Length % 2) != 0) {
                    Logger.WriteLine2("FAILED! Nothing to check! (a.k.a Faulty configuration!)");
                    return false;
                }
                expmsg += string.Format("Expected data:{0}", Environment.NewLine);
                expmsg += Common.GetDataReadable(checkdata.Value.Expected).Trim();
                if(checkdata.Value.Asciiout)
                    expmsg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, GetAsciiString(Common.HexToArray(checkdata.Value.Expected)));
            }
            else {
                Logger.WriteLine2("FAILED! Faulty configuration!");
                return false;
            }
            var tmp = new byte[checkdata.Value.Expected.Length / 2];
            if(checkdata.Value.Offset >= data.Length + tmp.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset/Data length)!");
                return false;
            }
            Buffer.BlockCopy(data, (int) checkdata.Value.Offset, tmp, 0, tmp.Length);
            var msg = Common.GetDataForTest(tmp);
            var isok = msg.Equals(checkdata.Value.Expected, StringComparison.CurrentCultureIgnoreCase);
            if(!isok) {
                if(Common.SwapBytes(ref tmp)) {
                    var swapped = Common.GetDataForTest(tmp);
                    isok = swapped.Equals(checkdata.Value.Expected, StringComparison.CurrentCultureIgnoreCase);
                    if(isok) {
                        reversed = true;
                        datareversed = true;
                    }
                }
            }
            Buffer.BlockCopy(data, (int) checkdata.Value.Offset, tmp, 0, tmp.Length);
            msg = Common.GetDataReadable(tmp).Trim();
            if(datareversed) {
                Common.SwapBytes(ref tmp);
                msg += string.Format("{0}{0}Reversed (checked) data:{0}{1}", Environment.NewLine, Common.GetDataReadable(tmp).Trim());
            }
            if(checkdata.Value.Asciiout)
                msg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, GetAsciiString(tmp));
            AddItem(new Common.PartsObject {
                Name = name.Trim(),
                ActualString = msg.Trim(),
                ExpectedString = expmsg,
                Result = isok
            });
            Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}{1}Actual data: {2}", expmsg, Environment.NewLine, msg));
            return isok;
        }

        private static bool CheckBinPart(ref byte[] data, string name) {
            var datareversed = false;
            var checkdata = Common.Types[data.Length].Bincheck.Value[name];
            if(checkdata.Value.Offset >= data.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset)!");
                return false;
            }
            var expmsg = string.Format("{0}Offset: 0x{2:X}{1}", checkdata.Value.Description, Environment.NewLine, checkdata.Value.Offset);
            var length = 0;
            foreach(var d in checkdata.Value.ExpectedList.Value) {
                var count = 0;
                var tmpmsg = Common.GetDataReadable(d.Expected, ref count).Trim();
                if(length == 0)
                    length = count;
                if(count != length || (length % 2) != 0)
                    expmsg += string.Format("{0}ERROR: Bad length of the following data!:", Environment.NewLine);
                expmsg += string.Format("{0}{1}", tmpmsg.Trim(), Environment.NewLine);
                if(checkdata.Value.Asciiout)
                    expmsg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, Encoding.ASCII.GetString(Common.HexToArray(d.Expected)));
            }
            if(expmsg.Contains("ERROR")) {
                Logger.WriteLine2("FAILED! Faulty configuration!");
                return false;
            }
            var tmp = new byte[length / 2];
            if(checkdata.Value.Offset >= data.Length + tmp.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset/Data length)!");
                return false;
            }
            Buffer.BlockCopy(data, (int) checkdata.Value.Offset, tmp, 0, tmp.Length);
            var msg = Common.GetDataForTest(tmp);
            var isok = false;
            foreach(var d in checkdata.Value.ExpectedList.Value) {
                isok = msg.Equals(d.Expected, StringComparison.CurrentCultureIgnoreCase);
                if(!isok)
                    continue;
                if(d.DisablePatch)
                    _ret.DisablePatch = true;
                break;
            }
            if(!isok) {
                if(tmp.Length == 1) {
                    if((checkdata.Value.Offset % 2) == 0) {
                        if(data.Length < checkdata.Value.Offset + 1) {
                            Logger.WriteLine2("FAILED! Offset is at end of image!");
                            return false;
                        }
                        tmp[0] = data[checkdata.Value.Offset + 1];
                    }
                    else
                        tmp[0] = data[checkdata.Value.Offset - 1];
                    msg = tmp[0].ToString("X2");
                }
                else if(Common.SwapBytes(ref tmp))
                    msg = Common.GetDataForTest(tmp);
                foreach(var d in checkdata.Value.ExpectedList.Value) {
                    isok = msg.Equals(d.Expected, StringComparison.CurrentCultureIgnoreCase);
                    if(!isok)
                        continue;
                    datareversed = true;
                    if(d.DisablePatch)
                        _ret.DisablePatch = true;
                    break;
                }
            }
            Buffer.BlockCopy(data, (int) checkdata.Value.Offset, tmp, 0, tmp.Length);
            msg = Common.GetDataReadable(tmp).Trim();
            if(datareversed) {
                Common.SwapBytes(ref tmp);
                msg += string.Format("{0}{0}Reversed (checked) data:{0}{1}", Environment.NewLine, Common.GetDataReadable(tmp).Trim());
            }
            if(checkdata.Value.Asciiout) {
                var asciidata = Encoding.ASCII.GetString(tmp);
                msg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, asciidata);
            }
            AddItem(new Common.PartsObject {
                Name = name.Trim(),
                ActualString = msg.Trim(),
                ExpectedString = expmsg,
                Result = isok,
            });
            Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}{1}Actual data: {2}", expmsg, Environment.NewLine, msg));
            return isok;
        }

        private static List<SkuCheckData> GetSkuCheckData(bool reversed, ref byte[] data, ref Common.TypeData checkdata) {
            var ret = new List<SkuCheckData>();
            foreach(var skuDataEntry in checkdata.SKUDataList.Value) {
                var skuCheckDataEntry = new SkuCheckData {
                    Type = skuDataEntry.Type
                };
                var tmpdata = new byte[skuDataEntry.Size];
                Buffer.BlockCopy(data, (int) skuDataEntry.Offset, tmpdata, 0, tmpdata.Length);
                if(reversed) {
                    if(skuDataEntry.Size == 1) {
                        if((skuDataEntry.Offset % 2) == 0) {
                            if(data.Length < skuDataEntry.Offset + 1) {
                                Logger.WriteLine2("FAILED! Offset is at end of image!");
                                tmpdata[0] = 0;
                            }
                            else
                                tmpdata[0] = data[skuDataEntry.Offset + 1];
                        }
                        else
                            tmpdata[0] = data[skuDataEntry.Offset - 1];
                    }
                    else
                        Common.SwapBytes(ref tmpdata);
                }
                if(skuDataEntry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase)) {
                    if(tmpdata.Length == 2)
                        skuCheckDataEntry.Size = Common.GetLdrSize(ref tmpdata);
                    else
                        throw new ArgumentException("The bootloader argument size should be 2");
                }
                else
                    skuCheckDataEntry.Data = Common.GetDataForTest(tmpdata);
                ret.Add(skuCheckDataEntry);
            }
            return ret;
        }

        private static List<Common.SKUEntry> GetFilterList(IEnumerable<Common.SKUEntry> skuEntryList, SkuCheckData dataEntry) {
            var ret = new List<Common.SKUEntry>();
            foreach(var skuEntry in skuEntryList) {
                if(!skuEntry.Type.Equals(dataEntry.Type, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                bool isok;
                if(dataEntry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase)) {
                    uint tmpval;
                    if(uint.TryParse(skuEntry.Data, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out tmpval))
                        isok = tmpval == dataEntry.Size;
                    else
                        isok = false;
                }
                else
                    isok = dataEntry.Data.Equals(skuEntry.Data, StringComparison.CurrentCultureIgnoreCase);
                if(isok)
                    ret.Add(skuEntry);
            }
            return ret;
        }

        private static bool CheckDataPart(ref byte[] srcdata, Common.DataCheck checkdata, bool reversed) {
            if(checkdata.Offset >= srcdata.Length || checkdata.LdrSize > srcdata.Length - 2) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset/Ldrsize)!");
                return false;
            }
            long size;
            if(checkdata.LdrSize != 0) {
                var tmpdata = new byte[2];
                Buffer.BlockCopy(srcdata, (int) checkdata.LdrSize, tmpdata, 0, tmpdata.Length);
                if(reversed)
                    Common.SwapBytes(ref tmpdata);
                size = Common.GetLdrSize(ref tmpdata);
            }
            else
                size = checkdata.Size;
            var tmp = new byte[size];
            if(checkdata.Offset >= srcdata.Length - tmp.Length) {
                Logger.WriteLine2(checkdata.LdrSize == 0 ? "FAILED! Faulty configuration (Bad Offset/Data length)!" : "FAILED! (Bad size data)");
                return false;
            }
            Buffer.BlockCopy(srcdata, (int) checkdata.Offset, tmp, 0, tmp.Length);
            var statlist = GetStatistics(ref tmp);
            var isok = CheckStatistics(statlist, checkdata, tmp.Length);
            return isok;
        }

        private static Dictionary<byte, double> GetStatistics(ref byte[] data) {
            var count = new Dictionary<byte, ulong>();
            foreach(var b in data) {
                if(count.ContainsKey(b))
                    count[b]++;
                else
                    count.Add(b, 1);
            }
            var ret = new Dictionary<byte, double>();
            for(var key = 0; key < 256; key++) {
                if(!count.ContainsKey((byte) key))
                    continue;
                ret.Add((byte) key, ((double) count[(byte) key] / data.Length) * 100);
            }
            return ret;
        }

        private static bool CheckStatistics(Dictionary<byte, double> inputList, Common.DataCheck checkdata, int length) {
            var statlist = checkdata.ThresholdList;
            var isok = !(statlist == null || statlist.Count == 0);
            if(isok) {
                foreach(var d in inputList.Keys) {
                    var val = inputList[d];
                    val = double.Parse(val.ToString("F2"));
                    double maxpercentage = 100;
                    if(statlist.ContainsKey(d.ToString("X2")))
                        maxpercentage = statlist[d.ToString("X2")];
                    else if(statlist.ContainsKey("*"))
                        maxpercentage = statlist["*"];
                    if(maxpercentage >= val)
                        continue;
                    Logger.WriteLine(string.Format("Statistics check Failed! 0x{0:X2} doesn't match expected percentage: lower then {1}% Actual value: {2:F2}%", d, maxpercentage, val));
                    isok = false;
                }
                var list = new List<byte>(inputList.Keys);
                list.Sort();
                var actmsg = "";
                foreach(var key in list)
                    actmsg += String.Format("0x{0:X2} : {1:F2}%{2}", key, inputList[key], Environment.NewLine);
                var expmsg = string.Format("Offset checked: 0x{1:X}{0}Length checked: 0x{2:X}", Environment.NewLine, checkdata.Offset, length);
                foreach(var key in checkdata.ThresholdList.Keys) {
                    var val = checkdata.ThresholdList[key];
                    if(!key.Equals("*"))
                        expmsg += string.Format("{0}{1} Should be less then {2:F2}%", Environment.NewLine, key, val);
                    else if(checkdata.ThresholdList.Count > 1)
                        expmsg += string.Format("{0}Everything else should be less then {1:F2}%", Environment.NewLine, val);
                    else
                        expmsg += string.Format("{0}Everything should be less then {1:F2}%", Environment.NewLine, val);
                }
                AddItem(new Common.PartsObject {
                    Name = checkdata.Name.Trim(),
                    ActualString = actmsg.Trim(),
                    ExpectedString = expmsg.Trim(),
                    Result = isok
                });
                Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}{1}Actual data: {2}", expmsg, Environment.NewLine, actmsg));
            }
            else
                Logger.WriteLine2("FAILED! (Bad configuration)");
            return isok;
        }

        private static bool CheckHash(bool reversed, ref byte[] data, HashCheck.HashListObject checkdata) {
            string hash;
            var tmp = Common.Hashes.CheckHash(ref data, checkdata.Offset, checkdata.Size, reversed, checkdata.Type, checkdata.Name, out hash);
            var isok = !string.IsNullOrEmpty(tmp);
            hash = isok ? string.Format("{0}{1}{1}MD5 Hash: {2}", tmp, Environment.NewLine, hash) : string.Format("MD5 Hash: {0}", hash);
            AddItem(new Common.PartsObject {
                Name = checkdata.Name,
                ActualString = hash,
                ExpectedString = "Check the hashlist for more information...",
                Result = isok
            });
            Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}Actual data: {1}", Environment.NewLine, hash));
            return isok;
        }

        private static bool Repetitions(bool reversed, ref byte[] data, ref Common.TypeData checkData) {
            var ret = true;
            var tmp = reversed ? Encoding.BigEndianUnicode.GetString(data) : Encoding.Unicode.GetString(data);
            var bigbuilder = new StringBuilder();
            var checkLines = 0;
            foreach(var key in checkData.RepCheck.Value.Keys) {
                _checkckount++;
                var rep = checkData.RepCheck.Value[key].Value;
                rep.FoundAt.Clear();
                Logger.Write(string.Format("{0,-70} Result: ", string.Format("Repetitions check for {0} Started...", rep.Name)));
                foreach(Match match in Regex.Matches(tmp, Regex.Escape(key))) {
                    var index = match.Index * 2;
                    if(index == rep.Offset)
                        continue;
                    rep.FoundAt.Add(index);
                    checkLines |= (index - rep.Offset) / 2;
                    ret = false;
                }
                if(rep.FoundAt.Count <= 0) {
                    Logger.WriteLine2("OK!");
                    continue;
                }
                Logger.WriteLine2(string.Format("FAILED! {0}Actual data:", Environment.NewLine));
                var builder = new StringBuilder();
                foreach(var offset in rep.FoundAt)
                    builder.Append(string.Format(" 0x{0:X}", offset));
                Logger.WriteLine2(string.Format("{0} Found at {1} offset(s):{2}", rep.Name, rep.FoundAt.Count, builder));
                Logger.WriteLine2(string.Format("{0} Expected at: 0x{1:X}", rep.Name, rep.Offset));
                bigbuilder.AppendLine(string.Format("{0} Found at {1} offsets:{2}", rep.Name, rep.FoundAt.Count, builder));
                bigbuilder.AppendLine(string.Format("{0} Expected at: 0x{1:X}", rep.Name, rep.Offset));
            }
            if(ret)
                bigbuilder.AppendLine("No Repetitions found!");
            else {
                var s = bigbuilder.ToString(); // Save current data
                bigbuilder.Length = 0; // Reset it so we can start fresh
                bigbuilder.Append("You should check address line(s): ");
                for(var i = 0; i < 30; i++) {
                    if((checkLines & (1 << i)) > 0)
                        bigbuilder.AppendFormat("{0} ", (AddressLines) (1 << i));
                }
                bigbuilder.AppendLine(); // Make sure the rest of it ends up on a new line...
                bigbuilder.Append(s); // Add the saved data back
            }
            AddItem(new Common.PartsObject {
                Name = "Repetitions Check",
                ActualString = bigbuilder.ToString(),
                ExpectedString = "No Repetitions are supposed to be listed!",
                Result = ret
            });
            return ret;
        }
        private static string DoCheckDataMatch(ref byte[] data, int offset, ref StringBuilder smallbuilder, ref Dictionary<string, string> testlist, ref bool islastok, Common.DataMatch testdata) {
            var name = testdata.Name;
            if(testdata.SequenceRepetitions > 1)
                name = string.Format(testdata.Name, offset);
            var tmp = Common.GetDataForTest(ref data, offset, testdata.Length);
            if (!testlist.ContainsKey(tmp) && testlist.Count > 0)
                islastok = false;
            if (!testlist.ContainsKey(tmp))
                testlist.Add(tmp, name);
            smallbuilder.AppendLine(!testdata.DisableDisplay ? string.Format("{0} :\r\n{1}", name, Common.GetDataReadable(tmp).Trim()) : string.Format("{0} : Too long to display", name));
            return testdata.DisableDisplay ? "Too long to display" : tmp;
        }

        private static bool CheckDataMatches(ref byte[] data, ref Common.TypeData checkdata) {
            var bigbuilder = new StringBuilder();
            var ret = true;
            foreach(var key in checkdata.DataMatchList.Value.Keys) {
                int cnt = 0, loffset = 0;
                Logger.Write(string.Format("{0,-70} Result: ", string.Format("Datamatch for: {0} Started", checkdata.DataMatchList.Value[key].Value.Name)));
                _checkckount++;
                var smallbuilder = new StringBuilder();
                var islastok = true;
                var testlist = new Dictionary<string, string>();
                var laststring = "";
                bigbuilder.AppendLine(string.Format("Check name: {0}", checkdata.DataMatchList.Value[key].Value.Name));
                foreach(var testdata in checkdata.DataMatchList.Value[key].Value.Data) {
                    if(testdata.SequenceRepetitions <= 0)
                        laststring = DoCheckDataMatch(ref data, testdata.Offset, ref smallbuilder, ref testlist, ref islastok, testdata);
                    else {
                        for(var i = testdata.Offset; i < testdata.Offset + (testdata.Length * testdata.SequenceRepetitions); i += testdata.Length) {
                            laststring = DoCheckDataMatch(ref data, i, ref smallbuilder, ref testlist, ref islastok, testdata);
                            loffset = i;
                            cnt++;
                        }
                    }
                    if(cnt <= 0 || loffset <= 0)
                        continue;
                    Console.WriteLine("Count checked: 0x{0:X}", cnt);
                    Console.WriteLine("End offset: 0x{0:X}", loffset);
                }
                if(!islastok) {
                    ret = false;
                    bigbuilder.Append("Failed!\r\n" + smallbuilder + "\r\n"); // Add to the big one
                    Logger.WriteLine2("Failed!");
                    Logger.WriteLine2(smallbuilder.ToString());
                }
                else {
                    bigbuilder.AppendLine(string.Format("All data matching:\r\n{0}\r\n", Common.GetDataReadable(laststring).Trim()));
                    Logger.WriteLine2("OK!");
                }
            }
            if(ret)
                bigbuilder.Append("All match checks are OK!");
            AddItem(new Common.PartsObject {
                Name = "Data Match Check",
                ActualString = bigbuilder.ToString(),
                ExpectedString = "No Failed matches are supposed to be listed!",
                Result = ret
            });
            return ret;
        }

        private static bool CheckROSVersion(ref byte[] data, int offset, out string rosversion) {
            var rosdata = new byte[0x6FFFE0];
            Buffer.BlockCopy(data, offset, rosdata, 0, rosdata.Length);
            if(_ret.Reversed)
                Common.SwapBytes(ref rosdata);
            rosversion = GetROSVersion(ref rosdata);
            // This shit is stupid!
            //if(_dohash && Common.Hashes != null && Common.Hashes.Offsets.ContainsKey(data.Length) && Common.Hashes.Offsets[data.Length].Value.Count > 0) {
            //    if (_checkdata.ROS0Offset == offset && HashCheck.ROS0Ver != null)
            //        return rosversion == HashCheck.ROS0Ver;
            //    if (_checkdata.ROS1Offset == offset && HashCheck.ROS1Ver != null)
            //        return rosversion == HashCheck.ROS1Ver;
            //}
            return Regex.IsMatch(rosversion, "[0-9]{3}\\.[0-9]{3}");
        }

        private static string GetROSVersion(ref byte[] data) {
            foreach(var e in GetRosEntries(ref data)) {
                if(!e.Filename.Equals("sdk_version", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                return Encoding.ASCII.GetString(data, (int) e.Offset, (int) e.Size).Trim();
            }
            return "N/A";
        }

        private static IEnumerable<ROSEntry> GetRosEntries(ref byte[] data) {
            var entrycount = Common.Swap(BitConverter.ToUInt32(data, 0x4));
            var list = new List<ROSEntry>();
            if (entrycount > 0x100) // There shouldn't be more then like 0x19 entries, but anything greater then 0x100 is WAY off!
                return list;
            for(var i = 0; i < entrycount; i++) {
                var entryoffset = Common.Swap(BitConverter.ToUInt64(data, 0x10 + (i * 0x30)));
                var size = Common.Swap(BitConverter.ToUInt64(data, 0x18 + (i * 0x30)));
                var name = Encoding.ASCII.GetString(data, 0x20 + (i * 0x30), 0x20).Replace("\0", "");
                list.Add(new ROSEntry {
                    Filename = name,
                    Offset = entryoffset,
                    Size = size
                });
            }
            return list;
        }

        #region Nested type: AddressLines

        private enum AddressLines {
            // ReSharper disable UnusedMember.Local
            A0 = 1 << 0,
            A1 = 1 << 1,
            A2 = 1 << 2,
            A3 = 1 << 3,
            A4 = 1 << 4,
            A5 = 1 << 5,
            A6 = 1 << 6,
            A7 = 1 << 7,
            A8 = 1 << 8,
            A9 = 1 << 9,
            A10 = 1 << 10,
            A11 = 1 << 11,
            A12 = 1 << 12,
            A13 = 1 << 13,
            A14 = 1 << 14,
            A15 = 1 << 15,
            A16 = 1 << 16,
            A17 = 1 << 17,
            A18 = 1 << 18,
            A19 = 1 << 19,
            A20 = 1 << 20,
            A21 = 1 << 21,
            A22 = 1 << 22,
            A23 = 1 << 23,
            A24 = 1 << 24,
            A25 = 1 << 25,
            A26 = 1 << 26,
            A27 = 1 << 27,
            A28 = 1 << 28,
            A29 = 1 << 29,
            A30 = 1 << 30
            // ReSharper restore UnusedMember.Local
        }

        #endregion

        #region Nested type: ROSEntry

        private sealed class ROSEntry {
            public string Filename;
            public ulong Offset;
            public ulong Size;
        }

        #endregion

        #region Nested type: SkuCheckData

        private struct SkuCheckData {
            public string Data;
            public uint Size;
            public string Type;
        }

        #endregion
    }
}