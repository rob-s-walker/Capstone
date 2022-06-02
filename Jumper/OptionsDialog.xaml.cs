using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jumper
{
    /// <summary>
    /// Interaction logic for OptionsDialog.xaml
    /// </summary>
    public partial class OptionsDialog : Window
    {
        string Path { get; set; }
        bool SubFolder { get; set; }
        int ArchType { get; set; }
        
        public OptionsDialog()
        {
            InitializeComponent();
            AFCombo.ItemsSource = Enum.GetNames(typeof(models.Archiver.Type));
            PathText.Text = Settings.Default.Path;
            AFCombo.SelectedIndex = Settings.Default.ArchiveType;
            SubfolderCheck.IsChecked = Settings.Default.SubFolder;
            Path = Settings.Default.Path;
            ArchType = Settings.Default.ArchiveType;
            SubFolder = Settings.Default.SubFolder;
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Path = fbd.SelectedPath;
                PathText.Text = Path;
            }
            
        }

        private void SubfolderCheck_Checked(object sender, RoutedEventArgs e)
        {
          SubFolder = (bool)SubfolderCheck.IsChecked;
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArchType = AFCombo.SelectedIndex;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Settings.Default.SubFolder = SubFolder;
            Settings.Default.ArchiveType = ArchType;
            Settings.Default.Path = Path;
            Settings.Default.Save();
            Close();
        }
    }
}
