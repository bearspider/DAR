using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace DAR
{
    /// <summary>
    /// Interaction logic for TriggerEditor.xaml
    /// </summary>
    ///     {

    public partial class TriggerEditor : Window
    {
        private CharacterProfile selectedCharacter;
        private Category selectedCategory;
        private int selectedGroupId;
        private List<SearchText> endEarlyCases = new List<SearchText>();
        private DataTable dt = new DataTable();
        private Audio basicAudioSettings = new Audio();
        private Audio endedEarlyAudioSettings = new Audio();
        private Audio endingEarlyAudioSettings = new Audio();
        public TriggerEditor()
        {
            InitializeComponent();
        }
        public TriggerEditor(String selectedGroup)
        {
            InitializeComponent();
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
            //datagridEarly.ItemsSource = dt;
            //datagridEarly.Columns[0].Width = 400;
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
                    comboBasicTest.Items.Add(profile.ProfileName);
                }
                if (comboBasicTest.Items.Count > 0)
                {
                    comboBasicTest.SelectedIndex = 0;
                    selectedCharacter = profiles.ElementAt<CharacterProfile>(0);
                }

                var categories = categoriesCol.FindAll();
                foreach (var category in categories)
                {
                    comboCategory.Items.Add(category.Name);
                }
                if (comboCategory.Items.Count > 0)
                {
                    comboCategory.SelectedIndex = 0;
                    selectedCategory = categories.ElementAt<Category>(0);
                }
            }
        }
        public TriggerEditor(int selectedTrigger)
        {
            InitializeComponent();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                Trigger trigger = triggers.FindById(selectedTrigger);
                textboxName.Text = trigger.Name;
                textboxSearch.Text = trigger.SearchText;
                textboxComments.Text = trigger.Comments;
                checkboxRegex.IsChecked = trigger.Regex;
                checkboxFast.IsChecked = trigger.Fastcheck;
                selectedGroupId = trigger.Parent;
                //(Category)comboBoxCategories.SelectedItem = trigger.TriggerCategory;
                textboxBasicDisplay.Text = trigger.Displaytext;
                textboxBasicClipboard.Text = trigger.Clipboardtext;
                //basicAudioSettings = trigger.AudioSettings;
                comboTimerType.Text = trigger.TimerType;
                textboxTimerName.Text = trigger.TimerName;
                //trigger.TimerDuration = GetDuration(textBoxTimerHours.Text, textBoxTimerMinutes.Text, textBoxTimerSeconds.Text);
                //comboBoxTriggered.SelectedIndex = trigger.TriggeredAgain;
                endEarlyCases = trigger.EndEarlyText;
                //trigger.TimerEndingDuration = GetDuration(textBoxEndingHours.Text, textBoxEndingMinutes.Text, textBoxEndingSeconds.Text);
                textboxEndingDisplay.Text = trigger.TimerEndingDisplayText;
                textboxEndingClipboard.Text = trigger.TimerEndingClipboardText;
                endingEarlyAudioSettings = trigger.TimerEndingAudio;
                checkboxEndingNotify.IsChecked = trigger.TimerEnding;
                textboxEndedClipboard.Text = trigger.TimerEndedClipboardText;
                textboxEndedDisplay.Text = trigger.TimerEndedDisplayText;
                checkboxEndedNotify.IsChecked = trigger.TimerEnded;
                //endedEarlyAudioSettings = trigger.TimerEndedAudio;
                checkboxCounterNotify.IsChecked = trigger.ResetCounter;
                //trigger.ResetCounterDuration = GetDuration(textBoxCounterHours.Text, textBoxCounterMinutes.Text, textBoxCounterSeconds.Text)

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
        private void ButtonBasicTest_Click(object sender, RoutedEventArgs e)
        {
            selectedCharacter.Speak(textboxBasicTTS.Text);
        }
        private void RadioBasicTTS_Checked(object sender, RoutedEventArgs e)
        {
            textboxBasicTTS.IsEnabled = true;
            labelBasicTTS.IsEnabled = true;
            checkboxBasicInterrupt.IsEnabled = true;
            textboxBasicSoundFile.IsEnabled = false;
            labelBasicSoundFile.IsEnabled = false;
            buttonBasicSoundFile.IsEnabled = false;
            radioBasicNoSound.IsChecked = false;
            radioBasicPlay.IsChecked = false;
            if (textboxBasicTTS.Text.Length > 0)
            {
                buttonBasicTest.IsEnabled = true;
            }
            else
            {
                buttonBasicTest.IsEnabled = false;
            }
        }
        private void RadioBasicPlay_Checked(object sender, RoutedEventArgs e)
        {
            textboxBasicTTS.IsEnabled = false;
            labelBasicTTS.IsEnabled = false;
            checkboxBasicInterrupt.IsEnabled = false;
            textboxBasicSoundFile.IsEnabled = true;
            labelBasicSoundFile.IsEnabled = true;
            buttonBasicSoundFile.IsEnabled = true;
            buttonBasicTest.IsEnabled = false;
            radioBasicNoSound.IsChecked = false;
            radioBasicTTS.IsChecked = false;
        }
        private void RadioBasicNoSound_Checked(object sender, RoutedEventArgs e)
        {
            if (textboxBasicTTS != null)
            {
                textboxBasicTTS.IsEnabled = false;
            }
            if(labelBasicTTS != null)
            {
                labelBasicTTS.IsEnabled = false;
            }
            if(checkboxBasicInterrupt != null)
            {
                checkboxBasicInterrupt.IsEnabled = false;
            }
            if(textboxBasicSoundFile != null)
            {
                textboxBasicSoundFile.IsEnabled = false;
            }
            if(labelBasicSoundFile != null)
            {
                labelBasicSoundFile.IsEnabled = false;
            }
            if(buttonBasicSoundFile != null)
            {
                buttonBasicSoundFile.IsEnabled = false;
            }
            if(buttonBasicTest != null)
            {
                buttonBasicTest.IsEnabled = false;
            }
            if(radioBasicPlay != null)
            {
                radioBasicPlay.IsChecked = false;
            }
            if(radioBasicTTS != null)
            {
                radioBasicTTS.IsChecked = false;
            }
        }
        private void TextboxBasicTTS_TextChanged(object sender, RoutedEventArgs e)
        {
            if (textboxBasicTTS.Text.Length > 0)
            {
                buttonBasicTest.IsEnabled = true;
            }
            else
            {
                buttonBasicTest.IsEnabled = false;
            }
        }
        private void CheckboxBasicDisplay_Checked(object sender, RoutedEventArgs e)
        {
            textboxBasicDisplay.IsEnabled = true;
        }
        private void CheckboxBasicClipboard_Checked(object sender, RoutedEventArgs e)
        {
            textboxBasicClipboard.IsEnabled = true;                
        }
        private void CheckboxRegex_Checked(object sender, RoutedEventArgs e)
        {
            if (checkboxRegex.IsChecked == true)
            {
                checkboxFast.IsEnabled = true;
                checkboxFast.IsChecked = true;
            }
            else
            {
                checkboxFast.IsEnabled = false;
                checkboxFast.IsChecked = false;
            }
        }
        private void ButtonTimerSave_Click(object sender, RoutedEventArgs e)
        {
            BsonValue newTriggerId = new BsonValue();
            /* Code to create EndEarlyText Dictionary*/
            foreach (DataRow drow in dt.Rows)
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
            if ((Boolean)radioBasicNoSound.IsChecked)
            {
                basicAudioSettings.AudioType = "radioButtonNoSound";
            }
            if ((Boolean)radioBasicPlay.IsChecked)
            {
                basicAudioSettings.AudioType = "radioButtonPlaySound";
                basicAudioSettings.SoundFileId = 0;
            }
            if ((Boolean)radioBasicTTS.IsChecked)
            {
                basicAudioSettings.AudioType = "radioButtonTTS";
                basicAudioSettings.TTS = textboxBasicTTS.Text;
                basicAudioSettings.Interrupt = (Boolean)checkboxBasicInterrupt.IsChecked;
            }
            //Timer Ending Audio Settings
            if ((Boolean)radioEndingNoSound.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingNoSound";
            }
            if ((Boolean)radioEndingPlay.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingPlaySound";
                endingEarlyAudioSettings.SoundFileId = 0;
            }
            if ((Boolean)radioEndingTTS.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "radioButtonEndingTTS";
                endingEarlyAudioSettings.TTS = textboxEndingTTS.Text;
                endingEarlyAudioSettings.Interrupt = (Boolean)checkboxEndingInterrupt.IsChecked;
            }
            //Timer Ended Audio Settings
            if ((Boolean)radioEndedNoSound.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonEndedNoSound";
            }
            if ((Boolean)radioEndedPlay.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonEndedPlay";
                endedEarlyAudioSettings.SoundFileId = 0;
            }
            if ((Boolean)radioEndedTTS.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "radioButtonTTS";
                endedEarlyAudioSettings.TTS = textboxEndingTTS.Text;
                endedEarlyAudioSettings.Interrupt = (Boolean)checkboxEndingInterrupt.IsChecked;
            }

            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                LiteCollection<TriggerGroup> triggergroups = db.GetCollection<TriggerGroup>("triggergroups");
                LiteCollection<CharacterProfile> profiles = db.GetCollection<CharacterProfile>("profiles");
                var existingTrigger = triggers.FindOne(Query.EQ("Name", textboxName.Text));
                var characters = profiles.FindAll();
                if (existingTrigger != null)
                {

                    //Update Trigger Instead
                    MessageBoxResult result = MessageBox.Show("Update Trigger?", "Add Trigger Error", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        existingTrigger.Name = textboxName.Text;
                        existingTrigger.SearchText = textboxSearch.Text;
                        existingTrigger.Comments = textboxComments.Text;
                        existingTrigger.Regex = (Boolean)checkboxRegex.IsChecked;
                        existingTrigger.Fastcheck = (Boolean)checkboxFast.IsChecked;
                        existingTrigger.Parent = selectedGroupId;
                        existingTrigger.TriggerCategory = (Category)comboCategory.SelectedItem;
                        existingTrigger.Displaytext = textboxBasicDisplay.Text;
                        existingTrigger.Clipboardtext = textboxBasicClipboard.Text;
                        existingTrigger.AudioSettings = basicAudioSettings;
                        existingTrigger.TimerType = comboTimerType.Text;
                        existingTrigger.TimerName = textboxTimerName.Text;
                        existingTrigger.TimerDuration = GetDuration(textboxTimerHours.Text, textboxTimerMinutes.Text, textboxTimerSeconds.Text);
                        existingTrigger.TriggeredAgain = comboTriggered.SelectedIndex;
                        existingTrigger.EndEarlyText = endEarlyCases;
                        existingTrigger.TimerEndingDuration = GetDuration(textboxEndingHours.Text, textboxEndingMinutes.Text, textboxEndingSeconds.Text);
                        existingTrigger.TimerEndingDisplayText = textboxEndingDisplay.Text;
                        existingTrigger.TimerEndingClipboardText = textboxEndingClipboard.Text;
                        existingTrigger.TimerEndingAudio = endingEarlyAudioSettings;
                        existingTrigger.TimerEnding = (Boolean)checkboxEndingNotify.IsChecked;
                        existingTrigger.TimerEndedClipboardText = textboxEndedClipboard.Text;
                        existingTrigger.TimerEndedDisplayText = textboxEndedDisplay.Text;
                        existingTrigger.TimerEnded = (Boolean)checkboxEndedNotify.IsChecked;
                        existingTrigger.TimerEndedAudio = endedEarlyAudioSettings;
                        existingTrigger.ResetCounter = (Boolean)checkboxCounterNotify.IsChecked;
                        existingTrigger.ResetCounterDuration = GetDuration(textboxCounterHours.Text, textboxCounterMinutes.Text, textboxCounterSeconds.Text);
                        triggers.Update(existingTrigger);
                    }
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    var newTrigger = new Trigger
                    {
                        Name = textboxName.Text,
                        SearchText = textboxSearch.Text,
                        Comments = textboxComments.Text,
                        Regex = (Boolean)checkboxRegex.IsChecked,
                        Fastcheck = (Boolean)checkboxFast.IsChecked,
                        Parent = selectedGroupId,
                        TriggerCategory = (Category)comboCategory.SelectedItem,
                        Displaytext = textboxBasicDisplay.Text,
                        Clipboardtext = textboxBasicClipboard.Text,
                        AudioSettings = basicAudioSettings,
                        TimerType = comboTimerType.Text,
                        TimerName = textboxTimerName.Text,
                        TimerDuration = GetDuration(textboxTimerHours.Text, textboxTimerMinutes.Text, textboxTimerSeconds.Text),
                        TriggeredAgain = comboTriggered.SelectedIndex,
                        EndEarlyText = endEarlyCases,
                        TimerEndingDuration = GetDuration(textboxEndingHours.Text, textboxEndingMinutes.Text, textboxEndingSeconds.Text),
                        TimerEndingDisplayText = textboxEndingDisplay.Text,
                        TimerEndingClipboardText = textboxEndingClipboard.Text,
                        TimerEndingAudio = endingEarlyAudioSettings,
                        TimerEnding = (Boolean)checkboxEndingNotify.IsChecked,
                        TimerEndedClipboardText = textboxEndedClipboard.Text,
                        TimerEndedDisplayText = textboxEndedDisplay.Text,
                        TimerEnded = (Boolean)checkboxEndedNotify.IsChecked,
                        TimerEndedAudio = endedEarlyAudioSettings,
                        ResetCounter = (Boolean)checkboxCounterNotify.IsChecked,
                        ResetCounterDuration = GetDuration(textboxCounterHours.Text, textboxCounterMinutes.Text, textboxCounterSeconds.Text)
                    };

                    newTriggerId = triggers.Insert(newTrigger);
                    var getTriggerGroup = triggergroups.FindById(existingTrigger.Parent);
                    getTriggerGroup.AddTriggers(newTriggerId.AsInt32);
                    triggergroups.Update(getTriggerGroup);
                    //If group is marked as default, enable it on each character
                    if (getTriggerGroup.DefaultEnabled)
                    {
                        foreach (CharacterProfile character in characters)
                        {
                            character.AddTrigger(newTriggerId.AsInt32);
                            var trigger = triggers.FindById(newTriggerId);
                            if (!(trigger.Profiles.Contains(newTriggerId.AsInt32)))
                            {
                                trigger.Profiles.Add(character.Id);
                                triggers.Update(trigger);
                            }
                            profiles.Update(character);

                        }
                    }
                }
            }
            this.Close();
        }
        private void ComboTimerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(comboTimerType.SelectedIndex)
            {
                case 0:
                    if(tabEnding != null && tabEnded != null)
                    {
                        tabEnding.IsEnabled = false;
                        tabEnded.IsEnabled = false;
                        DisableTimerEntry();
                    }
                    break;
                case 1:
                    tabEnding.IsEnabled = true;
                    tabEnded.IsEnabled = true;
                    EnableTimerEntry();
                    break;
                case 2:
                    tabEnding.IsEnabled = false;
                    tabEnded.IsEnabled = true;
                    EnableTimerEntry();
                    break;
                case 3:
                    tabEnding.IsEnabled = true;
                    tabEnded.IsEnabled = true;
                    EnableTimerEntry();
                    break;
                default:
                    break;
            }
        }
        private void EnableTimerEntry()
        {
            labelTimerDuration.IsEnabled = true;
            labelTimerHours.IsEnabled = true;
            labelTimerMinutes.IsEnabled = true;
            labelTimerName.IsEnabled = true;
            labelTimerSeconds.IsEnabled = true;
            labelTimerTriggered.IsEnabled = true;
            labelTimerType.IsEnabled = true;
        }
        private void DisableTimerEntry()
        {
            labelTimerDuration.IsEnabled = false;
            labelTimerHours.IsEnabled = false;
            labelTimerMinutes.IsEnabled = false;
            labelTimerName.IsEnabled = false;
            labelTimerSeconds.IsEnabled = false;
            labelTimerTriggered.IsEnabled = false;
            labelTimerType.IsEnabled = false;
        }
        private void CheckboxBasicDisplay_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxBasicDisplay.IsEnabled = false;
        }
        private void CheckboxBasicClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxBasicClipboard.IsEnabled = false;
        }
    }
}
