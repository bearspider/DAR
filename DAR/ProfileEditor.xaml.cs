using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;

namespace DAR
{
    /// <summary>
    /// Interaction logic for ProfileEditor.xaml
    /// </summary>
    public partial class ProfileEditor : Window
    {
        SpeechSynthesizer voicesynth = new SpeechSynthesizer();
        private String origProfileName;
        public ProfileEditor()
        {
            InitializeComponent();
            InitializeForm();
        }

        public ProfileEditor(CharacterProfile editCharacter)
        {
            InitializeComponent();
            InitializeForm();
            origProfileName = editCharacter.ProfileName;
            textboxVolume.Text = editCharacter.VolumeValue.ToString();
            textboxRate.Text = editCharacter.SpeechRate.ToString();
            textboxCharacter.Text = editCharacter.Name;
            textBoxProfileName.Text = editCharacter.ProfileName;
            textboxLogFile.Text = editCharacter.LogFile;
            checkboxMonitor.IsChecked = editCharacter.MonitorAtStartup;
            //trackBarRate.Value = editCharacter.SpeechRate;
            //trackBarVolume.Value = editCharacter.VolumeValue;
            textboxPhonetic.Text = editCharacter.Name;
            ClrPckerText.SelectedColor = (Color)ColorConverter.ConvertFromString(editCharacter.TextFontColor);
            ClrPckerTimer.SelectedColor = (Color)ColorConverter.ConvertFromString(editCharacter.TimerFontColor);
            ClrPckerBar.SelectedColor = (Color)ColorConverter.ConvertFromString(editCharacter.TimerBarColor);
        }

        private void InitializeForm()
        {
            foreach (System.Speech.Synthesis.InstalledVoice installedVoice in voicesynth.GetInstalledVoices())
            {
                comboVoice.Items.Add(installedVoice.VoiceInfo.Name);
            }
            if (comboVoice.Items.Count > 0)
            {
                comboVoice.SelectedIndex = 0;
            }
        }

        private void ClrPckerText_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ClrPckerTimer_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ClrPckerBar_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ButtonPlayPhonetic_Click(object sender, RoutedEventArgs e)
        {
            voicesynth.Speak(textboxPhonetic.Text);
        }

        private void ButtonPlaySample_Click(object sender, RoutedEventArgs e)
        {
            voicesynth.Speak(textboxSample.Text);
        }

        private void ButtonLogFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Everquest Log Files|eqlog*.txt";
            string filePattern = @"eqlog_(.*)_(.*)\.txt";
            if (fileDialog.ShowDialog() == true)
            {
                textboxLogFile.Text = fileDialog.FileName;
                Regex regexObj = new Regex(filePattern, RegexOptions.IgnoreCase);
                Match fileMatch = regexObj.Match(fileDialog.FileName);
                Group characterGroup = fileMatch.Groups[1];
                Group serverGroup = fileMatch.Groups[2];
                CaptureCollection characterCollection = characterGroup.Captures;
                CaptureCollection serverCollection = serverGroup.Captures;
                textBoxProfileName.Text = characterCollection[0].ToString() + "(" + serverCollection[0].ToString() + ")";
                textboxCharacter.Text = characterCollection[0].ToString();
                textboxPhonetic.Text = characterCollection[0].ToString();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                IEnumerable<CharacterProfile> recordSearch;
                if (textBoxProfileName.Text != origProfileName)
                {
                    recordSearch = col.Find(Query.EQ("ProfileName", origProfileName));
                }
                else
                {
                    recordSearch = col.Find(Query.EQ("ProfileName", textBoxProfileName.Text));
                }
                if (recordSearch.Count<CharacterProfile>() > 0)
                {
                    IEnumerator<CharacterProfile> enumerator = recordSearch.GetEnumerator();
                    enumerator.MoveNext();
                    var character = (enumerator.Current);
                    character.Name = textboxCharacter.Text;
                    character.ProfileName = textBoxProfileName.Text;
                    character.LogFile = textboxLogFile.Text;
                    character.MonitorAtStartup = (Boolean)checkboxMonitor.IsChecked;
                    character.SpeechRate = Convert.ToInt32(sliderRate.Value);
                    character.VolumeValue = Convert.ToInt32(sliderVolume.Value);
                    character.TimerBarColor = ClrPckerBar.SelectedColorText;
                    character.TimerFontColor = ClrPckerTimer.SelectedColorText;
                    character.TextFontColor = ClrPckerText.SelectedColorText;
                    col.Update(character);
                }
                else
                {
                    var player = new CharacterProfile
                    {
                        Name = textboxCharacter.Text,
                        ProfileName = textBoxProfileName.Text,
                        LogFile = textboxLogFile.Text,
                        MonitorAtStartup = (Boolean)checkboxMonitor.IsChecked,
                        SpeechRate = Convert.ToInt32(sliderRate.Value),
                        VolumeValue = Convert.ToInt32(sliderVolume.Value),
                        TimerFontColor = ClrPckerTimer.SelectedColorText,
                        TimerBarColor = ClrPckerBar.SelectedColorText,
                        TextFontColor = ClrPckerText.SelectedColorText
                };
                    col.Insert(player);
                }
            }
            this.Close();
        }

        private void ComboVoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            voicesynth.SelectVoice(comboVoice.SelectedItem.ToString());
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            voicesynth.Volume = Convert.ToInt32(sliderVolume.Value);
        }

        private void SliderRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            voicesynth.Rate = Convert.ToInt32(sliderRate.Value);
        }
    }
}
