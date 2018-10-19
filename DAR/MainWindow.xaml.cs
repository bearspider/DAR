using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;
using System.Media;
using Microsoft.Windows.Controls.Ribbon;

namespace DAR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class GlobalVariables
    {
        public static string defaultPath = @"C:\EQAudioTriggers";
        public static string defaultDB = $"{defaultPath}\\eqtriggers.db";
        public static string eqRegex = @"\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\](?<stringToMatch>.*)";
        public static string pathRegex = @"(?<logdir>.*\\)(?<logname>eqlog_.*\.txt)";
    }
    #region Converters
    public class MonitoringStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if((Boolean)value)
            {
                rString = "Images/Knob-Remove-icon.png";
            }
            else
            {
                rString = "Images/Knob-Remove-Red-icon.png";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class FileStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if ((Boolean)value)
            {
                rString = "";
            }
            else
            {
                rString = "Images/Oxygen-Icons.org-Oxygen-Status-image-missing.ico";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    public partial class MainWindow : Window
    {
        #region Properties
        private TreeViewModel tv;
        private List<TreeViewModel> treeView;
        public ObservableCollection<CharacterProfile> characterProfiles = new ObservableCollection<CharacterProfile>();
        private String currentSelection;
        private Dictionary<String, FileSystemWatcher> watchers = new Dictionary<String, FileSystemWatcher>();
        private Dictionary<Trigger,ArrayList> activeTriggers = new Dictionary<Trigger,ArrayList>();
        private ObservableCollection<OverlayTextWindow> textWindows = new ObservableCollection<OverlayTextWindow>();
        private ObservableCollection<OverlayTimerWindow> timerWindows = new ObservableCollection<OverlayTimerWindow>();
        private ObservableCollection<Category> categorycollection = new ObservableCollection<Category>();
        private ObservableCollection<OverlayTimer> availoverlaytimers = new ObservableCollection<OverlayTimer>();
        private ObservableCollection<OverlayText> availoverlaytexts = new ObservableCollection<OverlayText>();
        private CategoryWrapper categoryWrapper = new CategoryWrapper();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = Directory.Exists(GlobalVariables.defaultPath);
            if (!mainPath)
            {
                Directory.CreateDirectory(GlobalVariables.defaultPath);
            }
            //Prep and/or load database
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {                
                LiteCollection<CharacterProfile> dbcharacterProfiles = db.GetCollection<CharacterProfile>("profiles");
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");                
                LiteCollection<TriggerGroup> triggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                LiteCollection<Category> categories = db.GetCollection<Category>("categories");
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                dbcharacterProfiles.EnsureIndex((CharacterProfile x) => x.Id, true);
                overlaytimers.EnsureIndex((OverlayTimer v) => v.Id, true);
                triggerGroups.EnsureIndex((TriggerGroup y) => y.Id, true);
                overlaytexts.EnsureIndex((OverlayText u) => u.Id, true);
                categories.EnsureIndex((Category z) => z.Id, true);
                triggers.EnsureIndex((Trigger w) => w.Id, true);
            }
            UpdateListView();
            UpdateTriggerView();
            OverlayText_Refresh();
            OverlayTimer_Refresh();
            //Deploy Overlays
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                Trigger testtrigger = triggers.FindById(14);

                IEnumerable<Category> availcategories = categoriescol.FindAll();
                if(availcategories.Count<Category>() == 0)
                {
                    Category defaultcategory = new Category();
                    defaultcategory.DefaultCategory = true;
                    categoriescol.Insert(defaultcategory);
                    availcategories = categoriescol.FindAll();
                }
                foreach (var overlay in overlaytexts.FindAll())
                {
                    OverlayTextWindow newWindow = new OverlayTextWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    textWindows.Add(newWindow);
                    //newWindow.AddTrigger(testtrigger);
                    //newWindow.Show();
                }
                foreach (var overlay in overlaytimers.FindAll())
                {
                    OverlayTimerWindow newWindow = new OverlayTimerWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    timerWindows.Add(newWindow);
                    //newWindow.Show();
                }
                Refresh_Categories();                
            }
            //Start Monitoring Enabled Profiles
            
            foreach (CharacterProfile character in characterProfiles)
            {
                RibbonSplitMenuItem characterStopAlerts = new RibbonSplitMenuItem();
                RibbonSplitMenuItem characterResetCounters = new RibbonSplitMenuItem();
                characterStopAlerts.Header = character.Name;
                characterResetCounters.Header = character.Name;
                rbnStopAlerts.Items.Add(characterStopAlerts);
                rbnResetCounters.Items.Add(characterResetCounters);
                if(File.Exists(character.LogFile) && character.Monitor)
                {                    
                    MonitorCharacter(character);
                }
                else
                {
                    //Don't monitor character
                }
            }
        }
        #region Form Functions
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ribbonMain.Width = ActualWidth;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                foreach (CharacterProfile doc in colProfiles.FindAll())
                {
                    if (doc.MonitorAtStartup != doc.Monitor)
                    {
                        doc.Monitor = doc.MonitorAtStartup;
                        colProfiles.Update(doc);
                    }
                }
            }
            Environment.Exit(Environment.ExitCode);
        }
        public void OverlayTimer_Refresh()
        {
            categoryWrapper.OverlayTimers.Clear();
            availoverlaytimers.Clear();
            for (int i = ribbongroupTimerOverlays.Items.Count; i > 1; i--)
            {
                ribbongroupTimerOverlays.Items.RemoveAt(i - 1);
            }
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                foreach (var overlay in overlaytimers.FindAll())
                {
                    categoryWrapper.OverlayTimers.Add(overlay);
                    availoverlaytimers.Add(overlay);
                    RibbonSplitButton overlaytimer = new RibbonSplitButton();
                    overlaytimer.Label = overlay.Name;
                    overlaytimer.LargeImageSource = new BitmapImage(new Uri(@"Images/Google-Noto-Emoji-Travel-Places-42608-stopwatch.ico", UriKind.RelativeOrAbsolute));
                    overlaytimer.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
                    RibbonSplitMenuItem timerProperties = new RibbonSplitMenuItem();
                    timerProperties.Name = overlay.Name;
                    timerProperties.Header = "Properties";
                    timerProperties.AddHandler(Button.ClickEvent, new RoutedEventHandler(TimerOverlayProperties_Click));
                    RibbonSplitMenuItem timerDelete = new RibbonSplitMenuItem();
                    timerDelete.Header = "Delete";
                    timerDelete.Name = overlay.Name;
                    timerDelete.AddHandler(Button.ClickEvent, new RoutedEventHandler(TimerOverlayDelete_Click));
                    overlaytimer.Items.Add(timerProperties);
                    overlaytimer.Items.Add(timerDelete);
                    ribbongroupTimerOverlays.Items.Add(overlaytimer);
                }
            }
            Refresh_Categories();
        }

        public void OverlayText_Refresh()
        {
            categoryWrapper.OverlayTexts.Clear();
            availoverlaytexts.Clear();
            for(int i = ribbongroupTextOverlays.Items.Count; i > 1 ; i--)
            {
                ribbongroupTextOverlays.Items.RemoveAt(i-1);
            }
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                foreach (var overlay in overlaytexts.FindAll())
                {
                    categoryWrapper.OverlayTexts.Add(overlay);
                    availoverlaytexts.Add(overlay);
                    RibbonSplitButton overlaytext = new RibbonSplitButton();
                    overlaytext.Label = overlay.Name;
                    overlaytext.LargeImageSource = new BitmapImage(new Uri(@"Images/Oxygen-Icons.org-Oxygen-Actions-document-new.ico", UriKind.RelativeOrAbsolute));
                    overlaytext.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
                    RibbonSplitMenuItem textProperties = new RibbonSplitMenuItem();
                    textProperties.Name = overlay.Name;
                    textProperties.Header = "Properties";
                    textProperties.AddHandler(Button.ClickEvent, new RoutedEventHandler(TextOverlayProperties_Click));
                    RibbonSplitMenuItem textDelete = new RibbonSplitMenuItem();
                    textDelete.Header = "Delete";
                    textDelete.Name = overlay.Name;
                    textDelete.AddHandler(Button.ClickEvent, new RoutedEventHandler(TextOverlayDelete_Click));
                    overlaytext.Items.Add(textProperties);
                    overlaytext.Items.Add(textDelete);
                    ribbongroupTextOverlays.Items.Add(overlaytext);
                }
            }
            Refresh_Categories();
        }
        #endregion
        #region Character Profiles
        private void RibbonButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                CharacterProfile result = col.FindOne(Query.EQ("ProfileName", selectedCharacter.ProfileName));
                ProfileEditor editCharacter = new ProfileEditor(result);
                editCharacter.Show();
            }
            UpdateView();
        }
        private void RibbonButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                var triggers = db.GetCollection<Trigger>("triggers");
                var categories = db.GetCollection<Category>("categories");
                String selectedCharacter = ((CharacterProfile)listviewCharacters.SelectedItem).ProfileName;
                int profileid = ((CharacterProfile)listviewCharacters.SelectedItem).Id;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {selectedCharacter}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var dbdelete = col.Delete(Query.EQ("ProfileName", selectedCharacter));
                    currentSelection = null;
                    foreach(var trigger in triggers.FindAll())
                    {
                        if(trigger.Profiles.Contains(profileid))
                        {
                            trigger.Profiles.Remove(profileid);
                            triggers.Update(trigger);
                        }
                    }
                    foreach(var category in categories.FindAll())
                    {
                        var profile = from p in category.CharacterOverrides where p.ProfileName == selectedCharacter select p;
                        var collection = new ObservableCollection<CharacterOverride>(profile);
                        category.CharacterOverrides.Remove(collection[0]);                                
                        categories.Update(category);
                    }
                    UpdateView();
                }
            }
        }
        private void RibbonButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ProfileEditor newProfile = new ProfileEditor();
            newProfile.Show();
            UpdateView();
        }
        private void ListviewCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ribbonCharEdit.IsEnabled = true;
            ribbonCharRemove.IsEnabled = true;
            //Update TriggerView with selected triggers from profile
            if (listviewCharacters.Items.Count > 0)
            {
                CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
                currentSelection = selectedCharacter.ProfileName;
                UpdateTriggerView();
            }
        }
        private void ListviewCharacters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CharacterProfile selected = ((ListView)sender).SelectedItem as CharacterProfile;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                CharacterProfile currentProfile = colProfiles.FindById(selected.Id);
                currentProfile.Monitor = !currentProfile.Monitor;
                colProfiles.Update(currentProfile);
                if (currentProfile.Monitor)
                {
                    MonitorCharacter(currentProfile);
                }
            }

            UpdateListView();
        }
        #endregion
        #region Monitoring
        private void MonitorCharacter(CharacterProfile character)
        {
            //Look into doing Parallel.Foreach with semaphores by cpu core count inspection
            Thread t = new Thread(() =>
            {
                using (FileStream filestream = File.Open(character.LogFile, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    filestream.Seek(0, SeekOrigin.End);
                    using (StreamReader streamReader = new StreamReader(filestream))
                    {
                        for (; ; )
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(5));
                            String capturedLine = streamReader.ReadToEnd();
                            if (capturedLine.Length > 0)
                            {
                                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                                {
                                    var triggerCollection = db.GetCollection<Trigger>("triggers");
                                    foreach(var doc in triggerCollection.FindAll())
                                    {
                                        MatchCollection matches = Regex.Matches(capturedLine,doc.SearchText, RegexOptions.IgnoreCase);
                                        if (matches.Count > 0)
                                        {
                                            foreach (Match tMatch in matches)
                                            {
                                                if(doc.AudioSettings.AudioType == "tts")
                                                { character.Speak(doc.AudioSettings.TTS); }
                                                if(doc.AudioSettings.AudioType == "file")
                                                { PlaySound(doc.AudioSettings.SoundFileId); }
                                                //Add Timer code

                                            }
                                        }
                                    }
                                }

                            }
                            if (characterProfiles.Any(x => x.Monitor == false && x.ProfileName == character.ProfileName))
                            {
                                break;
                            }
                        }
                    }
                }
            });
            t.Start();
        }
        private void Changed(object sender, FileSystemEventArgs e)
        {
            //Check line with trigger and then speak.
            CharacterProfile fromLog = characterProfiles.Single<CharacterProfile>(i => i.LogFile == e.FullPath);
            String lastline = File.ReadLines(e.FullPath).Last();            
            foreach(KeyValuePair<Trigger,ArrayList> entry in activeTriggers)
            {
                Trigger toCompare = entry.Key;
                MatchCollection matches = Regex.Matches(lastline, toCompare.SearchText, RegexOptions.IgnoreCase);
                if(matches.Count > 0)
                {
                    foreach(CharacterProfile character in entry.Value)
                    {
                        if(character.Monitor)
                        {
                            character.Speak(lastline);
                        }
                    }
                }
            }            
        }
        #endregion
        #region Triggers
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            TreeViewModel selectedGroup = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerEditor newTrigger = new TriggerEditor(selectedGroup);
            newTrigger.Show();
        }
        private void TriggerRemoved_TreeViewModel(object sender, PropertyChangedEventArgs e)
        {
            RemoveTrigger(e.PropertyName);
            UpdateView();
        }
        private void TriggerAdded_TreeViewModel(object sender, PropertyChangedEventArgs e)
        {
            AddTrigger(e.PropertyName);
            UpdateView();
        }
        public void RemoveTrigger(String triggerName)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                var currentProfile = colProfiles.FindById(selectedCharacter.Id);
                var currentTrigger = colTriggers.FindOne(Query.EQ("Name", triggerName));
                if ((currentTrigger.Profiles.Contains(selectedCharacter.Id)))
                {
                    currentProfile.Triggers.Remove(currentTrigger.id);
                    currentTrigger.profiles.Remove(selectedCharacter.Id);
                }
                colTriggers.Update(currentTrigger);
                colProfiles.Update(currentProfile);
            }
        }
        public void AddTrigger(String triggerName)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                var currentProfile = colProfiles.FindById(selectedCharacter.Id);
                var currentTrigger = colTriggers.FindOne(Query.EQ("Name", triggerName));
                if (!(currentTrigger.Profiles.Contains(selectedCharacter.Id)))
                {
                    currentProfile.Triggers.Add(currentTrigger.id);
                    currentTrigger.profiles.Add(selectedCharacter.Id);
                }
                colTriggers.Update(currentTrigger);
                colProfiles.Update(currentProfile);
            }            
        }
        private void TriggerRemove_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
                var col = db.GetCollection<Trigger>("triggers");
                var triggergroup = db.GetCollection<TriggerGroup>("triggergroups");
                var getTrigger = col.FindOne(Query.EQ("Name", root.Name));
                var getGroup = triggergroup.FindById(getTrigger.Parent);
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                    var profiles = colProfiles.FindAll();                    
                    foreach (CharacterProfile profile in profiles)
                    {
                        profile.Triggers.Remove(getTrigger.Id);
                        colProfiles.Update(profile);
                    }
                    getGroup.RemoveTrigger(getTrigger.Id);
                    triggergroup.Update(getGroup);
                    col.Delete(getTrigger.Id);
                }
            }
            UpdateView();
        }
        private void TriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var triggerCollection = db.GetCollection<Trigger>("triggers");
                var currentTrigger = triggerCollection.FindOne(Query.EQ("Name", root.Name));
                TriggerEditor triggerDialog = new TriggerEditor(currentTrigger.Id);
                triggerDialog.Show();
            }
        }
        #endregion
        #region Trigger Groups
        private void TriggerGroupsAdd_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEdit triggerDialog = new TriggerGroupEdit();
            triggerDialog.Show();
            e.Handled = true;
        }
        private void TriggerGroupsRemove_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var dbid = (col.FindOne(x => x.TriggerGroupName.Contains(root.Name))).Id;
                    var childContains = col.FindAll().Where(x => x.children.Contains(dbid));
                    foreach (var child in childContains)
                    {
                        child.Children.Remove(dbid);
                        col.Update(child);
                    }
                    col.Delete(dbid);
                    UpdateView();
                }
            }
            e.Handled = true;
        }
        private void TriggerGroupsAddSelected_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerGroupEdit triggerDialog = new TriggerGroupEdit(root);
            triggerDialog.Show();
            e.Handled = true;
        }
        private void TriggerGroupsEdit_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            //Add case where adding a sub group with the same name in a different parent updates the correct one
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                TriggerGroup result = col.FindOne(Query.And(Query.EQ("TriggerGroupName", root.Name),Query.EQ("_id",root.Id)));
                TriggerGroupEdit triggerDialog = new TriggerGroupEdit(result);
                triggerDialog.Show();
            }
            UpdateView();
        }
        private void TriggerGroupsAddTopLevel_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEdit triggerDialog = new TriggerGroupEdit();
            triggerDialog.Show();
            e.Handled = true;
            UpdateView();
        }
        #endregion
        #region Tree
        private TreeViewModel BuildTree(TriggerGroup branch)
        {
            TreeViewModel rTree = new TreeViewModel(branch.TriggerGroupName)
            {
                Type = "triggergroup",
                Id = branch.Id
            };
            if (branch.triggers.Count > 0)
            {
                foreach (Int32 item in branch.triggers)
                {
                    using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                    {
                        Boolean isChecked = false;
                        var col = db.GetCollection<Trigger>("triggers");
                        var getTrigger = col.FindById(item);
                        if (currentSelection != null)
                        {
                            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
                            if (selectedCharacter.Triggers.Contains(item))
                            {
                                isChecked = true;
                            }
                        }
                        TreeViewModel newChildBranch = new TreeViewModel(getTrigger.Name)
                        {
                            Type = "trigger"
                        };
                        newChildBranch.IsChecked = isChecked;
                        newChildBranch.TriggerAdded += TriggerAdded_TreeViewModel;
                        newChildBranch.TriggerRemoved += TriggerRemoved_TreeViewModel;
                        rTree.Children.Add(newChildBranch);
                    }
                }
            }
            if (branch.Children.Count > 0)
            {
                foreach (int leaf in branch.Children)
                {
                    TriggerGroup leafGroup = GetTriggerGroup(leaf);
                    rTree.Children.Add(BuildTree(leafGroup));
                }
            }
            rTree.VerifyCheckedState();
            return rTree;
        }
        private TriggerGroup GetTriggerGroup(int id)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                    var col = db.GetCollection<TriggerGroup>("triggergroups");
                    var record = col.FindById(id);
                    return record;
            }
        }
        private void TreeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root.Name != "All Triggers")
            {
                if (root.Type == "triggergroup")
                {
                    triggerGroupsAdd.IsEnabled = true;
                    triggerGroupsEdit.IsEnabled = true;
                    triggerGroupsRemove.IsEnabled = true;
                    triggerGroupsAddSelected.IsEnabled = true;
                    triggerAdd.IsEnabled = true;
                    triggerEdit.IsEnabled = false;
                    triggerRemove.IsEnabled = false;
                }
                else
                {
                    triggerGroupsAdd.IsEnabled = false;
                    triggerGroupsEdit.IsEnabled = false;
                    triggerGroupsRemove.IsEnabled = false;
                    triggerGroupsAddSelected.IsEnabled = false;
                    triggerAdd.IsEnabled = false;
                    triggerEdit.IsEnabled = true;
                    triggerRemove.IsEnabled = true;
                }
            }
            else
            {
                triggerGroupsAdd.IsEnabled = true;
                triggerAdd.IsEnabled = false;
                triggerGroupsEdit.IsEnabled = false;
                triggerGroupsRemove.IsEnabled = false;
                triggerGroupsAddSelected.IsEnabled = false;
                triggerAdd.IsEnabled = false;
                triggerEdit.IsEnabled = false;
                triggerRemove.IsEnabled = false;
            }
        }
        #endregion
        #region Functions
        private void PlaySound(string soundid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                Stream soundfile = new System.IO.MemoryStream();
                db.FileStorage.Download($"$/triggersounds/{soundid}", soundfile);
                SoundPlayer test = new SoundPlayer(soundfile);
                test.Stream.Position = 0;
                test.Play();
            }
        }
        private void UpdateView()
        {
            UpdateListView();
            UpdateTriggerView();
            Refresh_Categories();
        }
        public void UpdateTriggerView()
        {                
            //root of the Trigger Tree
            treeView = new List<TreeViewModel>();
            tv = new TreeViewModel("All Triggers");
            treeView.Add(tv);
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                //Get Activated Triggers
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                foreach (var doc in col.FindAll())
                {
                    if (doc.Parent == 0)
                    {
                        TreeViewModel rTree = new TreeViewModel(doc.TriggerGroupName)
                        {
                            Type = "triggergroup",
                            Id = doc.Id
                        };
                        if (doc.triggers.Count > 0)
                        {
                            foreach (Int32 item in doc.triggers)
                            {
                                Boolean isChecked = false;
                                var collection = db.GetCollection<Trigger>("triggers");
                                var getTrigger = collection.FindById(item);
                                if (currentSelection != null)
                                {
                                    CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
                                    if (selectedCharacter.Triggers.Contains(item))
                                    {
                                        isChecked = true;
                                    }
                                }
                                TreeViewModel newChildBranch = new TreeViewModel(getTrigger.Name)
                                {
                                    Type = "trigger"
                                };
                                newChildBranch.IsChecked = isChecked;
                                newChildBranch.TriggerAdded += TriggerAdded_TreeViewModel;
                                newChildBranch.TriggerRemoved += TriggerRemoved_TreeViewModel;
                                rTree.Children.Add(newChildBranch);
                                rTree.VerifyCheckedState();
                            }
                        }
                        if (doc.Children.Count > 0)
                        {
                            tv.Children.Add(BuildTree(doc));
                        }
                        else
                        {
                            tv.Children.Add(rTree);
                        }
                    }                  
                }
            }
            //Build Tree
            tv.Initialize();
            tv.VerifyCheckedState();
            treeViewTriggers.ItemsSource = treeView;
        }
        public void UpdateListView()
        {
            characterProfiles.Clear();
            activeTriggers.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                foreach (var doc in col.FindAll())
                {
                    characterProfiles.Add(doc);
                    foreach (int triggerID in doc.Triggers)
                    {
                        var triggerCollection = db.GetCollection<Trigger>("triggers");
                        Trigger addedTrigger = (triggerCollection.FindById(triggerID));
                        Boolean keyExists = false;
                        foreach (KeyValuePair<Trigger, ArrayList> entry in activeTriggers)
                        {
                            if (entry.Key.Name == addedTrigger.Name)
                            {
                                entry.Value.Add(doc);
                                keyExists = true;
                            }
                        }
                        if (!keyExists)
                        {
                            ArrayList newList = new ArrayList
                            {
                                doc
                            };
                            activeTriggers.Add(addedTrigger,newList);
                        }
                    }
                }
            }
            listviewCharacters.ItemsSource = characterProfiles;
            if (listviewCharacters.SelectedItem == null && listviewCharacters.Items.Count > 0)
            {
                if(currentSelection == null)
                {
                    listviewCharacters.SelectedIndex = 0;
                    currentSelection = ((CharacterProfile)listviewCharacters.Items[0]).ProfileName;
                }
                else
                {
                    int count = listviewCharacters.Items.Count;
                    while (count > 0)
                    {
                        CharacterProfile foo = (CharacterProfile)listviewCharacters.Items[count - 1];
                        if (foo.ProfileName == currentSelection)
                        {
                            listviewCharacters.SelectedIndex = (count - 1);
                            currentSelection = ((CharacterProfile)listviewCharacters.SelectedItem).ProfileName;
                            break;
                        }
                        --count;
                    }
                }
            }
            
        }
        #endregion
        #region Overlays
        private void TextOverlayAddRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayTextEditor newOverlayEditor = new OverlayTextEditor();
            newOverlayEditor.Show();
        }
        private void TimerOverlayAddRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayTimerEditor newOverlayEditor = new OverlayTimerEditor();
            newOverlayEditor.Show();
        }
        private void TextOverlayProperties_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as RibbonSplitMenuItem).Name;            
            OverlayTextEditor newOverlayEditor = new OverlayTextEditor(overlayname);
            newOverlayEditor.Show();
        }
        private void TimerOverlayProperties_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as RibbonSplitMenuItem).Name;
            OverlayTimerEditor newOverlayEditor = new OverlayTimerEditor(overlayname);
            newOverlayEditor.Show();
        }
        private void TextOverlayDelete_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as RibbonSplitMenuItem).Name;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                overlaytexts.Delete(Query.EQ("Name", overlayname));
            }
            //Kill current overlay if running
            foreach(OverlayTextWindow overlay in textWindows)
            {
                if(overlay.Name == overlayname)
                {
                    textWindows.Remove(overlay);
                    overlay.Close();
                }
            }
            OverlayText_Refresh();
        }
        private void TimerOverlayDelete_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as RibbonSplitMenuItem).Name;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                overlaytimers.Delete(Query.EQ("Name", overlayname));
            }
            //Kill current overlay if running
            foreach (OverlayTimerWindow overlay in timerWindows)
            {
                if (overlay.Name == overlayname)
                {
                    timerWindows.Remove(overlay);
                    overlay.Close();
                }
            }
            OverlayTimer_Refresh();
        }
        private void NotifySaveOverlay_OverlayTextEditor(object sender, RoutedEventArgs e)
        {
            OverlayText_Refresh();
        }        
        #endregion

        public void Refresh_Categories()
        {
            categoryWrapper.CategoryList.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                foreach (var category in categoriescol.FindAll())
                {
                    categoryWrapper.CategoryList.Add(category);
                    category.AvailableTextOverlays = availoverlaytexts;
                    category.AvailableTimerOverlays = availoverlaytimers;
                    categoriescol.Update(category);
                    categorycollection.Add(category);
                }
            }
            tabcontrolCategory.DataContext = categoryWrapper;
            tabcontrolCategory.SelectedIndex = 0;
        }
        private void RibbonMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ribbonMain.SelectedIndex == 3)
            {
                categoriesDocument.IsSelected = true;
            }
            else
            {
                availabletriggers.IsSelected = true;
            }
        }

        private void Categories_IsSelectedChanged(object sender, EventArgs e)
        {
            if(categoriesDocument.IsSelected == true)
            {
                ribbonMain.SelectedIndex = 3;
            }
        }

        private void Availabletriggers_IsSelectedChanged(object sender, EventArgs e)
        {
            if(availabletriggers.IsSelected == true)
            {
                ribbonMain.SelectedIndex = 0;
            }
        }

        private void TabcontrolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ExpanderText_Expanded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
