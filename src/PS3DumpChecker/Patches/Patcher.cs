namespace PS3DumpChecker.Patches {
    using System;
    using System.IO;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal static class Patcher {
        private static byte[] GetPatchData(string name, bool swap)
        {
            byte[] data;
            if (!File.Exists(name.Substring(name.IndexOf('.') + 1))) // Strip "Patches." and check for the file "patch.bin"
#if EMBEDDED_PATCHES
            {
                BinaryReader reader = null;
                Stream input = null;
                try
                {
                    input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", typeof(Program).Namespace, name));
                    if (input == null)
                        throw new FileNotFoundException(string.Format("[EMBEDDED] {0}", name));
                    reader = new BinaryReader(input);
                    data = reader.ReadBytes((int)input.Length);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (input != null)
                        input.Close();
                }
            }
            else
#else
                throw new FileNotFoundException(name);
#endif
                data = File.ReadAllBytes(name.Substring(name.IndexOf('.') + 1)); // Strip "Patches."
            if (swap)
                Common.SwapBytes(ref data);
            return data;
        }

        private static byte[] GetRosPatchData(string name, bool swap) 
        {
            byte[] data;
            if (!File.Exists(name.Substring(name.IndexOf('.') + 1)) && !Program.GetRegSetting("customrospatch")) // Strip "Patches." and check for the file "patch.bin"
#if EMBEDDED_PATCHES
            {
                BinaryReader reader = null;
                Stream input = null;
                try {
                    input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", typeof(Program).Namespace, name));
                    if(input == null)
                        throw new FileNotFoundException(string.Format("[EMBEDDED] {0}", name));
                    reader = new BinaryReader(input);
                    data = reader.ReadBytes((int) input.Length);
                }
                finally {
                    if(reader != null)
                        reader.Close();
                    if(input != null)
                        input.Close();
                }
            }
            else 
#else
                throw new FileNotFoundException(name);
#endif
            if (Program.GetRegSetting("customrospatch") && !File.Exists(Program.GetRegSettingText("patchBox")))
            {
                throw new FileNotFoundException(Program.GetRegSettingText("patchBox"));
            }
            else if (Program.GetRegSetting("customrospatch") && File.Exists(Program.GetRegSettingText("patchBox")))
            {
                data = File.ReadAllBytes(Program.GetRegSettingText("patchBox")); // Strip "Custom Patches."
            }
            else
                data = File.ReadAllBytes(name.Substring(name.IndexOf('.') + 1)); // Strip "Patches."
            if (swap)
                Common.SwapBytes(ref data);
            return data;
        }

        private static void ApplyPatch(int offset, ref byte[] patchdata, BinaryWriter target) {
            target.Seek(offset, SeekOrigin.Begin);
            target.Write(patchdata);
        }

        private static string GetDestName(string filename) { return string.Format("{0}\\{1}_patched{2}", Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename), Path.GetExtension(filename)); }

        public static void PatchImage(string filename, bool swap) {
            try {
                var destFileName = GetDestName(filename);
                File.Copy(filename, destFileName, true);
                using(var src = new BinaryReader(File.OpenRead(filename))) {
                    using(var target = new BinaryWriter(File.OpenWrite(destFileName))) {
                        var patchdata = GetRosPatchData("Patches.patch.bin", swap);
                        switch(src.BaseStream.Length) {
                            case 0x1000000L:
                                Common.SendStatus("Applying ROS patch 1 of 2");
                                ApplyPatch(0xC0010, ref patchdata, target);
                                Common.SendStatus("Applying ROS patch 2 of 2");
                                ApplyPatch(0x7C0010, ref patchdata, target);

                                if (Program.GetRegSetting("trvkpatches"))
                                {
                                    Common.SendStatus("Applying TRVK patchs");
                                    patchdata = GetPatchData("Patches.nor_rvk.bin", swap);
                                    ApplyPatch(0x40000, ref patchdata, target);
                                }

                                if (Program.GetRegSetting("forcepatch") && Program.GetRegSetting("rosheaders"))
                                {
                                    patchdata = GetPatchData("Patches.ros_head.bin", swap);
                                    Common.SendStatus("Restoring ROS Header 1 of 2");
                                    ApplyPatch(0xC0000, ref patchdata, target);
                                    Common.SendStatus("Restoring ROS Header 2 of 2");
                                    ApplyPatch(0x7C0000, ref patchdata, target);
                                }

                                break;
                            case 0x10000000L:
                                Common.SendStatus("Applying ROS patch 1 of 2");
                                ApplyPatch(0xC0030, ref patchdata, target);
                                Common.SendStatus("Applying ROS patch 2 of 2");
                                ApplyPatch(0x7C0020, ref patchdata, target);

                                if (Program.GetRegSetting("trvkpatches"))
                                {
                                    Common.SendStatus("Applying TRVK patchs");
                                    patchdata = GetPatchData("Patches.nand_rvk.bin", swap);
                                    ApplyPatch(0x91800, ref patchdata, target);
                                }

                                if (Program.GetRegSetting("forcepatch") && Program.GetRegSetting("rosheaders"))
                                {
                                    patchdata = GetPatchData("Patches.ros_head.bin", swap);
                                    Common.SendStatus("Restoring ROS Header 1 of 2");
                                    ApplyPatch(0xC0020, ref patchdata, target);
                                    Common.SendStatus("Restoring ROS Header 2 of 2");
                                    ApplyPatch(0x7C0010, ref patchdata, target);
                                }

                                break;
                            default:
                                throw new NotSupportedException(string.Format("src.BaseStream.Length: 0x{0:X}L", src.BaseStream.Length));
                        }
                    }
                }
                Common.SendStatus(string.Format("All patches applied to {0}", destFileName));
                if (!Program.GetRegSetting("autoexit"))
                    MessageBox.Show(string.Format("All patches applied to {0}", destFileName), "Patching completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(FileNotFoundException ex) {
                MessageBox.Show(string.Format(Resources.PatchFailedCantFindPatchFile, filename, ex.Message), Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}