using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DAR
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonEQFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                textboxEQFolder.Text = folderDialog.SelectedPath;
            }
        }

        private void ButtonImportedMedia_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textboxImportedMedia.Text = folderDialog.SelectedPath;
            }

        }

        private void ButtonDataFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textboxDataFolder.Text = folderDialog.SelectedPath;
            }

        }

        private void ButtonLogToFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                textboxLogToFile.Text = fileDialog.FileName;
            }

        }

        private void CheckLogToFile_Checked(object sender, RoutedEventArgs e)
        {
            textboxLogToFile.IsEnabled = true;
            buttonLogToFile.IsEnabled = true;
        }

        private void CheckLogToFile_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxLogToFile.Clear();
            textboxLogToFile.IsEnabled = false;
            buttonLogToFile.IsEnabled = false;
        }

        private void ButtonLogArchive_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textboxLogArchive.Text = folderDialog.SelectedPath;
            }

        }
    }
}
