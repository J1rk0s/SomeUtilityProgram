using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AutoInstall {
    public partial class Backup : Page {
        private string _SavePath;
        public Backup() {
            this.InitializeComponent();
            this.Drives.ItemsSource = Environment.GetLogicalDrives();
        }

        private void EnterPath_OnGotFocus(object sender, RoutedEventArgs e) {
            this.EnterPath.Text = string.Empty;
        }

        private void OnAddPath(object sender, RoutedEventArgs e) {
            if (this.EnterPath.Text == string.Empty || this.Paths.Items.Contains(this.EnterPath.Text)) {
                return;
            }

            if (Directory.Exists(this.EnterPath.Text)) {
                this.Paths.Items.Add(this.EnterPath.Text);
                return;
            }
            return;
        }

        private void OnDeletePath(object sender, RoutedEventArgs e) {
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
                this.Paths.Items.Add("Specify save location!");
                return;
            }
            DateTime time = DateTime.Today;
            if (this.Paths.Items.IsEmpty) {
                return;
            }
            try {
                Directory.CreateDirectory($@"{this._SavePath}\{time.Day}.{time.Month} {time.Year}\");
                foreach (object pat in this.Paths.Items) {
                    //this.Paths.Items.Add(pat.ToString());
                    await Task.Run(() => this.CopyAll(new DirectoryInfo(pat.ToString()), new DirectoryInfo($@"{this._SavePath}\{time.Day}.{time.Month} {time.Year}\")));
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
            }
        }
        private void CopyAll(DirectoryInfo location1, DirectoryInfo location2) { // From internet
            foreach (FileInfo fi in location1.GetFiles()) {
                fi.CopyTo(Path.Combine(location2.FullName, fi.Name));
            }
            foreach (DirectoryInfo diSourceSubDir in location1.GetDirectories()) {
                DirectoryInfo nextTargetSubDir = location2.CreateSubdirectory(diSourceSubDir.Name);
                this.CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}