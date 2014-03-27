namespace PS3DumpChecker.Forms {
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Forms;

    internal sealed partial class MainForm: Form {
        public MainForm(ICollection<string> args) {
            InitializeComponent();
            var app = Assembly.GetExecutingAssembly();
            Text = string.Format("PS3 Dump Checker v{0}.{1} (Build: {2})", app.GetName().Version.Major, app.GetName().Version.Minor, app.GetName().Version.Build);
            Icon = Program.AppIcon;
        }

        private void MainDragEnter(object sender, DragEventArgs e) { e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None; }
    }
}