using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AutoInstall {
    public partial class Backup : Page {
        private string _SavePath;
        private static DateTime time;

        static Backup() => time = DateTime.Now;

        public Backup() {
            this.InitializeComponent();
            this.Drives.ItemsSource = Environment.GetLogicalDrives();
        }

        private void EnterPath_OnGotFocus(object sender, RoutedEventArgs e) {
            this.EnterPath.Text = string.Empty;
        }

        private void OnAddPath(object sender, RoutedEventArgs e) {
            if (this.EnterPath.Text == string.Empty || this.Paths.Items.Contains(this.EnterPath.Text) || !Directory.Exists(this.EnterPath.Text)) {
                MessageBox.Show("The path is invalid or was already added!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Directory.Exists(this.EnterPath.Text)) {
                this.Paths.Items.Add(this.EnterPath.Text);
                return;
            }
            return;
        }

        private void OnDeletePath(object sender, RoutedEventArgs e) {
            if (this.Paths.Items.Count == 0) {
                MessageBox.Show("Specify path to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.Paths.Items.RemoveAt(this.Paths.Items.IndexOf(this.Paths.SelectedItem));
        }
        
        private void OnSaveLocation(object sender, RoutedEventArgs e) {
            using(FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    this._SavePath = dialog.SelectedPath;
                }
            }
        }

        private async void OnBackup(object sender, RoutedEventArgs e) {
            if (this._SavePath == null) {
                MessageBox.Show("Specify save location!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //DateTime time = DateTime.Today;
            if (this.Paths.Items.IsEmpty) {
                MessageBox.Show("Path list is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try {
                Directory.CreateDirectory($@"{this._SavePath}\{time.Day}.{time.Month} {time.Year}\");
                foreach (object pat in this.Paths.Items) {
                    //this.Paths.Items.Add(pat.ToString());
                    string[] s = pat.ToString().Split('\\');
                    try {
                        await Task.Run(() => this.CopyAll(new DirectoryInfo(pat.ToString()), new DirectoryInfo($@"{this._SavePath}\{time.Day}.{time.Month} {time.Year}\")));
                        //await Task.Run(() => this.CopyAll(new DirectoryInfo(pat.ToString()), new DirectoryInfo($@"{this._SavePath}\{time.Day}.{time.Month} {time.Year}\{s[s.Length - 2]}")));
                    }
                    catch { }
                }
            }
            catch (Exception exception) {
                this.Paths.Items.Clear();
                this.Paths.Items.Add("[ERROR] " + exception.Message);
            }
            
        }
        private void OnDefaultPaths(object sender, RoutedEventArgs e) {
            this.Paths.Items.Clear();
            if (this.Drives.SelectedValue != null) {
                if (!Directory.Exists(this.Drives.SelectedValue + "Users")) {
                    MessageBox.Show("Selected drive has no users!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                foreach (string dir in Directory.GetDirectories($@"{this.Drives.SelectedValue}Users\")) {
                    if (!new[] {"Veřejné", "Public", "Default", "All"}.Any(c => dir.Contains(c))) {
                        this.Paths.Items.Add(Directory.Exists(dir + @"\Downloads") ? dir + @"\Downloads\" : string.Empty);
                        this.Paths.Items.Add(Directory.Exists(dir + @"\Documents") ? dir + @"\Documents\" : string.Empty);
                        this.Paths.Items.Add(Directory.Exists(dir + @"\Appdata") ? dir + @"\Appdata\" : string.Empty);
                    }
                }

                while (this.Paths.Items.Contains(string.Empty)) {
                    this.Paths.Items.Remove(string.Empty);
                }
                return;
            }

            MessageBox.Show("Drive not specified!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void CopyAll(DirectoryInfo location1, DirectoryInfo location2) {
            foreach (FileInfo fi in location1.GetFiles()) {
                fi.CopyTo(Path.Combine(location2.FullName, fi.Name));
            }
            foreach (DirectoryInfo diSourceSubDir in location1.GetDirectories()) {
                DirectoryInfo nextTargetSubDir = location2.CreateSubdirectory(diSourceSubDir.Name);
                this.CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        

        private void OnHelp(object sender, RoutedEventArgs e) {
            MessageBox.Show("1) Specify save location\n2) Select drive with users above default paths\n3) Click default paths\n4) Click backup\nOptional: type custom path in enter path and click add",
                "Help menu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}