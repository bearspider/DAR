using LiteDB;
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
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                foreach(Setting appsetting in settings.FindAll())
                {
                    switch (appsetting.Name)
                    {
                        case "MasterVolume":
                            sliderMaster.Value = Convert.ToInt32(appsetting.Value);
                            break;
                        case "ApplicationUpdate":
                            checkboxUpdate.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "EnableSound":
                            checkboxSoundEnable.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "EnableText":
                            checkboxTextEnable.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "EnableTimers":
                            checkboxTimers.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "Minimize":
                            checkboxMinimize.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "StopTriggerSearch":
                            checkboxStopTrigger.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "DisplayMatchLog":
                            checkboxMatchLog.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "MaxLogEntry":
                            textboxLogEntries.Text = appsetting.Value;
                            break;
                        case "LogMatchesToFile":
                            checkboxLogToFile.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "LogMatchFilename":
                            textboxLogToFile.Text = appsetting.Value;
                            break;
                        case "Clipboard":
                            textboxClipboard.Text = appsetting.Value;
                            break;
                        case "EQFolder":
                            textboxEQFolder.Text = appsetting.Value;
                            break;
                        case "ImportedMediaFolder":
                            textboxImportedMedia.Text = appsetting.Value;
                            break;
                        case "DataFolder":
                            textboxDataFolder.Text = appsetting.Value;
                            break;
                        case "SharingEnabled":
                            checkboxEnableSharing.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "EnableIncomingTriggers":
                            checkboxEnableIncoming.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "AcceptInvitationsFrom":
                            switch(appsetting.Value)
                            {
                                case "0":
                                    radioNobody.IsChecked = true;
                                    break;
                                case "1":
                                    radioTrusted.IsChecked = true;
                                    break;
                                case "2":
                                    radioAnybody.IsChecked = true;
                                    break;
                                default:
                                    radioNobody.IsChecked = true;
                                    break;
                            }
                            break;
                        case "MergeFrom":
                            switch (appsetting.Value)
                            {
                                case "0":
                                    radioNobody.IsChecked = true;
                                    break;
                                case "1":
                                    radioTrusted.IsChecked = true;
                                    break;
                                case "2":
                                    radioAnybody.IsChecked = true;
                                    break;
                                default:
                                    radioNobody.IsChecked = true;
                                    break;
                            }
                            break;
                        case "TrustedSenderList":
                            break;
                        case "LogArchiveFolder":
                            textboxLogArchive.Text = appsetting.Value;
                            break;
                        case "AutoArchive":
                            checkboxAutoArchive.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "CompressArchive":
                            checkboxCompress.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "ArchiveMethod":
                            comboArchiveMethod.SelectedValue = appsetting.Value;
                            break;
                        case "LogSize":
                            textboxLogSize.Text = appsetting.Value;
                            break;
                        case "DeleteArchives":
                            textboxDeleteDays.Text = appsetting.Value;                            
                            break;
                        case "AutoDelete":
                            checkboxDelete.IsEnabled = Convert.ToBoolean(appsetting.Value);
                            break;
                        case "ShareServiceURI":
                            textboxShareURI.Text = appsetting.Value;
                            break;
                        case "Reference":
                            textboxReference.Text = appsetting.Value;
                            break;
                        case "EnableDebug":
                            checkboxDebug.IsChecked = Convert.ToBoolean(appsetting.Value);
                            break;
                    }
                }
            }
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
