using System.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace AutoInstall {
    public partial class Installer : Page {
        public Installer() {
            this.InitializeComponent();
            this.clearList.Visibility = Visibility.Collapsed;
        }
        private void OnDialogBTN(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "Select exes to install",
                CheckFileExists = true,
                Filter = "executables|*.exe|installers|*.msi",
                ShowReadOnly = true,
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) {
                if (dialog.FileNames != null) {
                    this.clearList.Visibility = Visibility.Visible;
                }
                foreach (string file in dialog.FileNames) {
                    this.allExes.Items.Add(file);
                }
            }
        }

        private void OnRunInstall(object sender, RoutedEventArgs e) {
            if (this.allExes.Items.Count == 0) {
                MessageBox.Show("Add .exe or .msi first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try {
                Process process = new Process();
                foreach (object exe in this.allExes.Items) {
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = exe.ToString();
                    process.StartInfo.Arguments = "/Q /QB /quiet /q /qb";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                }
            }
            catch (Exception ex) {
                this.allExes.Items.Clear();
                this.allExes.Items.Add("[ERROR] Error occured!");
                this.allExes.Items.Add($"[ERROR] {ex.Message}");
            }
        }

        private void OnClearList(object sender, RoutedEventArgs e) {
            this.allExes.Items.Clear();
            this.clearList.Visibility = Visibility.Collapsed;
        }
    }
}