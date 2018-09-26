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
        public AddTrigger(String selectedGroup)
        {
            InitializeComponent();
            (tabControl1.TabPages[2] as TabPage).Enabled = false;
            (tabControl1.TabPages[3] as TabPage).Enabled = false;
            comboBoxTimerType.SelectedIndex = 1;
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
            var newTrigger = new Trigger
            {
                Name = textBoxTriggerName.Text,
                SearchText = textBoxSearchText.Text,
                Comments = textBoxComments.Text,
                Regex = checkBoxRegex.Checked,
                Fastcheck = checkBoxFastCheck.Checked,
                Parent = selectedGroupId,
                //TriggerCategory = comboBoxCategories.SelectedItem.ToString(),
                Displaytext = textBoxBasicDisplay.Text,
                Clipboardtext = textBoxBasicClipboard.Text,
                //Audio = new Hashtable(),
                TimerType = comboBoxTimerType.Text,
                TimerName = textBoxTimerName.Text,
                TimerDuration = GetDuration(textBoxTimerHours.Text, textBoxTimerMinutes.Text, textBoxTimerSeconds.Text),
                //TriggeredAgain = comboBoxTriggered.SelectedItem.ToString(),
                //EndEarlyText = new ArrayList(),
                TimerEndingDuration = GetDuration(textBoxEndingHours.Text, textBoxEndingMinutes.Text, textBoxEndingSeconds.Text),
                TimerEndingDisplayText = textBoxEndingDisplay.Text,
                TimerEndingClipboardText = textBoxEndingClipboard.Text,
                //TimerEndingAudio = new Hashtable(),
                TimerEnding = checkBoxNotify.Checked,
                TimerEndedClipboardText = textBoxEndedClipboard.Text,
                TimerEndedDisplayText = textBoxEndedDisplay.Text,
                TimerEnded = checkBoxEndedNotify.Checked,
                //TimerEndedAudio = new Hashtable(),
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
    }
}
