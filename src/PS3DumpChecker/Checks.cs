namespace PS3DumpChecker {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal static class Checks {
        private static int _checkId;

        private static void AddItem(Common.PartsObject data) {
            Common.AddItem(_checkId, data);
            _checkId++;
        }

        public static Common.ImgInfo StartCheck(string file, ref Stopwatch sw) {
            _checkId = 0;
            var fi = new FileInfo(file);
            var checkckount = 0;
            var ret = new Common.ImgInfo {
                                         FileName = file
                                         };
            var checkdata = Common.Types[fi.Length];
            var data = new byte[fi.Length];

            #region Statistics check

            if(checkdata.Statlist.Value.Count > 0) {
                Logger.WriteLine("Statistics check started...");
                checkckount++;
                if(!CheckStatisticsList(GetStatisticsAndFillData(fi, ref data), data.Length))
                    Common.AddBad(ref ret);
                Common.SendStatus("Statistics check Done!");
            }
            else {
                Common.SendStatus("Skipping Statistics check (nothing to check) Instead: Reading image into memory...");
                data = File.ReadAllBytes(fi.FullName);
                Logger.WriteLine(string.Format("{0,-50} (nothing to check)", "Statistics check skipped!"));
            }

            #endregion Statistics check

            #region Binary check

            if(checkdata.Bincheck.Value.Count > 0) {
                Logger.WriteLine("Binary check Started!");
                foreach(var key in checkdata.Bincheck.Value.Keys) {
                    checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Binary for: {0}", key));
                    var bintmp = string.Format("Binary check for {0} Started...", key);
                    Logger.Write(string.Format("{0,-50} Result: ", bintmp));
                    if(!checkdata.Bincheck.Value[key].Value.IsMulti) {
                        if(!CheckBinPart(ref data, key, ref ret.Reversed))
                            Common.AddBad(ref ret);
                    }
                    else if(!CheckBinPart(ref data, key))
                        Common.AddBad(ref ret);
                    GC.Collect();
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-50} (nothing to check)", "Binary check skipped!"));
            Common.SendStatus("Binary check(s) Done!");

            #endregion Binary check

            #region Data check

            if(checkdata.DataCheckList.Value.Count > 0) {
                Logger.WriteLine("Data check Started!");
                foreach(var key in checkdata.DataCheckList.Value) {
                    checkckount++;
                    Common.SendStatus(string.Format("Parsing Image... Checking Data Statistics for: {0}", key.Name));
                    var datatmp = string.Format("Data Statistics check for {0} Started...", key.Name);
                    Logger.Write(string.Format("{0,-50} Result: ", datatmp));
                    if(!CheckDataPart(ref data, key, ret.Reversed))
                        Common.AddBad(ref ret);
                    GC.Collect();
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-50} (nothing to check)", "Data check skipped!"));
            Common.SendStatus("Data check(s) Done!");

            #endregion Data check

            #region SKU List check

            if(checkdata.SKUList.Value.Count > 0) {
                Logger.WriteLine("SKU List check Started!");
                Common.SendStatus("Checking SKU List...");
                var skuCheckDataList = GetSkuCheckData(ret.Reversed, ref data, ref checkdata);

                var skuEntryList = new List<Common.SKUEntry>(checkdata.SKUList.Value);
                foreach(var entry in skuCheckDataList) {
                    if(skuEntryList.Count < skuCheckDataList.Count)
                        break;
                    var tmplist = GetFilterList(skuEntryList, entry);
                    var tmplist2 = new List<Common.SKUEntry>(skuEntryList);
                    skuEntryList.Clear();
                    foreach(var skuEntry in tmplist2) {
                        foreach(var tmpentry in tmplist) {
                            if(skuEntry.SKUKey == tmpentry.SKUKey)
                                skuEntryList.Add(skuEntry);
                        }
                    }
                }
                var datamsg = "";
                foreach(var entry in skuCheckDataList)
                    datamsg += entry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase) ? string.Format("{0} = {1:X4}{2}", entry.Type, entry.Size, Environment.NewLine) : string.Format("{0} = {1}{2}", entry.Type, entry.Data, Environment.NewLine);
                if(skuEntryList.Count == skuCheckDataList.Count) {
                    ret.SKUModel = skuEntryList[0].Name;
                    ret.MinVer = skuEntryList[0].MinVer;
                    Logger.WriteLine(string.Format("SKU Model: {0}", ret.SKUModel));
                    var msg = "";
                    if(skuEntryList[0].Warn) {
                        foreach(var entry in skuEntryList) {
                            if(string.IsNullOrEmpty(entry.WarnMsg))
                                continue;
                            msg = entry.WarnMsg;
                            break;
                        }
                        MessageBox.Show(msg, Resources.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Logger.WriteLine(msg);
                        datamsg += string.Format("{0}{1}", Environment.NewLine, msg);
                    }
                }
                else {
                    Common.AddBad(ref ret);
                    ret.SKUModel = null;
                    ret.MinVer = null;
                    Logger.WriteLine("No matching SKU model found!");
                    foreach(var entry in skuCheckDataList)
                        Logger.WriteLine(entry.Type.Equals("bootldrsize", StringComparison.CurrentCultureIgnoreCase) ? string.Format("{0} = {1:X4}", entry.Type, entry.Size) : string.Format("{0} = {1}", entry.Type, entry.Data));
                }
                AddItem(new Common.PartsObject {
                                               Name = "SKUIdentity Data", ActualString = datamsg.Trim(), ExpectedString = "", Result = (skuEntryList.Count == skuCheckDataList.Count),
                                               });
            }
            else
                Logger.WriteLine(string.Format("{0,-50} (nothing to check)", "SKU List check skipped!"));

            #endregion SKU List check

            #region Hash check

            if(Common.Hashes.Offsets.ContainsKey(data.Length) && Common.Hashes.Offsets[data.Length].Value.Count > 0) {
                Logger.WriteLine("Hash check Started!");
                foreach(var check in Common.Hashes.Offsets[data.Length].Value) {
                    checkckount++;

                    Common.SendStatus(string.Format("Parsing Image... Checking Hash for: {0}", check.Name));
                    var hashtmp = string.Format("Hash check for {0} Started...", check.Name);
                    Logger.Write(string.Format("{0,-50} Result: ", hashtmp));
                    if(!CheckHash(ret.Reversed, ref data, check))
                        Common.AddBad(ref ret);
                    GC.Collect();
                }
            }
            else
                Logger.WriteLine(string.Format("{0,-50} (nothing to check)", "Hash check skipped!"));
            Common.SendStatus("Hash check(s) Done!");

            #endregion Hash check

            #region Final Output

            Common.SendStatus(string.Format("All checks ({3} Checks) have been completed after {0} Minutes {1} Seconds and {2} Milliseconds", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds, checkckount));
            Logger.WriteLine(string.Format("All checks ({3} Checks) have been completed after {0} Minutes {1} Seconds and {2} Milliseconds", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds, checkckount));
            ret.IsOk = ret.BadCount == 0;
            ret.Status = ret.IsOk ? "Dump has been validated!" : "Dump is bad!";
            if(!ret.IsOk)
                MessageBox.Show(string.Format("ERROR: Your dump failed on {0} of {1} Checks\nPlease check the log for more information!", ret.BadCount, checkckount), Resources.Checks_StartCheck_ERROR___Bad_dump, MessageBoxButtons.OK, MessageBoxIcon.Error);
            var tmp = ret.IsOk ? "Pass!" : "Failed!";
            var outtmp = ret.IsOk ? string.Format("Tests done: {0}", checkckount) : string.Format("Bad count: {0} of {1} Tests", ret.BadCount, checkckount);
            Logger.WriteLine2(string.Format("{0,-50} Check result: {1}", outtmp, tmp));
            sw.Stop();

            #endregion Final Output

            return ret;
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
            GC.Collect();
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
                Logger.WriteLine(string.Format("Statistics check Failed! 0x{0:X2} doesn't match expected percentage: higher then {1}% lower then {2}% Actual value: {3:F2}%", d, low, high, val));
                isok = false;
            }
            var list = new List<byte>(tmp.Keys);
            list.Sort();
            foreach(var key in list)
                msg += String.Format("0x{0:X2} : {1:F2}%{2}", key, tmp[key], Environment.NewLine);
            AddItem(new Common.PartsObject {
                                           Name = "Statistics", ActualString = msg.Trim(), ExpectedString = Common.Types[len].StatDescription.Value, Result = isok
                                           });
            Logger.WriteLine(string.Format("{0,-50} Result: {1}", "Statistics check Completed!", isok ? "OK!" : "FAILED!"));
            return isok;
        }

        private static bool CheckBinPart(ref byte[] data, string name, ref bool reversed) {
            GC.Collect();
            var datareversed = false;
            var checkdata = Common.Types[data.Length].Bincheck.Value[name];
            if(checkdata.Value.Offset >= data.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset)!");
                return false;
            }
            var expmsg = string.Format("{0}{1}Offset: 0x{2:X}{1}", checkdata.Value.Description, Environment.NewLine, checkdata.Value.Offset);
            if(!string.IsNullOrEmpty(checkdata.Value.Expected)) {
                if((checkdata.Value.Expected.Length % 2) != 0) {
                    Logger.WriteLine2("FAILED! Nothing to check! (a.k.a Faulty configuration!)");
                    return false;
                }
                expmsg += string.Format("Expected data:{0}", Environment.NewLine);
                expmsg += Common.GetDataReadable(checkdata.Value.Expected).Trim();
                if(checkdata.Value.Asciiout)
                    expmsg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, Encoding.ASCII.GetString(Common.HexToArray(checkdata.Value.Expected)));
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
            msg = Common.GetDataReadable(tmp);
            if(datareversed) {
                Common.SwapBytes(ref tmp);
                msg += string.Format("{0}Reversed (checked) data:{0}{1}", Environment.NewLine, Common.GetDataReadable(tmp));
            }
            if(checkdata.Value.Asciiout)
                msg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, Encoding.ASCII.GetString(tmp));
            AddItem(new Common.PartsObject {
                                           Name = name.Trim(), ActualString = msg.Trim(), ExpectedString = expmsg, Result = isok
                                           });
            Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}{1}Actual data: {2}", expmsg, Environment.NewLine, msg));
            return isok;
        }

        private static bool CheckBinPart(ref byte[] data, string name) {
            GC.Collect();
            var datareversed = false;
            var checkdata = Common.Types[data.Length].Bincheck.Value[name];
            if(checkdata.Value.Offset >= data.Length) {
                Logger.WriteLine2("FAILED! Faulty configuration (Bad Offset)!");
                return false;
            }
            var expmsg = string.Format("{0}{1}Offset: 0x{2:X}{1}", checkdata.Value.Description, Environment.NewLine, checkdata.Value.Offset);
            var length = 0;
            foreach(var d in checkdata.Value.ExpectedList.Value) {
                var count = 0;
                var tmpmsg = Common.GetDataReadable(d.Expected, ref count);
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
                    break;
                }
            }
            Buffer.BlockCopy(data, (int) checkdata.Value.Offset, tmp, 0, tmp.Length);
            msg = Common.GetDataReadable(tmp);
            if(datareversed) {
                Common.SwapBytes(ref tmp);
                msg += string.Format("{0}Reversed (checked) data:{0}{1}", Environment.NewLine, Common.GetDataReadable(tmp));
            }
            if(checkdata.Value.Asciiout) {
                var asciidata = Encoding.ASCII.GetString(tmp);
                msg += string.Format("{0}Ascii Value: {1}", Environment.NewLine, asciidata);
            }
            AddItem(new Common.PartsObject {
                                           Name = name.Trim(), ActualString = msg.Trim(), ExpectedString = expmsg, Result = isok,
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
            GC.Collect();
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
            GC.Collect();
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
                                               Name = checkdata.Name.Trim(), ActualString = actmsg.Trim(), ExpectedString = expmsg.Trim(), Result = isok
                                               });
                Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}{1}Actual data: {2}", expmsg, Environment.NewLine, actmsg));
            }
            else
                Logger.WriteLine2("FAILED! (Bad configuration)");
            return isok;
        }

        private static bool CheckHash(bool reversed, ref byte[] data, HashCheck.HashListObject checkdata) {
            string hash;
            var tmp = Common.Hashes.CheckHash(ref data, checkdata.Offset, checkdata.Size, reversed, checkdata.Type, out hash);
            var isok = !string.IsNullOrEmpty(tmp);
            hash = isok ? string.Format("{0}{1}{1}MD5 Hash: {2}", tmp, Environment.NewLine, hash) : string.Format("MD5 Hash: {0}", hash);
            AddItem(new Common.PartsObject {
                                           Name = checkdata.Name, ActualString = hash, ExpectedString = "Check the hashlist for more information...", Result = isok
                                           });
            Logger.WriteLine2(isok ? "OK!" : string.Format("FAILED! {0}Actual data: {1}", Environment.NewLine, hash));
            return isok;
        }

        #region Nested type: SkuCheckData

        private struct SkuCheckData {
            public string Data;
            public uint Size;
            public string Type;
        }

        #endregion
    }
}