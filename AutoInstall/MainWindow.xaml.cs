using System.Windows;

namespace AutoInstall {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main {
        public Main() {
            this.InitializeComponent();
            this.Start.Content = new Installer();
        }

        private void Install(object sender, RoutedEventArgs e) {
            this.Start.Content = new Installer();
        }

        private void Backup(object sender, RoutedEventArgs e) {
            this.Start.Content = new Backup();
        }
    }
}