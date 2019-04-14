using LiteDB;
using System;
using System.Collections;
using System.Collections.ObjectModel;
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


namespace HEAP
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private Dictionary<String,Setting> settinglist = new Dictionary<string, Setting>();
        private ObservableCollection<String> sharinglist = new ObservableCollection<string>();

        public Settings()
        {
            InitializeComponent();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                foreach(Setting appsetting in settings.FindAll())
                {
                    settinglist.Add(appsetting.Name,appsetting);
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
                                    radioMergeNobody.IsChecked = true;
                                    break;
                                case "1":
                                    radioMergeTrusted.IsChecked = true;
                                    break;
                                case "2":
                                    radioMergeAnybody.IsChecked = true;
                                    break;
                                default:
                                    radioMergeNobody.IsChecked = true;
                                    break;
                            }
                            break;
                        case "TrustedSenderList":
                            if (appsetting.Value != "")
                            {
                                string[] members = appsetting.Value.Split(',');
                                foreach (string member in members)
                                {
                                    sharinglist.Add(member);
                                }
                            }
                            listboxSenders.ItemsSource = sharinglist;
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
                        case "ArchiveSchedule":
                            if(appsetting.Value != "")
                            {
                                comboArchiveSchedule.Text = appsetting.Value;
                            }
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
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                foreach (KeyValuePair<String,Setting> appsetting in settinglist)
                {
                    if (appsetting.Key == "TrustedSenderList")
                    {
                        appsetting.Value.Value = string.Join(",", sharinglist);
                    }
                    settings.Update(appsetting.Value);
                }
            }
            this.Close();
        }
        private void ButtonEQFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                settinglist["EQFolder"].Value = folderDialog.SelectedPath;
                textboxEQFolder.Text = folderDialog.SelectedPath;
            }
        }
        private void ButtonImportedMedia_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                settinglist["ImportedMediaFolder"].Value = folderDialog.SelectedPath;
                textboxImportedMedia.Text = folderDialog.SelectedPath;
            }

        }
        private void ButtonDataFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                settinglist["DataFolder"].Value = folderDialog.SelectedPath;
                textboxDataFolder.Text = folderDialog.SelectedPath;
            }

        }
        private void ButtonLogToFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                settinglist["LogMatchFilename"].Value = fileDialog.FileName;
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
        private void CheckboxUpdate_Checked(object sender, RoutedEventArgs e)
        {
            if(checkboxUpdate != null && settinglist.Count > 0)
            { settinglist["ApplicationUpdate"].Value = checkboxUpdate.IsChecked.ToString(); }            
        }
        private void CheckboxUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            if(checkboxUpdate != null && settinglist.Count > 0)
            { settinglist["ApplicationUpdate"].Value = checkboxUpdate.IsChecked.ToString(); }            
        }
        private void CheckboxSoundEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxSoundEnable != null && settinglist.Count > 0)
            { settinglist["EnableSound"].Value = checkboxSoundEnable.IsChecked.ToString(); }
        }
        private void CheckboxSoundEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxSoundEnable != null && settinglist.Count > 0)
            { settinglist["EnableSound"].Value = checkboxSoundEnable.IsChecked.ToString(); }
        }
        private void CheckboxTextEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxTextEnable != null && settinglist.Count > 0)
            { settinglist["EnableText"].Value = checkboxTextEnable.IsChecked.ToString(); }
        }
        private void CheckboxTextEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxTextEnable != null && settinglist.Count > 0)
            { settinglist["EnableText"].Value = checkboxTextEnable.IsChecked.ToString(); }
        }
        private void CheckboxMinimize_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxMinimize != null && settinglist.Count > 0)
            { settinglist["Minimize"].Value = checkboxMinimize.IsChecked.ToString(); }
        }
        private void CheckboxMinimize_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxMinimize != null && settinglist.Count > 0)
            { settinglist["Minimize"].Value = checkboxMinimize.IsChecked.ToString(); }
        }
        private void CheckboxMatchLog_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxMatchLog != null && settinglist.Count > 0)
            {
                settinglist["DisplayMatchLog"].Value = checkboxMatchLog.IsChecked.ToString();
                textblockLogEntries.IsEnabled = false;
                textboxLogEntries.IsEnabled = false;
            }
        }
        private void CheckboxMatchLog_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxMatchLog != null && settinglist.Count > 0)
            {
                settinglist["DisplayMatchLog"].Value = checkboxMatchLog.IsChecked.ToString();
                textblockLogEntries.IsEnabled = true;
                textboxLogEntries.IsEnabled = true;
            }
        }
        private void SliderMaster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderMaster != null && settinglist.Count > 0)
            { settinglist["MasterVolume"].Value = sliderMaster.Value.ToString(); }
        }
        private void CheckboxTimers_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxTimers != null && settinglist.Count > 0)
            { settinglist["EnableTimers"].Value = checkboxTimers.IsChecked.ToString(); }
        }
        private void CheckboxTimers_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxTimers != null && settinglist.Count > 0)
            { settinglist["EnableTimers"].Value = checkboxTimers.IsChecked.ToString(); }
        }
        private void CheckboxStopTrigger_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxStopTrigger != null && settinglist.Count > 0)
            { settinglist["StopTriggerSearch"].Value = checkboxStopTrigger.IsChecked.ToString(); }
        }
        private void CheckboxStopTrigger_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxStopTrigger != null && settinglist.Count > 0)
            { settinglist["StopTriggerSearch"].Value = checkboxStopTrigger.IsChecked.ToString(); }
        }
        private void TextboxLogEntries_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxLogEntries != null && settinglist.Count > 0)
            { settinglist["MaxLogEntry"].Value = textboxLogEntries.Text; }
        }
        private void TextboxClipboard_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxClipboard != null && settinglist.Count > 0)
            { settinglist["Clipboard"].Value = textboxClipboard.Text; }
        }
        private void CheckboxEnableSharing_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxEnableSharing != null && settinglist.Count > 0)
            {
                settinglist["SharingEnabled"].Value = checkboxEnableSharing.IsChecked.ToString();
                groupboxMergeFrom.IsEnabled = false;
                groupboxSenderList.IsEnabled = false;
                groupboxShareFrom.IsEnabled = false;
            }
        }
        private void CheckboxEnableSharing_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxEnableSharing != null && settinglist.Count > 0)
            {
                settinglist["SharingEnabled"].Value = checkboxEnableSharing.IsChecked.ToString();
                groupboxMergeFrom.IsEnabled = true;
                groupboxSenderList.IsEnabled = true;
                groupboxShareFrom.IsEnabled = true;
            }
        }
        private void CheckboxEnableIncoming_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxEnableIncoming != null && settinglist.Count > 0)
            { settinglist["EnableIncomingTriggers"].Value = checkboxEnableIncoming.IsChecked.ToString(); }
        }
        private void CheckboxEnableIncoming_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxEnableIncoming != null && settinglist.Count > 0)
            { settinglist["EnableIncomingTriggers"].Value = checkboxEnableIncoming.IsChecked.ToString(); }
        }
        private void RadioNobody_Checked(object sender, RoutedEventArgs e)
        {
            if(radioNobody != null && settinglist.Count > 0)
            { settinglist["AcceptInvitationsFrom"].Value = "0"; }
        }
        private void RadioTrusted_Checked(object sender, RoutedEventArgs e)
        {
            if (radioTrusted != null && settinglist.Count > 1)
            { settinglist["AcceptInvitationsFrom"].Value = "1"; }
        }
        private void RadioAnybody_Checked(object sender, RoutedEventArgs e)
        {
            if (radioAnybody != null && settinglist.Count > 0)
            { settinglist["AcceptInvitationsFrom"].Value = "2"; }
        }

        private void RadioMergeNobody_Checked(object sender, RoutedEventArgs e)
        {
            if(radioMergeNobody != null && settinglist.Count > 0)
            { settinglist["MergeFrom"].Value = "0"; }
        }
        private void RadioMergeTrusted_Checked(object sender, RoutedEventArgs e)
        {
            if (radioMergeTrusted != null && settinglist.Count > 0)
            { settinglist["MergeFrom"].Value = "1"; }
        }
        private void RadioMergeAnybody_Checked(object sender, RoutedEventArgs e)
        {
            if (radioMergeAnybody != null && settinglist.Count > 0)
            { settinglist["MergeFrom"].Value = "2"; }
        }
        private void CheckboxAutoArchive_Checked(object sender, RoutedEventArgs e)
        {
            if(checkboxAutoArchive != null && settinglist.Count > 0)
            {
                settinglist["AutoArchive"].Value = checkboxAutoArchive.IsChecked.ToString();
                checkboxDelete.IsEnabled = true;
                checkboxCompress.IsEnabled = true;
                textblockArchiveSchedule.IsEnabled = true;
                comboArchiveSchedule.IsEnabled = true;
                textboxDeleteDays.IsEnabled = true;
                textblockDeleteDays.IsEnabled = true;
            }
        }
        private void CheckboxAutoArchive_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxAutoArchive != null && settinglist.Count > 0)
            {
                settinglist["AutoArchive"].Value = checkboxAutoArchive.IsChecked.ToString();
                checkboxDelete.IsEnabled = false;
                checkboxCompress.IsEnabled = false;
                checkboxDelete.IsChecked = false;
                checkboxCompress.IsChecked = false;
                textblockArchiveSchedule.IsEnabled = false;
                comboArchiveSchedule.IsEnabled = false;
                textboxDeleteDays.IsEnabled = false;
                textblockDeleteDays.IsEnabled = false;
            }
        }
        private void CheckboxCompress_Unchecked(object sender, RoutedEventArgs e)
        {
            if(checkboxCompress != null && settinglist.Count > 0)
            { settinglist["CompressArchive"].Value = checkboxCompress.IsChecked.ToString(); }
        }
        private void CheckboxCompress_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxCompress != null && settinglist.Count > 0)
            { settinglist["CompressArchive"].Value = checkboxCompress.IsChecked.ToString(); }
        }
        private void CheckboxDelete_Checked(object sender, RoutedEventArgs e)
        {
            if(checkboxDelete != null && settinglist.Count > 0)
            { settinglist["AutoDelete"].Value = checkboxDelete.IsChecked.ToString(); }
        }
        private void CheckboxDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            if (checkboxDelete != null && settinglist.Count > 0)
            {
                textboxDeleteDays.Text = "";
                settinglist["AutoDelete"].Value = checkboxDelete.IsChecked.ToString();
            }
        }
        private void TextboxDeleteDays_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textboxDeleteDays != null && settinglist.Count > 0)
            { settinglist["DeleteArchives"].Value = textboxDeleteDays.Text; }
        }
        private void TextboxShareURI_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textboxShareURI != null && settinglist.Count > 0)
            { settinglist["ShareServiceURI"].Value = textboxShareURI.Text; }
        }
        private void TextboxReference_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textboxReference != null && settinglist.Count > 0)
            { settinglist["Reference"].Value = textboxReference.Text; }
        }
        private void CheckboxDebug_Unchecked(object sender, RoutedEventArgs e)
        {
            if(checkboxDebug != null && settinglist.Count > 0)
            { settinglist["EnableDebug"].Value = checkboxDebug.IsChecked.ToString(); }
        }
        private void CheckboxDebug_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxDebug != null && settinglist.Count > 0)
            { settinglist["EnableDebug"].Value = checkboxDebug.IsChecked.ToString(); }
        }

        private void ComboArchiveSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboArchiveSchedule != null && settinglist.Count > 0)
            {
                if (comboArchiveSchedule.Text != null)
                { settinglist["ArchiveSchedule"].Value = (e.AddedItems[0] as ComboBoxItem).Content as string; }
            }
        }
        private void ButtonAddSender_Click(object sender, RoutedEventArgs e)
        {
            Boolean duplicate = false;
            foreach(String member in sharinglist)
            {
                if(member.ToUpper() == textboxSenderList.Text.ToUpper())
                { duplicate = true; }
            }
            if(!duplicate)
            { sharinglist.Add(textboxSenderList.Text); }
            textboxSenderList.Text = "";
        }
        private void ButtonRemoveSender_Click(object sender, RoutedEventArgs e)
        {
            sharinglist.Remove(listboxSenders.SelectedValue.ToString());
        }
    }
}
