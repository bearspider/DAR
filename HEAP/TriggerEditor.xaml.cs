using LiteDB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for TriggerEditor.xaml
    /// </summary>
    ///     {

    public partial class TriggerEditor : Window
    {
        private CharacterProfile selectedCharacter;
        private String selectedCategory;
        private string selectedGroupId;
        private BindingList<SearchText> endEarlyCases = new BindingList<SearchText>();
        private DataTable dt = new DataTable();
        private Audio basicAudioSettings = new Audio();
        private Audio endedEarlyAudioSettings = new Audio();
        private Audio endingEarlyAudioSettings = new Audio();
        private Regex digestregex = new Regex(@"\[(?<digest>\w\s?\w?\s?\w?\s?\w?\s?\w?\s?\w?\s?\w?\s?)", RegexOptions.Compiled);

        public TriggerEditor()
        {
            InitializeComponent();
        }
        public TriggerEditor(TreeViewModel selectedGroup)
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
            datagridEarly.DataContext = dt.DefaultView;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                ILiteCollection<TriggerGroup> groupsCol = db.GetCollection<TriggerGroup>("triggergroups");
                var getTriggerGroup = groupsCol.FindOne(x => x.UniqueId == selectedGroup.Id);
                selectedGroupId = getTriggerGroup.UniqueId;
            }
            LoadCharacters();
        }
        public TriggerEditor(int selectedTrigger)
        {
            InitializeComponent();
            LoadCharacters();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                ILiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                ILiteCollection<Category> categories = db.GetCollection<Category>("categories");                
                Trigger trigger = triggers.FindById(selectedTrigger);
                Category category = categories.FindById(trigger.TriggerCategory);
                textboxName.Text = trigger.Name;
                textboxSearch.Text = trigger.SearchText;
                textboxComments.Text = trigger.Comments;
                checkboxRegex.IsChecked = trigger.Regex;
                checkboxFast.IsChecked = trigger.Fastcheck;
                selectedGroupId = trigger.Parent;
                comboCategory.SelectedItem = category.Name;
                datagridEarly.DataContext = trigger.EndEarlyText;
                textboxBasicDisplay.Text = trigger.Displaytext;
                if(textboxBasicDisplay.Text != "")
                {
                    checkboxBasicDisplay.IsChecked = true;
                }
                textboxBasicClipboard.Text = trigger.Clipboardtext;
                if (textboxBasicClipboard.Text != "")
                {
                    checkboxBasicClipboard.IsChecked = true;
                }
                switch (trigger.AudioSettings.AudioType)
                {
                    case "nosound":
                        radioBasicNoSound.IsChecked = true;
                        break;
                    case "tts":
                        radioBasicTTS.IsChecked = true;
                        textboxBasicTTS.Text = trigger.AudioSettings.TTS;
                        checkboxBasicInterrupt.IsChecked = trigger.AudioSettings.Interrupt;
                        break;
                    case "file":
                        radioBasicPlay.IsChecked = true;
                        textboxBasicSoundFile.Text = trigger.AudioSettings.SoundFileId;
                        break;
                    default:
                        break;
                }
                comboTimerType.Text = trigger.TimerType;
                textboxTimerName.Text = trigger.TimerName;
                TimeSpan timer = TimeSpan.FromSeconds(trigger.TimerDuration);
                textboxTimerHours.Text = timer.Hours.ToString();
                textboxTimerMinutes.Text = timer.Minutes.ToString();
                textboxTimerSeconds.Text = timer.Seconds.ToString();
                comboTriggered.SelectedIndex = trigger.TriggeredAgain;
                endEarlyCases = trigger.EndEarlyText;
                TimeSpan endingTimer = TimeSpan.FromSeconds(trigger.TimerEndingDuration);
                textboxEndingHours.Text = endingTimer.Hours.ToString();
                textboxEndingMinutes.Text = endingTimer.Minutes.ToString();
                textboxEndingSeconds.Text = endingTimer.Seconds.ToString();
                textboxEndingDisplay.Text = trigger.TimerEndingDisplayText;
                textboxEndingClipboard.Text = trigger.TimerEndingClipboardText;
                if (textboxEndingDisplay.Text != "")
                {
                    checkboxEndingDisplay.IsChecked = true;
                }
                if (textboxEndingDisplay.Text != "")
                {
                    checkboxEndingDisplay.IsChecked = true;
                }
                switch (trigger.TimerEndingAudio.AudioType)
                {
                    case "nosound":
                        radioEndingNoSound.IsChecked = true;
                        break;
                    case "tts":
                        radioEndingTTS.IsChecked = true;
                        textboxEndingTTS.Text = trigger.TimerEndingAudio.TTS;
                        checkboxEndingInterrupt.IsChecked = trigger.TimerEndingAudio.Interrupt;
                        break;
                    case "file":
                        radioEndingPlay.IsChecked = true;
                        textboxEndingSoundFile.Text = trigger.TimerEndingAudio.SoundFileId;
                        break;
                    default:
                        break;
                }
                checkboxEndingNotify.IsChecked = trigger.TimerEnding;
                textboxEndedClipboard.Text = trigger.TimerEndedClipboardText;
                textboxEndedDisplay.Text = trigger.TimerEndedDisplayText;
                if (textboxEndedDisplay.Text != "")
                {
                    checkboxEndedDisplay.IsChecked = true;
                }
                if (textboxEndedDisplay.Text != "")
                {
                    checkboxEndedDisplay.IsChecked = true;
                }
                checkboxEndedNotify.IsChecked = trigger.TimerEnded;
                switch (trigger.TimerEndedAudio.AudioType)
                {
                    case "nosound":
                        radioEndedNoSound.IsChecked = true;
                        break;
                    case "tts":
                        radioEndedTTS.IsChecked = true;
                        textboxEndedTTS.Text = trigger.TimerEndedAudio.TTS;
                        checkboxEndedInterrupt.IsChecked = trigger.TimerEndedAudio.Interrupt;
                        break;
                    case "file":
                        radioEndedPlay.IsChecked = true;
                        textboxEndedSoundFile.Text = trigger.TimerEndedAudio.SoundFileId;
                        break;
                    default:
                        break;
                }
                checkboxCounterNotify.IsChecked = trigger.ResetCounter;
                TimeSpan resetTimer = TimeSpan.FromSeconds(trigger.ResetCounterDuration);
                textboxCounterHours.Text = resetTimer.Hours.ToString();
                textboxCounterMinutes.Text = resetTimer.Minutes.ToString();
                textboxCounterSeconds.Text = resetTimer.Seconds.ToString();
            }
        }
        private string CreateDigest(string searchstring)
        {
            string digest = "";
            Match digestmatch = digestregex.Match(searchstring);
            if (digestmatch.Success)
            {
                digest = digestmatch.Groups["digest"].Value.ToString();
            }
            return digest;
        }
        private int GetDuration(String hours, String minutes, String seconds)
        {
            int totalDuration = 0;
            totalDuration += (Int32.Parse(hours)) * 60 * 60;
            totalDuration += (Int32.Parse(minutes)) * 60;
            totalDuration += (Int32.Parse(seconds));
            return totalDuration;
        }
        private void LoadCharacters()
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                ILiteCollection<CharacterProfile> characterProfilesCol = db.GetCollection<CharacterProfile>("profiles");
                ILiteCollection<Category> categoriesCol = db.GetCollection<Category>("categories");
                var profiles = characterProfilesCol.FindAll();
                foreach (var profile in profiles)
                {
                    comboBasicTest.Items.Add(profile.ProfileName);
                    comboEndingTest.Items.Add(profile.ProfileName);
                    comboEndedTest.Items.Add(profile.ProfileName);
                }
                if (profiles.Count<CharacterProfile>() > 0)
                {
                    comboBasicTest.SelectedIndex = 0;
                    comboEndingTest.SelectedIndex = 0;
                    comboEndedTest.SelectedIndex = 0;
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
                    selectedCategory = comboCategory.SelectedValue.ToString();
                }
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
            labelEarlyText.IsEnabled = true;
            textboxTimerHours.IsEnabled = true;
            textboxTimerMinutes.IsEnabled = true;
            textboxTimerName.IsEnabled = true;
            textboxTimerSeconds.IsEnabled = true;
            datagridEarly.IsEnabled = true;
            comboTriggered.IsEnabled = true;
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
            textboxTimerHours.IsEnabled = false;
            textboxTimerMinutes.IsEnabled = false;
            textboxTimerName.IsEnabled = false;
            textboxTimerSeconds.IsEnabled = false;
            datagridEarly.IsEnabled = false;
            comboTriggered.IsEnabled = false;
            labelEarlyText.IsEnabled = false;
        }
        #region Events        
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
            buttonBasicTest.IsEnabled = true;
            radioBasicNoSound.IsChecked = false;
            radioBasicTTS.IsChecked = false;
        }
        private void RadioBasicNoSound_Checked(object sender, RoutedEventArgs e)
        {
            if (textboxBasicTTS != null)
            {
                textboxBasicTTS.IsEnabled = false;
            }
            if (labelBasicTTS != null)
            {
                labelBasicTTS.IsEnabled = false;
            }
            if (checkboxBasicInterrupt != null)
            {
                checkboxBasicInterrupt.IsEnabled = false;
            }
            if (textboxBasicSoundFile != null)
            {
                textboxBasicSoundFile.IsEnabled = false;
            }
            if (labelBasicSoundFile != null)
            {
                labelBasicSoundFile.IsEnabled = false;
            }
            if (buttonBasicSoundFile != null)
            {
                buttonBasicSoundFile.IsEnabled = false;
            }
            if (buttonBasicTest != null)
            {
                buttonBasicTest.IsEnabled = false;
            }
            if (radioBasicPlay != null)
            {
                radioBasicPlay.IsChecked = false;
            }
            if (radioBasicTTS != null)
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
            //Basic Audio Settings
            if ((Boolean)radioBasicNoSound.IsChecked)
            {
                basicAudioSettings.AudioType = "nosound";
            }
            if ((Boolean)radioBasicPlay.IsChecked)
            {
                basicAudioSettings.AudioType = "file";
                basicAudioSettings.SoundFileId = textboxBasicSoundFile.Text;
            }
            if ((Boolean)radioBasicTTS.IsChecked)
            {
                basicAudioSettings.AudioType = "tts";
                basicAudioSettings.TTS = textboxBasicTTS.Text;
                basicAudioSettings.Interrupt = (Boolean)checkboxBasicInterrupt.IsChecked;
            }
            //Timer Ending Audio Settings
            if ((Boolean)radioEndingNoSound.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "nosound";
            }
            if ((Boolean)radioEndingPlay.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "file";
                endingEarlyAudioSettings.SoundFileId = textboxEndingSoundFile.Text;
            }
            if ((Boolean)radioEndingTTS.IsChecked)
            {
                endingEarlyAudioSettings.AudioType = "tts";
                endingEarlyAudioSettings.TTS = textboxEndingTTS.Text;
                endingEarlyAudioSettings.Interrupt = (Boolean)checkboxEndingInterrupt.IsChecked;
            }
            //Timer Ended Audio Settings
            if ((Boolean)radioEndedNoSound.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "nosound";
            }
            if ((Boolean)radioEndedPlay.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "file";
                endedEarlyAudioSettings.SoundFileId = textboxEndedSoundFile.Text;
            }
            if ((Boolean)radioEndedTTS.IsChecked)
            {
                endedEarlyAudioSettings.AudioType = "tts";
                endedEarlyAudioSettings.TTS = textboxEndedTTS.Text;
                endedEarlyAudioSettings.Interrupt = (Boolean)checkboxEndedInterrupt.IsChecked;
            }

            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                ILiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                ILiteCollection<TriggerGroup> triggergroups = db.GetCollection<TriggerGroup>("triggergroups");
                ILiteCollection<CharacterProfile> profiles = db.GetCollection<CharacterProfile>("profiles");
                ILiteCollection<Category> categories = db.GetCollection<Category>("categories");
                int categoryid = (categories.FindOne(Query.EQ("Name", comboCategory.SelectedItem.ToString()))).Id;
                var existingTrigger = triggers.FindOne(Query.EQ("Name", textboxName.Text));
                var characters = profiles.FindAll();
                if (existingTrigger != null)
                {

                    //Update Trigger Instead
                    MessageBoxResult result = MessageBox.Show("Update Trigger?", "Add Trigger Error", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {

                        existingTrigger.TimerType = comboTimerType.Text;
                        existingTrigger.Name = textboxName.Text;
                        existingTrigger.SearchText = textboxSearch.Text;
                        existingTrigger.Comments = textboxComments.Text;
                        existingTrigger.Regex = (Boolean)checkboxRegex.IsChecked;
                        existingTrigger.Fastcheck = (Boolean)checkboxFast.IsChecked;
                        existingTrigger.Digest = CreateDigest(textboxSearch.Text);
                        existingTrigger.Parent = selectedGroupId;
                        existingTrigger.TriggerCategory = categoryid;
                        existingTrigger.Displaytext = textboxBasicDisplay.Text;
                        existingTrigger.Clipboardtext = textboxBasicClipboard.Text;
                        existingTrigger.AudioSettings = basicAudioSettings;
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
                        Digest = CreateDigest(textboxSearch.Text),
                        UniqueId = Guid.NewGuid().ToString(),
                        Parent = selectedGroupId,
                        TriggerCategory = categoryid,
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
                    var getTriggerGroup = triggergroups.FindOne(x => x.UniqueId == selectedGroupId);
                    getTriggerGroup.AddTriggers(newTrigger.UniqueId);
                    triggergroups.Update(getTriggerGroup);
                    //If group is marked as default, enable it on each character
                    if (getTriggerGroup.DefaultEnabled)
                    {
                        foreach (CharacterProfile character in characters)
                        {
                            character.AddTrigger(newTrigger.UniqueId);
                            var trigger = triggers.FindById(newTriggerId);
                            if (!(trigger.Profiles.Contains(character.Id)))
                            {
                                trigger.Profiles.Add(character.Id);
                                triggers.Update(trigger);
                            }
                            profiles.Update(character);
                        }
                    }
                }
            }
            var main = App.Current.MainWindow as MainWindow;
            main.UpdateTriggerView();
            this.Close();
        }
        private void ButtonEndingTest_Click(object sender, RoutedEventArgs e)
        {
            if ((Boolean)radioEndingTTS.IsChecked)
            {
                selectedCharacter.Speak(textboxEndingTTS.Text);
            }
            if((Boolean)radioEndingPlay.IsChecked)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    Stream soundfile = new System.IO.MemoryStream();
                    db.FileStorage.Download($"{GlobalVariables.litedbfileprefix}{textboxEndingSoundFile.Text}", soundfile);
                    SoundPlayer test = new SoundPlayer(soundfile);
                    test.Stream.Position = 0;
                    test.Play();
                }
                   
            }
        }
        private void ButtonEndedTest_Click(object sender, RoutedEventArgs e)
        {
            if((Boolean)radioEndedTTS.IsChecked)
            {
                selectedCharacter.Speak(textboxEndedTTS.Text);
            }
            if ((Boolean)radioEndedPlay.IsChecked)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    Stream soundfile = new System.IO.MemoryStream();
                    db.FileStorage.Download($"{GlobalVariables.litedbfileprefix}{textboxEndedSoundFile.Text}", soundfile);
                    SoundPlayer test = new SoundPlayer(soundfile);
                    test.Stream.Position = 0;
                    test.Play();
                }

            }
        }
        private void ButtonBasicTest_Click(object sender, RoutedEventArgs e)
        {
            
            if ((Boolean)radioBasicTTS.IsChecked)
            {
                selectedCharacter.Speak(textboxBasicTTS.Text);
            }
            if ((Boolean)radioBasicPlay.IsChecked)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    Stream soundfile = new System.IO.MemoryStream();
                    db.FileStorage.Download($"{GlobalVariables.litedbfileprefix}{textboxBasicSoundFile.Text}", soundfile);
                    SoundPlayer test = new SoundPlayer(soundfile);
                    test.Stream.Position = 0;
                    test.Play();
                }

            }
        }
        private void ComboTimerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboTimerType.SelectedIndex)
            {
                case 0:
                    if (tabEnding != null && tabEnded != null)
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
        private void CheckboxBasicDisplay_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxBasicDisplay.IsEnabled = false;
        }
        private void CheckboxBasicClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxBasicClipboard.IsEnabled = false;
        }
        private void CheckboxEndingNotify_Checked(object sender, RoutedEventArgs e)
        {
            checkboxEndingClipboard.IsEnabled = true;
            checkboxEndingDisplay.IsEnabled = true;
            textboxEndingHours.IsEnabled = true;
            textboxEndingMinutes.IsEnabled = true;
            textboxEndingSeconds.IsEnabled = true;
            radioEndingNoSound.IsEnabled = true;
            radioEndingPlay.IsEnabled = true;
            radioEndingTTS.IsEnabled = true;
        }
        private void CheckboxEndingNotify_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxEndingClipboard.IsEnabled = false;
            textboxEndingDisplay.IsEnabled = false;
            textboxEndingHours.IsEnabled = false;
            textboxEndingMinutes.IsEnabled = false;
            textboxEndingSeconds.IsEnabled = false;
            radioEndingNoSound.IsEnabled = false;
            radioEndingPlay.IsEnabled = false;
            radioEndingTTS.IsEnabled = false;
        }
        private void RadioEndingNoSound_Checked(object sender, RoutedEventArgs e)
        {
            if(radioEndingTTS != null)
            {
                radioEndingTTS.IsChecked = false;
            }
            if(radioEndingPlay != null)
            {
                radioEndingPlay.IsChecked = false;
            }
            if(textboxEndingTTS != null)
            {
                textboxEndingTTS.IsEnabled = false;
            }
            if(labelEndingTTS != null)
            {
                labelEndingTTS.IsEnabled = false;
            }
            if(labelEndingSoundFile != null)
            {
                labelEndingSoundFile.IsEnabled = false;
            }
            if(buttonEndingSoundFile != null)
            {
                buttonEndingSoundFile.IsEnabled = false;
            }
            if(buttonEndingTest != null)
            {
                buttonEndingTest.IsEnabled = false;
            }
            
        }
        private void RadioEndingNoSound_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private void RadioEndingTTS_Checked(object sender, RoutedEventArgs e)
        {
            radioEndingNoSound.IsChecked = false;
            radioEndingPlay.IsChecked = false;
            labelEndingTTS.IsEnabled = true;
            textboxEndingTTS.IsEnabled = true;
            checkboxEndingInterrupt.IsEnabled = true;
            if (textboxEndingTTS.Text.Length > 0)
            {
                buttonEndingTest.IsEnabled = true;
            }
            else
            {
                buttonEndingTest.IsEnabled = false;
            }
        }
        private void RadioEndingTTS_Unchecked(object sender, RoutedEventArgs e)
        {
            labelEndingTTS.IsEnabled = false;
            textboxEndingTTS.IsEnabled = false;
            checkboxEndingInterrupt.IsEnabled = false;
        }
        private void RadioEndingPlay_Checked(object sender, RoutedEventArgs e)
        {
            radioEndingTTS.IsChecked = false;
            radioEndingNoSound.IsChecked = false;
            labelEndingTTS.IsEnabled = false;
            textboxEndingTTS.IsEnabled = false;
            checkboxEndingInterrupt.IsEnabled = false;
            labelEndingSoundFile.IsEnabled = true;
            textboxEndingSoundFile.IsEnabled = true;
            buttonEndingSoundFile.IsEnabled = true;
            buttonEndingTest.IsEnabled = true;
        }
        private void RadioEndingPlay_Unchecked(object sender, RoutedEventArgs e)
        {
            labelEndingSoundFile.IsEnabled = false;
            textboxEndingSoundFile.IsEnabled = false;
            buttonEndingSoundFile.IsEnabled = false;
            buttonEndingTest.IsEnabled = false;
        }
        private void CheckboxEndingDisplay_Checked(object sender, RoutedEventArgs e)
        {
            textboxEndingDisplay.IsEnabled = true;
        }
        private void CheckboxEndingDisplay_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxEndingDisplay.IsEnabled = false;
        }
        private void CheckboxEndingClipboard_Checked(object sender, RoutedEventArgs e)
        {
            textboxEndingClipboard.IsEnabled = true;
        }
        private void CheckboxEndingClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxEndingClipboard.IsEnabled = false;
        }
        private void CheckboxCounterNotify_Checked(object sender, RoutedEventArgs e)
        {
            textboxCounterHours.IsEnabled = true;
            textboxCounterMinutes.IsEnabled = true;
            textboxCounterSeconds.IsEnabled = true;
        }
        private void CheckboxCounterNotify_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxCounterHours.IsEnabled = false;
            textboxCounterMinutes.IsEnabled = false;
            textboxCounterSeconds.IsEnabled = false;
        }
        private void CheckboxEndedNotify_Checked(object sender, RoutedEventArgs e)
        {
            checkboxEndedDisplay.IsEnabled = true;
            checkboxEndedClipboard.IsEnabled = true;
            radioEndedNoSound.IsEnabled = true;
            radioEndedPlay.IsEnabled = true;
            radioEndedTTS.IsEnabled = true;
        }
        private void CheckboxEndedNotify_Unchecked(object sender, RoutedEventArgs e)
        {
            checkboxEndedDisplay.IsEnabled = false;
            checkboxEndedClipboard.IsEnabled = false;
            radioEndedNoSound.IsEnabled = false;
            radioEndedPlay.IsEnabled = false;
            radioEndedTTS.IsEnabled = false;
        }
        private void CheckboxEndedDisplay_Checked(object sender, RoutedEventArgs e)
        {
            textboxEndedDisplay.IsEnabled = true;
        }
        private void CheckboxEndedDisplay_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxEndedDisplay.IsEnabled = false;
        }
        private void CheckboxEndedClipboard_Checked(object sender, RoutedEventArgs e)
        {
            textboxEndedClipboard.IsEnabled = true;
        }
        private void CheckboxEndedClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxEndedClipboard.IsEnabled = false;
        }
        private void RadioEndedNoSound_Checked(object sender, RoutedEventArgs e)
        {
            if(radioEndedTTS != null)
            {
                radioEndedTTS.IsChecked = false;
            }
            if(radioEndedPlay != null)
            {
                radioEndedPlay.IsChecked = false;
            }
            if(labelEndedTTS != null)
            {
                labelEndedTTS.IsEnabled = false;
            }
            if(textboxEndedTTS != null)
            {
                textboxEndedTTS.IsEnabled = false;
            }
            if(checkboxEndedInterrupt != null)
            {
                checkboxEndedInterrupt.IsEnabled = false;
            }
            if(labelEndedSoundFile != null)
            {
                labelEndedSoundFile.IsEnabled = false;
            }
            if(textboxEndedSoundFile != null)
            {
                textboxEndedSoundFile.IsEnabled = false;
            }
            if(buttonEndedSoundFile != null)
            {
                buttonEndedSoundFile.IsEnabled = false;
            }
            if(buttonEndedTest != null)
            {
                buttonEndedTest.IsEnabled = false;
            }            
        }
        private void RadioEndedTTS_Checked(object sender, RoutedEventArgs e)
        {
            radioEndedNoSound.IsChecked = false;
            radioEndedPlay.IsChecked = false;
            labelEndedTTS.IsEnabled = true;
            textboxEndedTTS.IsEnabled = true;
            checkboxEndedInterrupt.IsEnabled = true;
            labelEndedSoundFile.IsEnabled = false;
            textboxEndedSoundFile.IsEnabled = false;
            buttonEndedSoundFile.IsEnabled = false;
            if (textboxEndedTTS.Text.Length > 0)
            {
                buttonEndedTest.IsEnabled = true;
            }
            else
            {
                buttonEndedTest.IsEnabled = false;
            }
        }
        private void RadioEndedTTS_Unchecked(object sender, RoutedEventArgs e)
        {
            labelEndedTTS.IsEnabled = false;
            textboxEndedTTS.IsEnabled = false;
            checkboxEndedInterrupt.IsEnabled = false;
            buttonEndedTest.IsEnabled = false;
        }
        private void RadioEndedPlay_Checked(object sender, RoutedEventArgs e)
        {
            radioEndedNoSound.IsChecked = false;
            radioEndedTTS.IsChecked = false;
            labelEndedTTS.IsEnabled = false;
            textboxEndedTTS.IsEnabled = false;
            checkboxEndedInterrupt.IsEnabled = false;
            labelEndedSoundFile.IsEnabled = true;
            textboxEndedSoundFile.IsEnabled = true;
            buttonEndedSoundFile.IsEnabled = true;
            buttonEndedTest.IsEnabled = true;
        }
        private void RadioEndedPlay_Unchecked(object sender, RoutedEventArgs e)
        {
            labelEndedSoundFile.IsEnabled = false;
            textboxEndedSoundFile.IsEnabled = false;
            buttonEndedSoundFile.IsEnabled = false;
            buttonEndedTest.IsEnabled = false;
        }
        private void ButtonBasicSoundFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Audio Files|*.wav";
            if (fileDialog.ShowDialog() == true)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    db.FileStorage.Upload($"{GlobalVariables.litedbfileprefix}{(fileDialog.SafeFileName).Replace(" ","")}", fileDialog.FileName);
                }
                textboxBasicSoundFile.Text = (fileDialog.SafeFileName).Replace(" ", "");
            }
        }
        private void ButtonEndingSoundFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Audio Files|*.wav";
            if (fileDialog.ShowDialog() == true)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    db.FileStorage.Upload($"{GlobalVariables.litedbfileprefix}{fileDialog.SafeFileName}", fileDialog.FileName);
                }
                textboxEndingSoundFile.Text = fileDialog.SafeFileName;
            }
        }
        private void ButtonEndedSoundFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Audio Files|*.wav";
            if (fileDialog.ShowDialog() == true)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    db.FileStorage.Upload($"{GlobalVariables.litedbfileprefix}{fileDialog.SafeFileName}", fileDialog.FileName);
                }
                textboxEndedSoundFile.Text = fileDialog.SafeFileName;
            }
        }
        private void TextboxEndedTTS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxEndedTTS.Text.Length > 0)
            {
                buttonEndedTest.IsEnabled = true;
            }
            else
            {
                buttonEndedTest.IsEnabled = false;
            }
        }
        private void TextboxEndingTTS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxEndingTTS.Text.Length > 0)
            {
                buttonEndingTest.IsEnabled = true;
            }
            else
            {
                buttonEndingTest.IsEnabled = false;
            }
        }
        #endregion

        private void ButtonTimerCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
