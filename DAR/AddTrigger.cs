using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAR
{
    public partial class AddTrigger : Form
    {
        private CharacterProfile selectedCharacter;
        private Category selectedCategory;
        private int selectedGroupId;
        private List<SearchText> endEarlyCases = new List<SearchText>();
        private DataTable dt = new DataTable();
        private Audio basicAudioSettings = new Audio();
        private Audio endedEarlyAudioSettings = new Audio();
        private Audio endingEarlyAudioSettings = new Audio();
        public AddTrigger(String selectedGroup)
        {
            InitializeComponent();
            (tabControl1.TabPages[2] as TabPage).Enabled = false;
            (tabControl1.TabPages[3] as TabPage).Enabled = false;
            comboBoxTimerType.SelectedIndex = 0;
            comboBoxTriggered.SelectedIndex = 2;
            DataColumn column = new DataColumn
            {
                DataType = typeof(String),
                ColumnName = "Search Text"
            };
            dt.Columns.Add(column);
            column = new DataColumn
            {
                DataType = typeof(Boolean),
                ColumnName = "Regex"
            };
            dt.Columns.Add(column);
            dataGridViewEndEarly.DataSource = dt;
            dataGridViewEndEarly.Columns[0].Width = 400;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<CharacterProfile> characterProfilesCol = db.GetCollection<CharacterProfile>("profiles");
                LiteCollection<Category> categoriesCol = db.GetCollection<Category>("categories");
                LiteCollection<TriggerGroup> groupsCol = db.GetCollection<TriggerGroup>("triggergroups");
                var getTriggerGroup = groupsCol.FindOne(Query.EQ("TriggerGroupName", selectedGroup));
                selectedGroupId = getTriggerGroup.Id;
                var profiles = characterProfilesCol.FindAll();
                foreach (var profile in profiles)
                {
                    comboBoxBasicTest.Items.Add(profile.ProfileName);
                }
                if (comboBoxBasicTest.Items.Count > 0)
                {
                    comboBoxBasicTest.SelectedIndex = 0;
                    selectedCharacter = profiles.ElementAt<CharacterProfile>(0);
                }

                var categories = categoriesCol.FindAll();
                foreach (var category in categories)
                {
                    comboBoxCategories.Items.Add(category.Name);
                }
                if (comboBoxCategories.Items.Count > 0)
                {
                    comboBoxCategories.SelectedIndex = 0;
                    selectedCategory = categories.ElementAt<Category>(0);
                }
            }
        }
        private int GetDuration(String hours, String minutes, String seconds)
        {
            int totalDuration = 0;
            totalDuration += (Int32.Parse(hours)) * 60 * 60;
            totalDuration += (Int32.Parse(minutes)) * 60;
            totalDuration += (Int32.Parse(seconds));
            return totalDuration;
        }
        private void ButtonTestSound_Click(object sender, EventArgs e)
        {
            selectedCharacter.Speak(textBoxBasicTTS.Text);
        }
        private void RadioButtonTTS_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBasicTTS.Enabled = true;
            labelTextSay.Enabled = true;
            checkBoxInterrupt.Enabled = true;
            textBoxSoundFile.Enabled = false;
            labelSoundFile.Enabled = false;
            buttonSelectSound.Enabled = false;
            if (textBoxBasicTTS.Text.Length > 0)
            {
                buttonTestSound.Enabled = true;
            }
            else
            {
                buttonTestSound.Enabled = false;
            }
        }
        private void RadioButtonNoSound_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBasicTTS.Enabled = false;
            labelTextSay.Enabled = false;
            checkBoxInterrupt.Enabled = false;
            textBoxSoundFile.Enabled = false;
            labelSoundFile.Enabled = false;
            buttonSelectSound.Enabled = false;
            buttonTestSound.Enabled = false;
        }
        private void RadioButtonPlaySound_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBasicTTS.Enabled = false;
            labelTextSay.Enabled = false;
            checkBoxInterrupt.Enabled = false;
            textBoxSoundFile.Enabled = true;
            labelSoundFile.Enabled = true;
            buttonSelectSound.Enabled = true;
            buttonTestSound.Enabled = false;
        }
        private void TextBoxBasicTTS_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBasicTTS.Text.Length > 0)
            {
                buttonTestSound.Enabled = true;
            }
            else
            {
                buttonTestSound.Enabled = false;
            }
        }
        private void CheckBoxDisplayText_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisplayText.Checked == true)
            {
                textBoxBasicDisplay.Enabled = true;
            }
            else
            {
                textBoxBasicDisplay.Enabled = false;
            }
        }
        private void CheckBoxClipboard_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxClipboard.Checked == true)
            {
                textBoxBasicClipboard.Enabled = true;
            }
            else
            {
                textBoxBasicClipboard.Enabled = false;
            }
        }
        private void CheckBoxRegex_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRegex.Checked == true)
            {
                checkBoxFastCheck.Enabled = true;
                checkBoxFastCheck.Checked = true;
            }
            else
            {
                checkBoxFastCheck.Enabled = false;
                checkBoxFastCheck.Checked = false;
            }
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            BsonValue newTriggerId = new BsonValue();
            /* Code to create EndEarlyText Dictionary*/
            foreach(DataRow drow in dt.Rows)
            {
                if (drow.ItemArray[0].ToString() != null)
                {
                    SearchText st = new SearchText
                    {
                        Searchtext = drow.ItemArray[0].ToString(),
                        regexEnabled = (Boolean)(drow.ItemArray[1])
                    };
                    endEarlyCases.Add(st);
                }
            }
            /* Add Code to determine Sound File Id and AudioType Radio Button */
            //Basic Audio Settings
            if(radioButtonNoSound.Checked)
            {
                basicAudioSettings.AudioType = "radioButtonNoSound";
            }
            if(radioButtonPlaySound.Checked)
            {
                basicAudioSettings.AudioType = "radioButtonPlaySound";
                basicAudioSettings.SoundFileId = 0;
            }
            if(radioButtonTTS.Checked)
            {
                basicAudioSettings.AudioType = "radioButtonTTS";
                basicAudioSettings.TTS = textBoxBasicTTS.Text;
                basicAudioSettings.Interrupt = checkBoxInterrupt.Checked;
            }
            //Timer Ending Audio Settings
            if (radioButtonEndingNoSound.Checked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingNoSound";
            }
            if (radioButtonEndingPlaySound.Checked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingPlaySound";
                endingEarlyAudioSettings.SoundFileId = 0;
            }
            if (radioButtonEndingTTS.Checked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingTTS";
                endingEarlyAudioSettings.TTS = textBoxEndingTTS.Text;
                endingEarlyAudioSettings.Interrupt = checkBoxEndingInterrupt.Checked;
            }
            //Timer Ended Audio Settings
            if (radioButtonEndedNoSound.Checked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonEndedNoSound";
            }
            if (radioButtonEndedPlay.Checked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonEndedPlay";
                endedEarlyAudioSettings.SoundFileId = 0;
            }
            if (radioButtonEndedTTS.Checked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonTTS";
                endedEarlyAudioSettings.TTS = textBoxEndingTTS.Text;
                endedEarlyAudioSettings.Interrupt = checkBoxEndingInterrupt.Checked;
            }
            var newTrigger = new Trigger
            {
                Name = textBoxTriggerName.Text,
                SearchText = textBoxSearchText.Text,
                Comments = textBoxComments.Text,
                Regex = checkBoxRegex.Checked,
                Fastcheck = checkBoxFastCheck.Checked,
                Parent = selectedGroupId,
                TriggerCategory = (Category)comboBoxCategories.SelectedItem,
                Displaytext = textBoxBasicDisplay.Text,
                Clipboardtext = textBoxBasicClipboard.Text,
                AudioSettings = basicAudioSettings,
                TimerType = comboBoxTimerType.Text,
                TimerName = textBoxTimerName.Text,
                TimerDuration = GetDuration(textBoxTimerHours.Text, textBoxTimerMinutes.Text, textBoxTimerSeconds.Text),
                TriggeredAgain = comboBoxTriggered.SelectedIndex,
                EndEarlyText = endEarlyCases,
                TimerEndingDuration = GetDuration(textBoxEndingHours.Text, textBoxEndingMinutes.Text, textBoxEndingSeconds.Text),
                TimerEndingDisplayText = textBoxEndingDisplay.Text,
                TimerEndingClipboardText = textBoxEndingClipboard.Text,
                TimerEndingAudio = endingEarlyAudioSettings,
                TimerEnding = checkBoxNotify.Checked,
                TimerEndedClipboardText = textBoxEndedClipboard.Text,
                TimerEndedDisplayText = textBoxEndedDisplay.Text,
                TimerEnded = checkBoxEndedNotify.Checked,
                TimerEndedAudio = endedEarlyAudioSettings,
                ResetCounter = checkBoxUnmatched.Checked,
                ResetCounterDuration = GetDuration(textBoxCounterHours.Text, textBoxCounterMinutes.Text, textBoxCounterSeconds.Text)
            };
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                var existingTrigger = triggers.Find(Query.EQ("Name", newTrigger.Name));
                if(existingTrigger.Count<Trigger>() > 0)
                {
                    MessageBox.Show("Trigger Already Exists");
                    //Update Trigger Instead
                }
                else
                {
                    newTriggerId = triggers.Insert(newTrigger);
                }
            }
            //Add trigger to TriggerGroup db entry
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<TriggerGroup> triggergroups = db.GetCollection<TriggerGroup>("triggergroups");
                var getTriggerGroup = triggergroups.FindById(newTrigger.Parent);
                getTriggerGroup.AddTriggers(newTriggerId.AsInt32);
                triggergroups.Update(getTriggerGroup);
            }
            this.Close();
        }

        private void ComboBoxTimerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxTimerType.SelectedIndex == 0)
            {
                (tabControl1.TabPages[2] as TabPage).Enabled = false;
                (tabControl1.TabPages[3] as TabPage).Enabled = false;
                //Clear not needed fieldsif they were populated
            }
            if (comboBoxTimerType.SelectedIndex == 1)
            {
                (tabControl1.TabPages[2] as TabPage).Enabled = true;
                (tabControl1.TabPages[3] as TabPage).Enabled = true;
            }
            if(comboBoxTimerType.SelectedIndex == 2)
            {
                (tabControl1.TabPages[2] as TabPage).Enabled = false;
                (tabControl1.TabPages[3] as TabPage).Enabled = true;
                //Clear Not needed fields if they were populated
            }
        }
    }
}
