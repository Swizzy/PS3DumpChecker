namespace PS3DumpChecker {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;
    using PS3DumpChecker.Properties;

    internal sealed partial class UpdateForm : Form {
        internal UpdateForm() {
            InitializeComponent();
            Icon = Program.AppIcon;
        }

        private void UpdateFormLoad(object sender, EventArgs e) {
            statuslbl.Text = Resources.downloadchangelog;
            changelogbw.RunWorkerAsync();
        }

        private static string GetFinalUrl(string url) {
            var location = url;
            do {
                url = location;
                var req = WebRequest.Create(url) as HttpWebRequest;
                if(req == null)
                    break;
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;
                try {
                    using(var response = req.GetResponse() as HttpWebResponse) {
                        if(response != null)
                            location = response.GetResponseHeader("Location");
                    }
                }
                catch(WebException) {
                    return url; //Not getting any further anyways...
                }
            }
            while(location != url && !string.IsNullOrEmpty(location));
            return url;
        }

        private void BWDoWork(object sender, DoWorkEventArgs e) {
            try {
                var url = string.Format("{0}/changelog", GetFinalUrl("http://ps3dumpchecker.xeupd.com"));
                var wc = new WebClient();
                e.Result = wc.DownloadString(url);
                if(string.IsNullOrEmpty(e.Result as string))
                    e.Result = "Oh noes! There is no changelog for you :'(";
            }
            catch(WebException ex) {
                var code = ((HttpWebResponse) ex.Response).StatusCode;
                if(code == HttpStatusCode.NoContent || code == HttpStatusCode.NotFound)
                    e.Result = "Oh noes! There is no changelog for you :'(";
                else
                    e.Result = string.Format("Oh noes! You've found a bug!! Please copy 'n' paste the error below and send to me... {0}{0}{0}{1}", Environment.NewLine, ex);
            }
        }

        private void SetBtns(bool state) {
            appbtn.Enabled = state;
            cfgbtn.Enabled = state;
            hashlistbtn.Enabled = state;
        }

        private void BWRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            statuslbl.Text = Resources.changelogdone;
            changelog.Text = e.Result as string;
            SetBtns(true);
        }

        private void DwlCompleted() {
            statuslbl.Text = Resources.downloadcompleted;
            SetBtns(true);
        }

        private void DownloadFile(string file) {
            SetBtns(false);
            dwlbw = new BackgroundWorker();
            switch(file) {
                case "latest.cfg":
                    dwlbw.RunWorkerCompleted += CfgDwlCompleted;
                    break;
                case "latest.hashlist":
                    dwlbw.RunWorkerCompleted += HashListDwlCompleted;
                    break;
                case "latest.exe":
                    dwlbw.RunWorkerCompleted += ExeDwlCompleted;
                    break;
                default:
                    dwlbw.RunWorkerCompleted += (sender, args) => DwlCompleted();
                    break;
            }
            dwlbw.DoWork += DwlDoWork;
            dwlbw.RunWorkerAsync(file);
        }

        private void CfgDwlCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(CheckHash("latest.cfg")) {
                Program.MainForm.ParseConfig("latest.cfg");
                MoveFile("latest.cfg", "default.cfg");
            }
            else
                MessageBox.Show(Resources.baddwl);
            DwlCompleted();
        }

        private void HashListDwlCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) {
            if(CheckHash("latest.hashlist")) {
                Common.Hashes = new HashCheck("latest.hashlist");
                MoveFile("latest.hashlist", "default.hashlist");
            }
            else
                MessageBox.Show(Resources.baddwl);
            DwlCompleted();
        }

        private void ExeDwlCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) {
            if (CheckHash("latest.exe")) {
                var fi = new FileInfo(Path.GetTempPath() + "UpdateHelper.exe");
                Program.ExtractResource(fi, "PS3DumpChecker.UpdateHelper.exe");
                var dir = Path.GetDirectoryName(Application.ExecutablePath);
                if (string.IsNullOrEmpty(dir))
                    throw new InvalidOperationException();
                var cproc = Process.GetCurrentProcess();
                var proc = new Process {
                                       StartInfo = {
                                           WorkingDirectory = dir,
                                           UseShellExecute = false,
                                           CreateNoWindow = true,
                                           FileName = fi.FullName,
                                           Arguments = string.Format("\"{0}\" \"latest.exe\" \"{1}\"", cproc.Id, cproc.MainModule.FileName)
                                                   }
                                       };
                proc.Start();
                return;
            }
            MessageBox.Show(Resources.baddwl);
            DwlCompleted();
        }

        private static void DwlDoWork(object sender, DoWorkEventArgs e) {
            try {
                var file = e.Argument as string;
                if(file == null)
                    throw new ArgumentNullException(Resources.NoFile, new Exception(Resources.NoFile));
                var url = string.Format("{0}/{1}", GetFinalUrl("http://ps3dumpchecker.xeupd.com"), file);
                var wc = new WebClient();
                wc.DownloadFile(url, file);
            }
            catch(WebException ex) {
                MessageBox.Show(((HttpWebResponse) ex.Response).StatusDescription, Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void MoveFile(string src, string target) {
            File.Delete(target);
            File.Move(src, target);
        }

        private static bool CheckHash(string file) {
            FileStream tmp = null;
            try {
                var wc = new WebClient();
                var url = string.Format("{0}/hash.php?file={1}", GetFinalUrl("http://ps3dumpchecker.xeupd.com").TrimEnd(new[] {
                                                                                                                              '/'
                                                                                                                              }), file);
                var hash = Encoding.UTF8.GetString(wc.DownloadData(url));
                if(string.IsNullOrEmpty(hash))
                    return false;
                tmp = new FileStream(file, FileMode.Open);
                var real = "";
                foreach(var b in MD5.Create().ComputeHash(tmp))
                    real += b.ToString("X2");
                return real.Equals(hash, StringComparison.CurrentCultureIgnoreCase);
            }
            catch(WebException ex) {
                MessageBox.Show(((HttpWebResponse) ex.Response).StatusDescription, Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally {
                if(tmp != null)
                    tmp.Close();
            }
        }

        private void CfgbtnClick(object sender, EventArgs e) {
            statuslbl.Text = Resources.dlinglatestcfg;
            DownloadFile("latest.cfg");
        }

        private void HashlistbtnClick(object sender, EventArgs e) {
            statuslbl.Text = Resources.dlinghashlist;
            DownloadFile("latest.hashlist");
        }

        private void AppbtnClick(object sender, EventArgs e) {
            statuslbl.Text = Resources.dlingapp;
            DownloadFile("latest.exe");
        }
    }
}