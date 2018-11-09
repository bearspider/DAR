using LiteDB;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;

namespace DAR
{
    /// <summary>
    /// Global Variables that are used throughout the program
    /// </summary>
    public class GlobalVariables
    {
        public static string defaultPath = @"C:\EQAudioTriggers";
        public static string defaultDB = $"{defaultPath}\\eqtriggers.db";
        public static string eqRegex = @"\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\](?<stringToMatch>.*)";
        public static string pathRegex = @"(?<logdir>.*\\)(?<logname>eqlog_.*\.txt)";
    }
    #region Converters
    /// <summary>
    /// Convert a Boolean value to icon which will display the monitoring status of the character
    /// </summary>
    /// <returns>Returns the image to be used on the form, Monitoring or not monitoring</returns>
    public class MonitoringStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if ((Boolean)value)
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
    /// <summary>
    /// Convert a Boolean value to icon which will display the status of if a log file exists for a character
    /// </summary>
    /// <returns>When false, there is not a logfile to monitor and an icon will be displayed indicating the missing file</returns>
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
    /// <summary>
    /// Convert a Boolean value which monitors the logfile of the CharacterProfile.  Complements MonitoringStatusConverter by putting a red border 
    /// around the Character
    /// </summary>
    /// <returns>When false, there is not a logfile to monitor and a red border will be displayed indicating the missing file</returns>
    public class BorderConverter : IValueConverter
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
                rString = "Red";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RadioOverride : IValueConverter
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
                rString = "";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Convert a Boolean value to icon which will display the whether a category is DEFAULT
    /// </summary>
    /// <returns>When true, an icon will displayed which indicates the DEFAULT category</returns>
    public class DefaultCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if ((Boolean)value)
            {
                rString = "Images/Paomedia-Small-N-Flat-Sign-check.ico";
            }
            else
            {
                rString = "";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Convert a Boolean value to icon which will display the monitoring status of the Pushback Monitor
    /// </summary>
    /// <returns>When true, a green icon is displayed.  When false, a red icon is displayed</returns>
    public class PushbackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if ((Boolean)value)
            {
                rString = "Images/Hopstarter-Soft-Scraps-Button-Blank-Green.ico";
            }
            else
            {
                rString = "Images/Hopstarter-Soft-Scraps-Button-Blank-Red.ico";
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
        private CharacterProfile currentprofile;
        private String selectedcategory;
        private int categoryindex = 0;
        private Boolean pushbackToggle = false;
        private object _itemsLock = new object();
        private object _timersLock = new object();
        private object _textsLock = new object();
        private object _categoryLock = new object();
        //basicregex should be used for all character monitoring
        Regex basicregex = new Regex(@"\[(?<EQTIME>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\]\s(?<DATA>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //spellregex sould ONLY be used for the Pushback Monitor Feature
        Regex spellregex = new Regex(@"\[(?<EQTIME>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\]\s(?<CHARACTER>\w+)\sbegins\sto\scast\sa\sspell\.\s\<(?<SPELL>.+)\>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Dictionary<String, FileSystemWatcher> watchers = new Dictionary<String, FileSystemWatcher>();
        private Dictionary<Trigger, ArrayList> activeTriggers = new Dictionary<Trigger, ArrayList>();
        private ObservableCollection<OverlayTextWindow> textWindows = new ObservableCollection<OverlayTextWindow>();
        private ObservableCollection<OverlayTimerWindow> timerWindows = new ObservableCollection<OverlayTimerWindow>();
        private ObservableCollection<OverlayText> availoverlaytexts = new ObservableCollection<OverlayText>();
        private ObservableCollection<OverlayTimer> availoverlaytimers = new ObservableCollection<OverlayTimer>();
        private ObservableCollection<Category> categorycollection = new ObservableCollection<Category>();
        private ObservableCollection<CategoryWrapper> CategoryTab = new ObservableCollection<CategoryWrapper>();
        //These collections are used for the Pushback Monitor feature
        private ObservableCollection<Pushback> pushbackList = new ObservableCollection<Pushback>();
        private Dictionary<String, Tuple<String, Double>> masterpushbacklist = new Dictionary<String, Tuple<String, Double>>();
        private Dictionary<String,Tuple<String,Double>> masterpushuplist = new Dictionary<String, Tuple<String, Double>>();
        //Trigger Clipboard
        private int triggerclipboard = 0;
        private int triggergroupclipboard = 0;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            //Load settings
            /*Add Code*/

            //Initialize pushback monitor
            image_pushbackindicator.DataContext = pushbackToggle;
            datagrid_pushback.ItemsSource = pushbackList;

            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = Directory.Exists(GlobalVariables.defaultPath);
            if (!mainPath)
            {
                Directory.CreateDirectory(GlobalVariables.defaultPath);
            }

            //Load the Pushback and Pushup data from CSV files.  If the CSV files do not exist, they will be downloaded.
            InitializePushback();
            InitializePushup();

            //Initialize thread bindings
            BindingOperations.EnableCollectionSynchronization(pushbackList, _itemsLock);
            BindingOperations.EnableCollectionSynchronization(timerWindows, _timersLock);
            BindingOperations.EnableCollectionSynchronization(textWindows, _textsLock);
            BindingOperations.EnableCollectionSynchronization(categorycollection, _categoryLock);

            //Prep Views
            UpdateListView();
            UpdateTriggerView();
            OverlayText_Refresh();
            OverlayTimer_Refresh();

            //Deploy Overlays
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<CharacterProfile> dbcharacterProfiles = db.GetCollection<CharacterProfile>("profiles");
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                Trigger testtrigger = triggers.FindById(1);
                //Check if no overlays or character profiles exist.  If none exist, prompt the editors to create the first entries.
                if (availoverlaytexts.Count<OverlayText>() == 0)
                {
                    OverlayTextEditor newOverlayEditor = new OverlayTextEditor();
                    newOverlayEditor.Topmost = true;
                    newOverlayEditor.Show();
                }
                if (availoverlaytimers.Count<OverlayTimer>() == 0)
                {
                    OverlayTimerEditor newOverlayEditor = new OverlayTimerEditor();
                    newOverlayEditor.Topmost = true;
                    newOverlayEditor.Show();
                }
                if (characterProfiles.Count<CharacterProfile>() == 0)
                {
                    ProfileEditor newProfile = new ProfileEditor();
                    newProfile.Topmost = true;
                    newProfile.Show();
                    UpdateView();
                }
                if (settings.Count() == 0)
                {
                    //populate default settings
                    DefaultSettings();
                }
                //If no categories exist(Blank Database), create a default category. DEFAULT category is immutable.
                IEnumerable<Category> availcategories = categoriescol.FindAll();
                if (availcategories.Count<Category>() == 0)
                {
                    Category defaultcategory = new Category();
                    defaultcategory.DefaultCategory = true;
                    foreach (var profile in dbcharacterProfiles.FindAll())
                    {
                        CharacterOverride newoverride = new CharacterOverride();
                        newoverride.ProfileName = profile.ProfileName;
                        defaultcategory.CharacterOverrides.Add(newoverride);
                    }
                    defaultcategory.AvailableTimerOverlays = availoverlaytimers;
                    defaultcategory.AvailableTextOverlays = availoverlaytexts;
                    categoriescol.Insert(defaultcategory);
                    availcategories = categoriescol.FindAll();
                }
                //Deploy all text overlays
                foreach (var overlay in overlaytexts.FindAll())
                {
                    OverlayTextWindow newWindow = new OverlayTextWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    textWindows.Add(newWindow);
                    //newWindow.AddTrigger(testtrigger);
                    newWindow.Show();
                }
                //Deply all timer overlays
                foreach (var overlay in overlaytimers.FindAll())
                {
                    OverlayTimerWindow newWindow = new OverlayTimerWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    timerWindows.Add(newWindow);
                    //newWindow.Add(testtrigger);
                    newWindow.Show();
                }
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
                if (File.Exists(character.LogFile) && character.Monitor)
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
        //Re-adjust the ribbon size if the main window is resized
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ribbonMain.Width = ActualWidth;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                ///If a charcter is set to default monitor on startup and monitoring was stopped during the program.
                ///reflag the monitor Boolean in the database for the character.
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
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingswindow = new Settings();
            settingswindow.Show();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About aboutwindow = new About();
            aboutwindow.Show();
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
                    foreach (var trigger in triggers.FindAll())
                    {
                        if (trigger.Profiles.Contains(profileid))
                        {
                            trigger.Profiles.Remove(profileid);
                            triggers.Update(trigger);
                        }
                    }
                    foreach (var category in categories.FindAll())
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
                currentprofile = selectedCharacter;
                currentSelection = selectedCharacter.ProfileName;
                UpdateTriggerView();
                Refresh_Categories();
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
        private void MenuItemCharEdit_Click(object sender, RoutedEventArgs e)
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
        private void MenuItemCharDelete_Click(object sender, RoutedEventArgs e)
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
                    foreach (var trigger in triggers.FindAll())
                    {
                        if (trigger.Profiles.Contains(profileid))
                        {
                            trigger.Profiles.Remove(profileid);
                            triggers.Update(trigger);
                        }
                    }
                    foreach (var category in categories.FindAll())
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
        private void MenuItemStartMonitor_Click(object sender, RoutedEventArgs e)
        {
            CharacterProfile selected = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                CharacterProfile currentProfile = colProfiles.FindById(selected.Id);
                currentProfile.Monitor = true;
                colProfiles.Update(currentProfile);
                if (currentProfile.Monitor)
                {
                    MonitorCharacter(currentProfile);
                }
            }
            UpdateListView();
        }
        private void MenuItemStopMonitor_Click(object sender, RoutedEventArgs e)
        {
            CharacterProfile selected = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                CharacterProfile currentProfile = colProfiles.FindById(selected.Id);
                currentProfile.Monitor = false;
                colProfiles.Update(currentProfile);
            }
            UpdateListView();
        }
        private void ListviewCharacters_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            CharacterProfile selected = (CharacterProfile)listviewCharacters.SelectedItem;
            if(selected.Monitor)
            {
                cmStopMonitor.IsEnabled = true;
                cmStartMonitor.IsEnabled = false;
            }
            else
            {
                cmStopMonitor.IsEnabled = false;
                cmStartMonitor.IsEnabled = true;
            }
        }
        #endregion
        #region Monitoring
        private void InitializePushback()
        {
            if (!File.Exists(GlobalVariables.defaultPath + @"\pushback-list.csv"))
            {
                //Get file from github
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushback.csv", GlobalVariables.defaultPath + @"\pushback-list.csv");
                }

            }
            using (var reader = new StreamReader(@"C:\eqaudiotriggers\pushback-list.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    String[] vars = line.Split(',');
                    Tuple<String,Double> entry = new Tuple<string, double>(vars[1],Convert.ToDouble(vars[2]));
                    masterpushbacklist.Add(vars[0],entry);
                }
            }
        }
        private void InitializePushup()
        {
            if (!File.Exists(GlobalVariables.defaultPath + @"\pushup-list.csv"))
            {
                //Get file from github
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushup.csv", GlobalVariables.defaultPath + @"\pushup-list.csv");
                }
            }
            using (var reader = new StreamReader(@"C:\eqaudiotriggers\pushup-list.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    String[] vars = line.Split(',');
                    Tuple<String, Double> entry = new Tuple<string, double>(vars[1], Convert.ToDouble(vars[2]));
                    masterpushuplist.Add(vars[0], entry);
                }
            }
        }
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
                                String[] delimiter = new string[] { "\r\n" };
                                String[] lines = capturedLine.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string line in lines)
                                {
                                    using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                                    {
                                        var triggerCollection = db.GetCollection<Trigger>("triggers");
                                        foreach (var doc in triggerCollection.FindAll())
                                        {
                                            MatchCollection matches = Regex.Matches(line, doc.SearchText, RegexOptions.IgnoreCase);
                                            if (matches.Count > 0)
                                            {
                                                foreach (Match tMatch in matches)
                                                {
                                                    if (doc.AudioSettings.AudioType == "tts")
                                                    { character.Speak(doc.AudioSettings.TTS); }
                                                    if (doc.AudioSettings.AudioType == "file")
                                                    { PlaySound(doc.AudioSettings.SoundFileId); }
                                                    //Add Timer code
                                                    Category triggeredcategory = categorycollection.Single<Category>(i => i.Id == doc.TriggerCategory);
                                                    if (doc.Displaytext != null)
                                                    {
                                                        //find the text overlay from category
                                                        Dispatcher.BeginInvoke((Action)(() =>
                                                        {
                                                            OverlayTextWindow otw = textWindows.Single<OverlayTextWindow>(i => i.Name == triggeredcategory.TextOverlay);
                                                            otw.AddTrigger(doc);
                                                            otw.DataContext = otw;
                                                        }));

                                                    }
                                                    lock (_timersLock)
                                                    {

                                                        switch (doc.TimerType)
                                                        {
                                                            case "Timer(Count Down)":
                                                                Dispatcher.BeginInvoke((Action)(() =>
                                                                {

                                                                    OverlayTimerWindow otw = timerWindows.Single<OverlayTimerWindow>(i => i.Name == triggeredcategory.TimerOverlay);
                                                                    otw.AddTimer(doc.TimerName, doc.TimerDuration, false);
                                                                    otw.DataContext = otw;
                                                                }));
                                                                break;
                                                            case "Stopwatch(Count Up)":
                                                                Dispatcher.BeginInvoke((Action)(() =>
                                                                {
                                                                    OverlayTimerWindow otw = timerWindows.Single<OverlayTimerWindow>(i => i.Name == triggeredcategory.TimerOverlay);
                                                                    otw.AddTimer(doc.TimerName, doc.TimerDuration, true);
                                                                    (timerWindows.Single<OverlayTimerWindow>(i => i.Name == triggeredcategory.TimerOverlay)).DataContext = otw;
                                                                }));
                                                                break;
                                                            case "Repeating Timer":
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (pushbackToggle)
                                        {
                                            Match pushmatch = spellregex.Match(line);
                                            foreach (KeyValuePair<String,Tuple<String,Double>> spell in masterpushbacklist)
                                            {
                                                MatchCollection matches = Regex.Matches(pushmatch.Groups["SPELL"].Value, spell.Value.Item1, RegexOptions.IgnoreCase);
                                                if (matches.Count > 0)
                                                {
                                                    foreach (Match spellmatch in matches)
                                                    {
                                                        Pushback pushback = new Pushback
                                                        {
                                                            Character = pushmatch.Groups["CHARACTER"].Value,
                                                            PushType = "Pushback",
                                                            Spell = pushmatch.Groups["SPELL"].Value,
                                                            FromCharacter = character.ProfileName,
                                                            Distance = spell.Value.Item2
                                                        };
                                                        lock (_itemsLock)
                                                        {
                                                            pushbackList.Add(pushback);
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            foreach (KeyValuePair<String,Tuple<String,Double>> spell in masterpushuplist)
                                            {
                                                MatchCollection matches = Regex.Matches(pushmatch.Groups["SPELL"].Value, spell.Value.Item1, RegexOptions.IgnoreCase);
                                                if (matches.Count > 0)
                                                {
                                                    foreach (Match spellmatch in matches)
                                                    {
                                                    Pushback pushback = new Pushback
                                                    {
                                                        Character = pushmatch.Groups["CHARACTER"].Value,
                                                        PushType = "Pushup",
                                                        Spell = pushmatch.Groups["SPELL"].Value,
                                                        FromCharacter = character.ProfileName,
                                                        Distance = spell.Value.Item2
                                                    };
                                                        lock (_itemsLock)
                                                        {
                                                            pushbackList.Add(pushback);
                                                        }
                                                    }
                                                    break;
                                                }
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
            foreach (KeyValuePair<Trigger, ArrayList> entry in activeTriggers)
            {
                Trigger toCompare = entry.Key;
                MatchCollection matches = Regex.Matches(lastline, toCompare.SearchText, RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    foreach (CharacterProfile character in entry.Value)
                    {
                        if (character.Monitor)
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
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteTrigger(root.Name);
                UpdateView();
            }
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
        private void Availabletriggers_IsSelectedChanged(object sender, EventArgs e)
        {
            if (availabletriggers.IsSelected == true)
            {
                ribbonMain.SelectedIndex = 0;
            }
        }
        private void DeleteTrigger(String triggername)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<Trigger>("triggers");
                Trigger deadtrigger = col.FindOne(Query.EQ("Name", triggername));
                var triggergroup = db.GetCollection<TriggerGroup>("triggergroups");
                var getGroup = triggergroup.FindById(deadtrigger.Parent);
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var profiles = colProfiles.FindAll();
                foreach (CharacterProfile profile in profiles)
                {
                    profile.Triggers.Remove(deadtrigger.Id);
                    colProfiles.Update(profile);
                }
                getGroup.RemoveTrigger(deadtrigger.Id);
                triggergroup.Update(getGroup);
                col.Delete(deadtrigger.Id);
            }
        }
        private void DeleteTrigger(int triggerid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<Trigger>("triggers");
                Trigger deadtrigger = col.FindById(triggerid);
                var triggergroup = db.GetCollection<TriggerGroup>("triggergroups");
                var getGroup = triggergroup.FindById(deadtrigger.Parent);
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var profiles = colProfiles.FindAll();
                foreach (CharacterProfile profile in profiles)
                {
                    profile.Triggers.Remove(triggerid);
                    colProfiles.Update(profile);
                }
                getGroup.RemoveTrigger(triggerid);
                triggergroup.Update(getGroup);
                col.Delete(triggerid);
            }
        }
        private void CopyTrigger(int triggerid, int newgroupid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var triggerCollection = db.GetCollection<Trigger>("triggers");
                var triggergroupCollection = db.GetCollection<TriggerGroup>("triggergroups");
                Trigger basetrigger = triggerCollection.FindById(triggerid);
                TriggerGroup basegroup = triggergroupCollection.FindById(newgroupid);
                basetrigger.Id = 0;
                basetrigger.Name = basetrigger.Name + "-Copy";
                basetrigger.Parent = newgroupid;
                BsonValue newid = triggerCollection.Insert(basetrigger);
                basegroup.Triggers.Add(newid);
                basegroup.Triggers.Remove(triggerid);
                triggergroupCollection.Update(basegroup);
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
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteTriggerGroup(root.Name);
                UpdateView();
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
                TriggerGroup result = col.FindOne(Query.And(Query.EQ("TriggerGroupName", root.Name), Query.EQ("_id", root.Id)));
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
        private void DeleteTriggerGroup(String groupname)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                TriggerGroup deadgroup = col.FindOne(Query.EQ("TriggerGroupName", groupname));
                var dbid = deadgroup.Id;
                var childContains = col.FindAll().Where(x => x.children.Contains(dbid));
                //Delete all triggers associated with the group
                var triggers = deadgroup.Triggers;                
                foreach (int triggerid in triggers)
                {
                    DeleteTrigger(triggerid);
                }
                //If child group, remove child from parent
                foreach (var child in childContains)
                {
                    child.Children.Remove(dbid);
                    col.Update(child);
                }
                //if parent group, remove children
                foreach (int childgroup in deadgroup.Children)
                {
                    DeleteTriggerGroup(childgroup);
                }
                col.Delete(dbid);
            }
        }
        private void DeleteTriggerGroup(int groupid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                TriggerGroup deadgroup = col.FindById(groupid);
                var childContains = col.FindAll().Where(x => x.children.Contains(groupid));
                //Delete all triggers associated with the group
                var triggers = deadgroup.Triggers;                
                foreach (int triggerid in triggers)
                {
                    DeleteTrigger(triggerid);
                }
                //If child group, remove child from parent
                foreach (var child in childContains)
                {
                    child.Children.Remove(groupid);
                    col.Update(child);
                }
                //If Parent group, remove children
                foreach(int childgroup in deadgroup.Children)
                {
                    DeleteTriggerGroup(childgroup);
                }
                col.Delete(groupid);
            }
        }
        private void CopyTriggerGroup(int copyfrom, int parent)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var triggergroupCollection = db.GetCollection<TriggerGroup>("triggergroups");
                TriggerGroup basegroup = triggergroupCollection.FindById(copyfrom);                
                TriggerGroup newgroup = new TriggerGroup();
                newgroup.DefaultEnabled = basegroup.DefaultEnabled;
                newgroup.TriggerGroupName = basegroup.TriggerGroupName + "-Copy";
                newgroup.Id = 0;
                newgroup.Parent = parent;
                BsonValue newgid = triggergroupCollection.Insert(newgroup);
                if(basegroup.Triggers.Count > 0)
                {
                    foreach(int triggerid in basegroup.Triggers)
                    {
                        CopyTrigger(triggerid, newgid);
                    }
                }
                if(basegroup.Children.Count > 0)
                {
                    foreach(int child in basegroup.Children)
                    {
                        CopyTriggerGroup(child, newgid);
                    }
                }
                if (parent != 0)
                {
                    TriggerGroup parentgroup = triggergroupCollection.FindById(parent);
                    parentgroup.AddChild(newgid);
                    triggergroupCollection.Update(parentgroup);
                }
            }
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
        private void DefaultSettings()
        {
            Setting mastervolume = new Setting
            {
                Name = "MasterVolume",
                Value = "100"
            };
            Setting update = new Setting
            {
                Name = "ApplicationUpdate",
                Value = "true"
            };
            Setting enablesound = new Setting
            {
                Name = "EnableSound",
                Value = "true"
            };
            Setting enabletext = new Setting
            {
                Name = "EnableText",
                Value = "true"
            };
            Setting enabletimers = new Setting
            {
                Name = "EnableTimers",
                Value = "true"
            };
            Setting minimize = new Setting
            {
                Name = "Minimize",
                Value = "false"
            };
            Setting stoptrigger = new Setting
            {
                Name = "StopTriggerSearch",
                Value = "false"
            };
            Setting displaymatchlog = new Setting
            {
                Name = "DisplayMatchLog",
                Value = "true"
            };
            Setting maxlogentry = new Setting
            {
                Name = "MaxLogEntry",
                Value = "100"
            };
            Setting logmatchtofile = new Setting
            {
                Name = "LogMatchesToFile",
                Value = "false"
            };
            Setting logmatchfilename = new Setting
            {
                Name = "LogMatchFilename",
                Value = ""
            };
            Setting clipboard = new Setting
            {
                Name = "Clipboard",
                Value = "{C}"
            };
            Setting eqfolder = new Setting
            {
                Name = "EQFolder",
                Value = @"C:\EQ"
            };
            Setting importedmedia = new Setting
            {
                Name = "ImportedMediaFolder",
                Value = @"C:\EQAudioTriggers\ImportedMedia"
            };
            Setting datafolder = new Setting
            {
                Name = "DataFolder",
                Value = @"C:\EQAudioTriggers"
            };
            Setting enablesharing = new Setting
            {
                Name = "SharingEnabled",
                Value = "true"
            };
            Setting enableincoming = new Setting
            {
                Name = "EnableIncomingTriggers",
                Value = "true"
            };
            Setting acceptfrom = new Setting
            {
                Name = "AcceptInvitationsFrom",
                Value = "2"
            };
            Setting mergefrom = new Setting
            {
                Name = "MergeFrom",
                Value = "2"
            };
            Setting senderlist = new Setting
            {
                Name = "TrustedSenderList",
                Value = ""
            };
            Setting logarchive = new Setting
            {
                Name = "LogArchiveFolder",
                Value = @"C:\EQAudioTriggers\Archive"
            };
            Setting autoarchive = new Setting
            {
                Name = "AutoArchive",
                Value = "true"
            };
            Setting compress = new Setting
            {
                Name = "CompressArchive",
                Value = "true"
            };
            Setting archivemethod = new Setting
            {
                Name = "ArchiveMethod",
                Value = "Size Threshold"
            };
            Setting logsize = new Setting
            {
                Name = "LogSize",
                Value = "50"
            };
            Setting autodelete = new Setting
            {
                Name = "AutoDelete",
                Value = "true"
            };
            Setting deletearchive = new Setting
            {
                Name = "DeleteArchives",
                Value = "90"
            };
            Setting shareuri = new Setting
            {
                Name = "ShareServiceURI",
                Value = @"http:\\shareservice.com"
            };
            Setting reference = new Setting
            {
                Name = "Reference",
                Value = "You"
            };
            Setting enabledebug = new Setting
            {
                Name = "EnableDebug",
                Value = "false"
            };
            Setting archiveschedule = new Setting
            {
                Name = "ArchiveSchedule",
                Value = ""
            };
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                settings.Insert(mastervolume);
                settings.Insert(update);
                settings.Insert(enablesound);
                settings.Insert(enabletext);
                settings.Insert(enabletimers);
                settings.Insert(minimize);
                settings.Insert(stoptrigger);
                settings.Insert(displaymatchlog);
                settings.Insert(maxlogentry);
                settings.Insert(logmatchtofile);
                settings.Insert(logmatchfilename);
                settings.Insert(clipboard);
                settings.Insert(eqfolder);
                settings.Insert(importedmedia);
                settings.Insert(datafolder);
                settings.Insert(enablesharing);
                settings.Insert(enableincoming);
                settings.Insert(acceptfrom);
                settings.Insert(mergefrom);
                settings.Insert(senderlist);
                settings.Insert(logarchive);
                settings.Insert(autoarchive);
                settings.Insert(compress);
                settings.Insert(archivemethod);
                settings.Insert(logsize);
                settings.Insert(autodelete);
                settings.Insert(deletearchive);
                settings.Insert(shareuri);
                settings.Insert(reference);
                settings.Insert(enabledebug);
                settings.Insert(archiveschedule);
            }
        }
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
                    //Check if the file exists and mark the fileexists boolean
                    if (File.Exists(doc.LogFile))
                    {
                        doc.FileExists = true;
                    }
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
                            activeTriggers.Add(addedTrigger, newList);
                        }
                    }
                }
            }
            listviewCharacters.ItemsSource = characterProfiles;
            if (listviewCharacters.SelectedItem == null && listviewCharacters.Items.Count > 0)
            {
                if (currentSelection == null)
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
            foreach (OverlayTextWindow overlay in textWindows)
            {
                if (overlay.Name == overlayname)
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
        public void OverlayTimer_Refresh()
        {
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
            availoverlaytexts.Clear();
            for (int i = ribbongroupTextOverlays.Items.Count; i > 1; i--)
            {
                ribbongroupTextOverlays.Items.RemoveAt(i - 1);
            }
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                foreach (var overlay in overlaytexts.FindAll())
                {
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
        #region Categories
        public void Refresh_Categories()
        {
            CategoryTab.Clear();
            categorycollection.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                foreach (var category in categoriescol.FindAll())
                {
                    category.AvailableTextOverlays = availoverlaytexts;
                    category.AvailableTimerOverlays = availoverlaytimers;
                    categoriescol.Update(category);
                    categorycollection.Add(category);

                    CategoryWrapper newcat = new CategoryWrapper();
                    newcat.CategoryItem = category;
                    newcat.OverlayTexts = availoverlaytexts;
                    newcat.OverlayTimers = availoverlaytimers;
                    newcat.SelectedOverride = (category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First();
                    CategoryTab.Add(newcat);
                }
            }
            tabcontrolCategory.DataContext = CategoryTab;
            tabcontrolCategory.SelectedIndex = categoryindex;
        }
        private void Categories_IsSelectedChanged(object sender, EventArgs e)
        {
            if (categoriesDocument.IsSelected == true)
            {
                ribbonMain.SelectedIndex = 3;
                Refresh_Categories();
            }
        }
        private void RibbonMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ribbonMain.SelectedIndex == 3)
            {
                categoriesDocument.IsSelected = true;
                Refresh_Categories();
            }
            else
            {
                availabletriggers.IsSelected = true;
            }
        }
        private void TabcontrolCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currenttab = (sender as System.Windows.Controls.TabControl);
            CategoryWrapper currentcategory = (CategoryWrapper)currenttab.SelectedItem;
            if (currentcategory != null)
            {
                categoryindex = currenttab.SelectedIndex;
                selectedcategory = currentcategory.CategoryItem.Name;
            }
        }
        private void CategoryName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newname = (sender as TextBox).Text;
            if (newname != "" && selectedcategory != "Default")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.Name = newname;
                    categoriescol.Update(category);
                    selectedcategory = newname;
                }
            }
        }
        private void CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            NewCategory addcategory = new NewCategory();
            if (addcategory.ShowDialog() == true)
            {
                Category newcategory = new Category();
                newcategory.Name = addcategory.textboxName.Text;
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    LiteCollection<CharacterProfile> profilecol = db.GetCollection<CharacterProfile>("profiles");
                    foreach (var profile in profilecol.FindAll())
                    {
                        CharacterOverride newoverride = new CharacterOverride();
                        newoverride.ProfileName = profile.ProfileName;
                        newcategory.CharacterOverrides.Add(newoverride);
                    }
                    categoriescol.Insert(newcategory);
                }
            }
            Refresh_Categories();
        }
        private void CategoryRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedcategory != "Default")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    LiteCollection<Trigger> triggerscol = db.GetCollection<Trigger>("triggers");
                    Category category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    Category defaultcategory = categoriescol.FindOne(Query.EQ("Name", "Default"));
                    //If a trigger is in this category, reset it's category to default
                    foreach (var trigger in triggerscol.FindAll())
                    {
                        if (trigger.TriggerCategory == category.Id)
                        {
                            trigger.TriggerCategory = defaultcategory.id;
                            triggerscol.Update(trigger);
                        }
                    }
                    categoriescol.Delete(Query.EQ("Name", selectedcategory));
                    selectedcategory = "Default";
                    categoryindex = 0;
                    tabcontrolCategory.SelectedIndex = categoryindex;
                }
            }
        }
        private void CategoryName_LostFocus(object sender, RoutedEventArgs e)
        {
            int tabindex = categoryindex;
            Refresh_Categories();
            tabcontrolCategory.SelectedIndex = tabindex;
        }
        private void CategoryTextOverlay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OverlayText selection = (OverlayText)(sender as ComboBox).SelectedItem;
            if (selection != null)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextOverlay = selection.Name;
                    categoriescol.Update(category);
                }
            }

        }
        private void ClrpckrText_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextFontColor = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryTimerOverlay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OverlayTimer selection = (OverlayTimer)(sender as ComboBox).SelectedItem;
            if (selection != null)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerOverlay = selection.Name;
                    categoriescol.Update(category);
                }
            }
        }
        private void ClrpckrTimerText_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerFontColor = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void ClrpckrTimerBar_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerBarColor = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void ComboOverrideTextOverlay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OverlayText selection = (OverlayText)(sender as ComboBox).SelectedItem;
            if (selection != null)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlay = selection.Name;
                    categoriescol.Update(category);
                }
            }
        }
        private void ClrpckrTextOverride_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorFont = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void ComboTimerOverlayOverride_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OverlayTimer selection = (OverlayTimer)(sender as ComboBox).SelectedItem;
            if (selection != null)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlay = selection.Name;
                    categoriescol.Update(category);
                }
            }
        }
        private void ClrpckrTimerFontOverride_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorFont = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void ClrpckrTimerBarOverride_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            String selection = (sender as Xceed.Wpf.Toolkit.ColorPicker).SelectedColorText;
            if (selection != "")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorBar = selection;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryColor_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextColors = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryColor_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                Console.WriteLine("UserChecked");
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextColors = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TextThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void TimerColors_Checked(object sender, RoutedEventArgs e)
        {
            if ((e.Source as RadioButton).IsMouseOver)
            {
                Boolean status = (Boolean)(sender as RadioButton).IsChecked;
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerColors = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void TimerColors_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerColors = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void TimerThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void TimerThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    category.TimerThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideOverlayCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideOverlayCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorCharacter_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCharacter = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorCharacter_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCharacter = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTextColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorCat_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorCat_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCategory = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorChar_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCharacter = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorChar_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCharacter = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void OverrideTimerColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            if ((e.Source as RadioButton).IsMouseOver)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                    var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                    ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorThis = status;
                    categoriescol.Update(category);
                }
            }
        }
        private void CategoryDefault_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                foreach (var cat in categoriescol.FindAll())
                {
                    if (cat.DefaultCategory)
                    {
                        cat.DefaultCategory = false;
                        categoriescol.Update(cat);
                    }
                }
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.DefaultCategory = true;
                categoriescol.Update(category);
            }
            button_CategorySetDefault.Focus();
            int tabindex = categoryindex;
            Refresh_Categories();
            tabcontrolCategory.SelectedIndex = tabindex;
        }

        #endregion
        #region Pushback Monitor
        private void Button_pushbacktoggle_Click(object sender, RoutedEventArgs e)
        {
            pushbackToggle = !pushbackToggle;
            image_pushbackindicator.DataContext = pushbackToggle;
        }
        #endregion
        #region LogSearch
        private void LogfileSearch_Click(object sender, RoutedEventArgs e)
        {
            LogSearch logsearch = new LogSearch(characterProfiles);
            logsearch.Show();
        }
        #endregion
        #region TriggerContextMenus
        private void MenuItemTriggerCopy_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root.Type == "triggergroup")
            {
                triggerclipboard = 0;
                triggergroupclipboard = root.Id;
            }
            if (root.Type == "trigger")
            {
                triggergroupclipboard = 0;
                triggerclipboard = root.Id;
            }
        }
        private void MenuItemTriggerDelete_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root.Type == "triggergroup")
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteTriggerGroup(root.Name);
                    UpdateView();
                }
            }
            if (root.Type == "trigger")
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteTrigger(root.Name);
                    UpdateView();
                }                
            }
        }
        private void MenuItemTriggerPaste_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var triggerCollection = db.GetCollection<Trigger>("triggers");
                var triggergroupCollection = db.GetCollection<TriggerGroup>("triggergroups");
                //Add new Trigger
                if(root.Type == "trigger")
                {
                    CopyTrigger(triggerclipboard,root.Id);
                }
                //Add new Trigger Group
                if (root.Type == "triggergroup")
                {
                    CopyTriggerGroup(triggergroupclipboard, root.Id);
                }
                if(root.Name == "All Triggers")
                {
                    CopyTriggerGroup(triggergroupclipboard, 0);
                }
            }            
            UpdateTriggerView();
        }
        private void TreeViewTriggers_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root != null)
            {
                if (root.Name == "All Triggers")
                {
                    cmTreeCopy.IsEnabled = false;
                    cmTreeDelete.IsEnabled = false;
                    if (root.Type == "triggergroup")
                    {
                        if (triggergroupclipboard != 0)
                        {
                            cmTreePaste.IsEnabled = true;
                        }
                        else
                        {
                            cmTreePaste.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    cmTreeCopy.IsEnabled = true;
                    cmTreeDelete.IsEnabled = true;
                    if (root.Type == "triggergroup")
                    {
                        if (triggergroupclipboard != 0 || triggerclipboard != 0)
                        {
                            cmTreePaste.IsEnabled = true;
                        }
                        else
                        {
                            cmTreePaste.IsEnabled = false;
                        }
                    }
                    if(root.Type == "trigger")
                    {
                        if (triggerclipboard != 0)
                        {
                            cmTreePaste.IsEnabled = true;
                        }
                        else
                        {
                            cmTreePaste.IsEnabled = false;
                        }
                    }             
                }
            }
            else
            {
                cmTreeCopy.IsEnabled = false;
                cmTreeDelete.IsEnabled = false;
                cmTreePaste.IsEnabled = false;
            }
        }
        #endregion
    }
}
