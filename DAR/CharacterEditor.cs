using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace DAR
{
    public partial class CharacterEditor : Form
    {
        SpeechSynthesizer voicesynth = new SpeechSynthesizer();
        public CharacterEditor()
        {
            InitializeComponent();
            InitializeForm();
            //CharacterProfile houkaa = new CharacterProfile();
            //houkaa.Speak("Greetings");
            
        }
        private void InitializeForm()
        {
            labelVolumeValue.Text = trackBarVolume.Value.ToString();
            labelRateValue.Text = trackBarRate.Value.ToString();

            foreach (System.Speech.Synthesis.InstalledVoice installedVoice in voicesynth.GetInstalledVoices())
            {
                comboBoxVoice.Items.Add(installedVoice.VoiceInfo.Name);
            }
            if (comboBoxVoice.Items.Count > 0)
            {
                comboBoxVoice.SelectedIndex = 0;
            }

            comboBoxTextFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            comboBoxTimerBar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            comboBoxTimerFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            comboBoxTextFont.DrawItem += new DrawItemEventHandler(ComboboxText_DrawItem);
            comboBoxTimerBar.DrawItem += new DrawItemEventHandler(ComboboxText_DrawItem);
            comboBoxTimerFont.DrawItem += new DrawItemEventHandler(ComboboxText_DrawItem);

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                comboBoxTextFont.Items.Add(c.Name);
                comboBoxTimerBar.Items.Add(c.Name);
                comboBoxTimerFont.Items.Add(c.Name);
            }
            comboBoxTextFont.SelectedItem = "Black";
            comboBoxTimerBar.SelectedItem = "Blue";
            comboBoxTimerFont.SelectedItem = "Lime";
            labelTextColor.ForeColor = Color.FromName(comboBoxTextFont.SelectedItem.ToString());
            labelTimerBarColor.ForeColor = Color.FromName(comboBoxTimerBar.SelectedItem.ToString());
            labelTimerFontColor.ForeColor = Color.FromName(comboBoxTimerFont.SelectedItem.ToString());
        }
        public CharacterEditor(CharacterProfile editCharacter)
        {
            InitializeComponent();
            InitializeForm();
            labelVolumeValue.Text = editCharacter.VolumeValue.ToString();
            labelRateValue.Text = editCharacter.SpeechRate.ToString();
            textBoxCECharacter.Text = editCharacter.Name;
            textBoxCEProfile.Text = editCharacter.ProfileName;
            textBoxCELog.Text = editCharacter.LogFile;
            trackBarRate.Value = editCharacter.SpeechRate;
            trackBarVolume.Value = editCharacter.VolumeValue;
            textBoxPhonetic.Text = editCharacter.Name;
            comboBoxTextFont.SelectedItem = editCharacter.TextFontColor;
            comboBoxTimerFont.SelectedItem = editCharacter.TimerFontColor;
            comboBoxTimerBar.SelectedItem = editCharacter.TimerBarColor;
        }
        private void TrackBarVolume_Scroll(object sender, EventArgs e)
        {
            labelVolumeValue.Text = trackBarVolume.Value.ToString();
            voicesynth.Volume = trackBarVolume.Value;
        }

        private void TrackBarRate_Scroll(object sender, EventArgs e)
        {
            labelRateValue.Text = trackBarRate.Value.ToString();
            voicesynth.Rate = trackBarRate.Value;
        }

        private void ButtonNamePlay_Click(object sender, EventArgs e)
        {
            voicesynth.Speak(textBoxPhonetic.Text);
        }

        private void ButtonSamplePlay_Click(object sender, EventArgs e)
        {
            voicesynth.Speak(textBoxSample.Text);
        }

        private void ComboBoxVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            voicesynth.SelectVoice(comboBoxVoice.SelectedItem.ToString());
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");


                var recordSearch = col.Find(Query.EQ("ProfileName", textBoxCEProfile.Text));
                if (recordSearch.Count<CharacterProfile>() > 0)
                {
                    IEnumerator<CharacterProfile> enumerator = recordSearch.GetEnumerator();
                    enumerator.MoveNext();
                    var character = (enumerator.Current);
                    character.Name = textBoxCECharacter.Text;
                    character.ProfileName = textBoxCEProfile.Text;
                    character.LogFile = textBoxCELog.Text;
                    character.SpeechRate = trackBarRate.Value;
                    character.VolumeValue = trackBarVolume.Value;
                    character.TimerBarColor = comboBoxTimerBar.SelectedItem.ToString();
                    character.TimerFontColor = comboBoxTimerFont.SelectedItem.ToString();
                    character.TextFontColor = comboBoxTextFont.SelectedItem.ToString();
                    col.Update(character);

                }
                else
                {
                    var player = new CharacterProfile
                    {
                        Name = textBoxCECharacter.Text,
                        ProfileName = textBoxCEProfile.Text,
                        LogFile = textBoxCELog.Text,
                        SpeechRate = trackBarRate.Value,
                        VolumeValue = trackBarVolume.Value,
                        TimerFontColor = comboBoxTimerFont.SelectedItem.ToString(),
                        TimerBarColor = comboBoxTimerBar.SelectedItem.ToString(),
                        TextFontColor = comboBoxTimerFont.SelectedItem.ToString()
                    };
                    col.Insert(player);
                }
                col.EnsureIndex(x => x.Name);
            }
            this.Close();
        }
        private void ButtonLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openLogFile = new OpenFileDialog())
            {
                openLogFile.InitialDirectory = "C:\\";
                openLogFile.Filter = "Everquest Log Files|eqlog*.txt";
                openLogFile.RestoreDirectory = true;
                string filePattern = @"eqlog_(.*)_(.*)\.txt";
                
                if (openLogFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxCELog.Text = openLogFile.FileName;
                    Regex regexObj = new Regex(filePattern, RegexOptions.IgnoreCase);
                    Match fileMatch = regexObj.Match(openLogFile.FileName);
                    Group characterGroup = fileMatch.Groups[1];
                    Group serverGroup = fileMatch.Groups[2];
                    CaptureCollection characterCollection = characterGroup.Captures;
                    CaptureCollection serverCollection = serverGroup.Captures;
                    textBoxCEProfile.Text = characterCollection[0].ToString() + "(" + serverCollection[0].ToString() + ")";
                    textBoxCECharacter.Text = characterCollection[0].ToString();
                    textBoxPhonetic.Text = characterCollection[0].ToString();
                }
            }
        }

        private void CharacterEditor_Load(object sender, EventArgs e)
        {

        }

        private void ComboboxText_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if(e.Index >= 0)
            {
                string n = ((ComboBox)sender).Items[e.Index].ToString();
                Font f = new Font("Arial", 9, FontStyle.Regular);
                Color c = Color.FromName(n);
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, Brushes.Black, rect.X, rect.Top);
                g.FillRectangle(b, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
            }
        }

        private void ComboBoxTextFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelTextColor.ForeColor = Color.FromName(comboBoxTextFont.SelectedItem.ToString());
        }

        private void ComboBoxTimerFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelTimerFontColor.ForeColor = Color.FromName(comboBoxTimerFont.SelectedItem.ToString());
        }

        private void ComboBoxTimerBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelTimerBarColor.ForeColor = Color.FromName(comboBoxTimerBar.SelectedItem.ToString());
        }
    }
}
