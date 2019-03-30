using LiteDB;
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
using System.IO.Compression;
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
using System.Windows.Shell;
using Xceed.Wpf.AvalonDock;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public static Regex eqspellRegex = new Regex(@"(\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\])\s((?<character>\w+)\sbegin\s(casting|singing)\s(?<spellname>.*)\.)|(\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\])\s(?<character>\w+)\s(begins\sto\s(cast|sing)\s.*\<(?<spellname>.*)\>)",RegexOptions.Compiled);
        public static string pathRegex = @"(?<logdir>.*\\)(?<logname>eqlog_.*\.txt)";
        public static string pushbackurl = @"https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushback.csv";
        public static string pushupurl = @"https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushup.csv";
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
            String rString = "Images/Knob-Remove-Red-icon.png";
            if ((Boolean)value)
            {
                rString = "Images/Knob-Remove-icon.png";
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
            String rString = "Images/Oxygen-Icons.org-Oxygen-Actions-dialog-ok-apply.ico";
            if (!(Boolean)value)
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
            String rString = "Red";
            if ((Boolean)value)
            {
                rString = "Transparent";
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
        //Main Tree
        private TreeViewModel tv;
        private List<TreeViewModel> treeView;

        //Variables
        private int triggergroupid = 0;
        private int triggerid = 0;
        private String currentSelection;
        private CharacterProfile currentprofile;
        private Category defaultcategory;
        private String selectedcategory;
        private int categoryindex = 0;
        private Boolean pushbackToggle = false;

        //Locks for threads
        private object _itemsLock = new object();
        private object _timersLock = new object();
        private object _textsLock = new object();
        private object _categoryLock = new object();
        private object _triggerLock = new object();
        private readonly SynchronizationContext syncontext;

        //Drag and Drop Merge Triggers
        private Point _lastMouseDown;
        private ObservableCollection<TreeViewModel> mergetreeView = new ObservableCollection<TreeViewModel>();
        private List<TriggerGroup> mergegroups = new List<TriggerGroup>();
        private List<Trigger> mergetriggers = new List<Trigger>();
        private TreeViewModel mergeview;
        private TreeViewModel droptree;
        private int mergetriggercount = 0;

        //Overlays
        private ObservableCollection<OverlayTextWindow> textWindows = new ObservableCollection<OverlayTextWindow>();
        private ObservableCollection<OverlayTimerWindow> timerWindows = new ObservableCollection<OverlayTimerWindow>();
        private ObservableCollection<OverlayText> availoverlaytexts = new ObservableCollection<OverlayText>();
        private ObservableCollection<OverlayTimer> availoverlaytimers = new ObservableCollection<OverlayTimer>();

        //Variable Collections
        private ObservableCollection<Category> categorycollection = new ObservableCollection<Category>();
        private ObservableCollection<CategoryWrapper> CategoryTab = new ObservableCollection<CategoryWrapper>();
        private ObservableCollection<CharacterProfile> characterProfiles = new ObservableCollection<CharacterProfile>();
        private ObservableCollection<ActivatedTrigger> activatedTriggers = new ObservableCollection<ActivatedTrigger>();
        private Dictionary<Trigger, ArrayList> listoftriggers = new Dictionary<Trigger, ArrayList>();
        private List<Setting> programsettings = new List<Setting>();

        //These collections are used for the Pushback Monitor feature
        private ObservableCollection<Pushback> pushbackList = new ObservableCollection<Pushback>();
        private Dictionary<String, Tuple<String, Double>> masterpushbacklist = new Dictionary<String, Tuple<String, Double>>();
        private Dictionary<String, Tuple<String, Double>> masterpushuplist = new Dictionary<String, Tuple<String, Double>>();
        private Dictionary<String, Double> dictpushback = new Dictionary<string, double>();
        private Dictionary<String, Double> dictpushup = new Dictionary<string, double>();
        

        //Trigger Clipboard
        private int triggerclipboard = 0;
        private int triggergroupclipboard = 0;
        private string clipboardtype = "";
        private static string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        private int totallinecount = 0;

        #endregion
        #region Main Program
        public MainWindow()
        {
            InitializeComponent();
            syncontext = SynchronizationContext.Current;
            textblockVersion.Text = version;
            statusbarStatus.DataContext = totallinecount;
            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = Directory.Exists(GlobalVariables.defaultPath);
            if (!mainPath)
            {
                Directory.CreateDirectory(GlobalVariables.defaultPath);
            }

            //Load settings
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                if (settings.Count() == 0)
                {
                    //populate default settings
                    DefaultSettings();
                }
                else
                {
                    foreach(Setting programsetting in settings.FindAll())
                    {
                        programsettings.Add(programsetting);                        
                    }
                }
            }

            //Initialize pushback monitor
            image_pushbackindicator.DataContext = pushbackToggle;
            datagrid_pushback.ItemsSource = pushbackList;
            ICollectionView pushbackview = CollectionViewSource.GetDefaultView(pushbackList);
            pushbackview.SortDescriptions.Add(new SortDescription(("TriggerTime"), ListSortDirection.Descending));
            var pushbackshape = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(pushbackList);
            pushbackshape.IsLiveSorting = true;

            //Set Datagrid for activated triggers
            datagrid_activated.ItemsSource = activatedTriggers;
            ICollectionView triggerview = CollectionViewSource.GetDefaultView(activatedTriggers);
            triggerview.SortDescriptions.Add(new SortDescription(("TriggerTime"), ListSortDirection.Descending));
            var triggershape = (ICollectionViewLiveShaping)CollectionViewSource.GetDefaultView(activatedTriggers);
            triggershape.IsLiveSorting = true;

            //Load the Pushback and Pushup data from CSV files.  If the CSV files do not exist, they will be downloaded.
            InitializePushback();
            //InitializePushup();

            //Initialize thread bindings
            BindingOperations.EnableCollectionSynchronization(pushbackList, _itemsLock);
            BindingOperations.EnableCollectionSynchronization(timerWindows, _timersLock);
            BindingOperations.EnableCollectionSynchronization(textWindows, _textsLock);
            BindingOperations.EnableCollectionSynchronization(categorycollection, _categoryLock);
            BindingOperations.EnableCollectionSynchronization(activatedTriggers, _triggerLock);

            //Load Triggers
            TriggerLoad();

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
                //If no categories exist(Blank Database), create a default category. DEFAULT category is immutable.
                defaultcategory = categoriescol.FindOne(Query.EQ("Name", "Default"));
                if (defaultcategory == null)
                {
                    defaultcategory = new Category();
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
                }
                //Deploy all text overlays
                foreach (var overlay in overlaytexts.FindAll())
                {
                    OverlayTextWindow newWindow = new OverlayTextWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    textWindows.Add(newWindow);
                    newWindow.Show();
                }
                //Deply all timer overlays
                foreach (var overlay in overlaytimers.FindAll())
                {
                    OverlayTimerWindow newWindow = new OverlayTimerWindow();
                    newWindow.SetProperties(overlay);
                    newWindow.ShowInTaskbar = false;
                    timerWindows.Add(newWindow);
                    newWindow.Show();
                }
            }
            StartMonitoring();
        }
        #endregion
        #region Form Functions
        private void CloseProgram()
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
        //Re-adjust the ribbon size if the main window is resized
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ribbonMain.Width = ActualWidth;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CloseProgram();
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
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            CloseProgram();
        }
        #endregion
        #region Character Profiles
        public void AddResetRibbon()
        {
            rbnStopAlerts.Items.Clear();
            rbnResetCounters.Items.Clear();
            foreach (CharacterProfile character in characterProfiles)
            {
                Fluent.Button characterStopAlerts = new Fluent.Button();
                Fluent.Button characterResetCounters = new Fluent.Button();
                characterStopAlerts.Header = character.Name;
                characterResetCounters.Header = character.Name;
                characterStopAlerts.Size = Fluent.RibbonControlSize.Middle;
                characterResetCounters.Size = Fluent.RibbonControlSize.Middle;
                characterStopAlerts.HorizontalAlignment = HorizontalAlignment.Right;
                characterResetCounters.HorizontalAlignment = HorizontalAlignment.Right;
                characterStopAlerts.Icon = "Images/Oxygen-Icons.org-Oxygen-Actions-im-user.ico";
                characterResetCounters.Icon = "Images/Oxygen-Icons.org-Oxygen-Actions-im-user.ico";
                characterStopAlerts.Click += RbnStopAlert_Click;
                rbnStopAlerts.Items.Add(characterStopAlerts);
                rbnResetCounters.Items.Add(characterResetCounters);
            }
        }
        private void PopResetRibbon(String character)
        {
            List<Fluent.Button> stoptoremove = new List<Fluent.Button>();
            List<Fluent.Button> resettoremove = new List<Fluent.Button>();
            foreach (Fluent.Button stopalert in rbnStopAlerts.Items)
            {
                if (stopalert.Header.ToString() == character)
                {
                    stoptoremove.Add(stopalert);
                }
            }
            foreach (Fluent.Button resetalert in rbnResetCounters.Items)
            {
                if (resetalert.Header.ToString() == character)
                {
                    resettoremove.Add(resetalert);
                }
            }
            foreach (Fluent.Button removeitem in stoptoremove)
            {
                rbnStopAlerts.Items.Remove(removeitem);
            }
            foreach (Fluent.Button removeitem in resettoremove)
            {
                rbnResetCounters.Items.Remove(removeitem);
            }
        }
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
                    //Remove Character from stop alerts menu items.
                    PopResetRibbon(((CharacterProfile)listviewCharacters.SelectedItem).Name);
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
            if (selected.Monitor)
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
        private void StartMonitoring()
        {
            //Start Monitoring Enabled Profiles
            string archivefolder = programsettings.Single<Setting>(i => i.Name == "LogArchiveFolder").Value;
            string archivemethod = programsettings.Single<Setting>(i => i.Name == "ArchiveMethod").Value;
            string autodelete = programsettings.Single<Setting>(i => i.Name == "AutoDelete").Value;
            string compressarchive = programsettings.Single<Setting>(i => i.Name == "CompressArchive").Value;
            int logsize = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "LogSize").Value);
            int archivedays = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "DeleteArchives").Value);
            foreach (CharacterProfile character in characterProfiles)
            {
                AddResetRibbon();
                if (File.Exists(character.LogFile) && character.Monitor)
                {
                    MonitorCharacter(character);
                    //Monitor(character);
                    //Start Log Maintenance
                    if ((programsettings.Single<Setting>(i => i.Name == "AutoArchive")).Value == "true")
                    {
                        switch (archivemethod)
                        {
                            case "Size Threshold":
                                //Start Task, check log size every 5 minutes.
                                //If log size is greater than programsettings["Log Size"].
                                //Move File to archive
                                //Create new file
                                SizeMaintenance(character.LogFile, archivefolder, logsize, compressarchive, autodelete);
                                break;
                            case "Scheduled":
                                //Start Task, check the date every 5 minutes.
                                //If today is the scheduled day
                                //Move File to archive
                                //Create new file
                                ScheduledMaintenance(character.LogFile, archivefolder, logsize, compressarchive, autodelete);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    //Don't monitor character
                }
            }
        }
        private void StopMonitoring()
        {
            foreach(CharacterProfile profile in characterProfiles)
            {
                profile.Monitor = false;
            }
        }
        private async void InitializePushback()
        {
            await Task.Run(() =>
            {
                masterpushbacklist.Clear();
                if (!File.Exists(GlobalVariables.defaultPath + @"\pushback-list.csv"))
                {
                    //Get file from github
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(GlobalVariables.pushbackurl, GlobalVariables.defaultPath + @"\pushback-list.csv");
                    }

                }
                using (var reader = new StreamReader(GlobalVariables.defaultPath + @"\pushback-list.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        String[] vars = line.Split(',');
                        Tuple<String, Double> entry = new Tuple<string, double>(vars[1], Convert.ToDouble(vars[2]));
                        masterpushbacklist.Add(vars[0], entry);
                    }
                }
            });
        }
        private async void InitializePushup()
        {
            await Task.Run(() =>
            {
                masterpushuplist.Clear();
                if (!File.Exists(GlobalVariables.defaultPath + @"\pushup-list.csv"))
                {
                    //Get file from github
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(GlobalVariables.pushupurl, GlobalVariables.defaultPath + @"\pushup-list.csv");
                    }
                }
                using (var reader = new StreamReader(GlobalVariables.defaultPath + @"\pushup-list.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        String[] vars = line.Split(',');
                        try
                        {
                            dictpushup.Add(vars[1], Convert.ToDouble(vars[2]));
                        }
                        catch (ArgumentException)
                        { /*item probably already added*/ }
                        Tuple<String, Double> entry = new Tuple<string, double>(vars[1], Convert.ToDouble(vars[2]));
                        masterpushuplist.Add(vars[0], entry);
                    }
                }
            });
        }
        private void UpdateTimer(OverlayTimerWindow otw, Trigger acttrigger, Boolean updown, String charname, Category actcategory)
        {
            syncontext.Post(new SendOrPostCallback(o =>
            {
                otw.AddTimer(((Trigger)o).TimerName, ((Trigger)o).TimerDuration, updown, charname, actcategory); ;
                otw.DataContext = otw;
            }), acttrigger);
        }
        private void UpdateText(OverlayTextWindow otw, Trigger acttrigger)
        {
            syncontext.Post(new SendOrPostCallback(o =>
            {
                otw.AddTrigger((Trigger)o);
                otw.DataContext = otw;
            }), acttrigger);
        }
        private void UpdateLineCount(int value)
        {
            syncontext.Post(new SendOrPostCallback(o =>
            {
                totallinecount += (int)o;
                statusbarStatus.DataContext = totallinecount;
            }), value);
        }
        private async void MonitorCharacter(CharacterProfile character)
        {
           Console.WriteLine($"Monitoring {character.Name}");
            #region threading
            await Task.Run(async () =>
            {
                using (FileStream filestream = File.Open(character.LogFile, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    filestream.Seek(0, SeekOrigin.End);
                    using (StreamReader streamReader = new StreamReader(filestream))
                    {
                        while (true)
                        {
                            String capturedLine = streamReader.ReadLine();
                            if (capturedLine != null)
                            {
                                Stopwatch stopwatch = new Stopwatch();
                                stopwatch.Start();
                                UpdateLineCount(1);
                                Match eqline = Regex.Match(capturedLine, GlobalVariables.eqRegex);
                                String tomatch = eqline.Groups["stringToMatch"].Value;
                                String eqtime = eqline.Groups["eqtime"].Value;
                                foreach (KeyValuePair<Trigger,ArrayList> doc in listoftriggers)
                                {
                                    Match triggermatch = Regex.Match(tomatch, Regex.Escape(doc.Key.SearchText), RegexOptions.IgnoreCase);
                                    if (triggermatch.Success && doc.Value.Contains(character.Id))
                                    {
                                        Console.WriteLine($"Matched Trigger {doc.Key.Id}");
                                        Stopwatch firetrigger = new Stopwatch();
                                        firetrigger.Start();
                                        await Task.Run( () => { FireTrigger(doc.Key, character, tomatch, eqtime); });
                                        firetrigger.Stop();
                                        Console.WriteLine($"Fired Trigger in {firetrigger.Elapsed.Seconds}");
                                    }
                                }
                                stopwatch.Stop();
                                Console.WriteLine($"Trigger matched in: {stopwatch.Elapsed.ToString()}");
                                if (pushbackToggle)
                                {
                                    PushMonitor(capturedLine, character.ProfileName);
                                }
                            }
                            if (characterProfiles.Any(x => x.Monitor == false && x.ProfileName == character.ProfileName))
                            {
                                break;
                            }
                            Thread.Sleep(1);
                        }
                    }
                }
            });
            #endregion
        }
        private void FireTrigger(Trigger activetrigger,CharacterProfile character, String matchline, String matchtime)
        {
            //Add stopwatch info for trigger
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ActivatedTrigger newactive = new ActivatedTrigger
            {
                Name = activetrigger.Name,
                FromLog = character.ProfileName,
                MatchText = matchline,
                TriggerTime = matchtime
            };
            lock (_triggerLock)
            {
                activatedTriggers.Add(newactive);
            }
            if (activetrigger.AudioSettings.AudioType == "tts")
            { character.Speak(activetrigger.AudioSettings.TTS); }
            if (activetrigger.AudioSettings.AudioType == "file")
            { PlaySound(activetrigger.AudioSettings.SoundFileId); }
            //Add Timer code
            Category triggeredcategory = categorycollection.Single<Category>(i => i.Id == activetrigger.TriggerCategory);
            String overlayname = triggeredcategory.TextOverlay;
            Stopwatch texttimer = new Stopwatch();
            texttimer.Start();
            if (activetrigger.Displaytext != null)
            {
                OverlayTextWindow otw = textWindows.Single<OverlayTextWindow>(i => i.windowproperties.Name == triggeredcategory.TextOverlay);
                UpdateText(otw, activetrigger);
            }
            texttimer.Stop();
            Console.WriteLine($"Text: {texttimer.Elapsed.ToString()}");
            Stopwatch countertimer = new Stopwatch();
            countertimer.Start();
            lock (_timersLock)
            {                                                        
                switch (activetrigger.TimerType)
                {
                    case "Timer(Count Down)":
                        OverlayTimerWindow timerwindowdown = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                        UpdateTimer(timerwindowdown, activetrigger, false, character.characterName, triggeredcategory);
                        break;
                    case "Stopwatch(Count Up)":
                        OverlayTimerWindow timerwindowup = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                        UpdateTimer(timerwindowup, activetrigger, true, character.characterName, triggeredcategory);
                        break;
                    case "Repeating Timer":
                        break;
                    default:
                        break;
                }
            }
            countertimer.Stop();
            Console.WriteLine($"Timer: {countertimer.Elapsed.ToString()}");
            stopwatch.Stop();
            Console.WriteLine($"Trigger Monitor: {stopwatch.Elapsed.ToString()}");
        }
        private void PushMonitor(String line, string profilename)
        {
            //Add stopwatch info for push monitor
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Match pushmatch = GlobalVariables.eqspellRegex.Match(line);
            if (pushmatch.Success)
            {                    
                String logspell = pushmatch.Groups["spellname"].Value;
                #region pushback
                foreach (KeyValuePair<String, Tuple<String, Double>> spell in masterpushbacklist)
                {
                    //Match pushbackmatch = Regex.Match($"^{logspell}$", spell.Value.Item1);
                    if (logspell == spell.Value.Item1)
                    {
                        Pushback pushback = new Pushback
                        {
                            Character = pushmatch.Groups["character"].Value,
                            PushType = "Pushback",
                            Spell = logspell,
                            FromCharacter = profilename,
                            Distance = spell.Value.Item2
                        };
                        lock (_itemsLock)
                        {
                            pushbackList.Add(pushback);
                        }
                    }
                }
                #endregion
                #region pushup
                /*foreach (KeyValuePair<String, Tuple<String, Double>> spell in masterpushuplist)
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
                }*/
                #endregion
            }
            stopwatch.Stop();
            //Console.WriteLine($"Pushback Monitor: {stopwatch.Elapsed.TotalSeconds}");
        }
        private void RbnStopAlerts_Click(object sender, RoutedEventArgs e)
        {
            foreach (OverlayTimerWindow timerwindow in timerWindows)
            {
                timerwindow.TimerBars.Clear();
            }
        }
        private void RbnStopAlert_Click(object sender, RoutedEventArgs e)
        {
            String character = (String)((System.Windows.Controls.Ribbon.RibbonSplitMenuItem)e.Source).Header;
            foreach (OverlayTimerWindow timerwindow in timerWindows)
            {
                timerwindow.RemoveTimer(character);
            }
            e.Handled = true;
        }
        #endregion
        #region Triggers
        private void TriggerLoad()
        {
            listoftriggers.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colTriggers = db.GetCollection<Trigger>("triggers");
                IEnumerable<Trigger> triggerlist = colTriggers.FindAll();
                foreach(Trigger trigger in triggerlist)
                {
                    listoftriggers.Add(trigger, trigger.profiles);
                }
            }
        }
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            TreeViewModel selectedGroup = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerEditor newTrigger = new TriggerEditor(selectedGroup);
            newTrigger.Show();
            TriggerLoad();
        }
        private void TriggerRemoved_TreeViewModel(object sender, PropertyChangedEventArgs e)
        {
            RemoveTrigger(e.PropertyName);
        }
        private void TriggerAdded_TreeViewModel(object sender, PropertyChangedEventArgs e)
        {
            AddTrigger(e.PropertyName);
        }
        public void RemoveTrigger(String triggerName)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                var currentTrigger = colTriggers.FindById(Convert.ToInt32(triggerName));
                if ((currentTrigger.Profiles.Contains(selectedCharacter.Id)))
                {
                    currentprofile.Triggers.Remove(currentTrigger.id);
                    currentTrigger.profiles.Remove(selectedCharacter.Id);
                }
                colTriggers.Update(currentTrigger);
                colProfiles.Update(currentprofile);
            }
            //TriggerLoad();
        }
        public void AddTrigger(String triggerName)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                var currentTrigger = colTriggers.FindById(Convert.ToInt32(triggerName));
                if (!(currentTrigger.Profiles.Contains(selectedCharacter.Id)))
                {
                    currentprofile.Triggers.Add(currentTrigger.id);
                    currentTrigger.profiles.Add(selectedCharacter.Id);
                }
                colTriggers.Update(currentTrigger);
                colProfiles.Update(currentprofile);
            }
            TriggerLoad();
        }
        private void TriggerRemove_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteTrigger(root.Id);
                UpdateView();
                TriggerLoad();
            }
        }
        private void TriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var triggerCollection = db.GetCollection<Trigger>("triggers");
                var currentTrigger = triggerCollection.FindById(root.Id);
                //var currentTrigger = triggerCollection.FindOne(Query.EQ("Name", root.Name));
                TriggerEditor triggerDialog = new TriggerEditor(currentTrigger.Id);
                triggerDialog.Show();
            }
        }
        private void Availabletriggers_IsSelectedChanged(object sender, EventArgs e)
        {
            if (availabletriggers.IsSelected == true)
            {
                ribbonMain.SelectedTabIndex = 0;
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
            TriggerLoad();
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
            TriggerLoad();
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
                triggergroupCollection.Update(basegroup);
            }
            TriggerLoad();
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
                foreach (int childgroup in deadgroup.Children)
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
                if (basegroup.Triggers.Count > 0)
                {
                    foreach (int triggerid in basegroup.Triggers)
                    {
                        CopyTrigger(triggerid, newgid);
                    }
                }
                if (basegroup.Children.Count > 0)
                {
                    foreach (int child in basegroup.Children)
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
                            Type = "trigger",
                            Id = getTrigger.Id
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
        private void Treemerge_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(null);
            }
        }
        private void Treemerge_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseposition = e.GetPosition(null);
            Vector diff = _lastMouseDown - mouseposition;

            if(e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                TreeView tree = sender as TreeView;
                TreeViewItem treeitem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if(treeitem != null)
                {
                    TreeViewModel treemodel = (TreeViewModel)treeitem.Header;
                    DataObject dragdata = new DataObject("TreeViewModel", treemodel);
                    DragDrop.DoDragDrop(treeitem, dragdata, DragDropEffects.Move);
                }                               
            }
        }
        private void TreeViewTriggers_DragEnter(object sender, DragEventArgs e)
        {
            if(sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void TreeViewTriggers_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent("TreeViewModel"))
            {
                TreeViewModel copytriggers = e.Data.GetData("TreeViewModel") as TreeViewModel;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ImportTriggers(copytriggers);
                stopwatch.Stop();
                Console.WriteLine($"Imported Triggers in {stopwatch.Elapsed.ToString()}");
            }
            if(e.Data.GetDataPresent("MainTree"))
            {
                TreeViewModel selectedbranch = e.Data.GetData("MainTree") as TreeViewModel;
                if(selectedbranch.Type == "trigger" && droptree.Id != 0 && selectedbranch.Name != droptree.Name)
                {
                    using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                    {
                        var colTriggers = db.GetCollection<Trigger>("triggers");
                        var colTriggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                        //Find the old parent group and remove it as a child
                        TriggerGroup oldgroup = colTriggerGroups.FindOne(x => x.Triggers.Contains(selectedbranch.Id));
                        oldgroup.RemoveTrigger(selectedbranch.Id);
                        colTriggerGroups.Update(oldgroup);
                        //Add the trigger as child to droptree
                        TriggerGroup newgroup = colTriggerGroups.FindById(droptree.Id);
                        newgroup.Triggers.Add(selectedbranch.Id);
                        colTriggerGroups.Update(newgroup);
                        //Change the parent of the trigger
                        Trigger movetrigger = colTriggers.FindById(selectedbranch.Id);
                        movetrigger.Parent = droptree.Id;
                        colTriggers.Update(movetrigger);
                    }
                    UpdateTriggerView();
                }
                if(selectedbranch.Type == "triggergroup" && selectedbranch.Name != droptree.Name)
                {
                    using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                    {
                        var colTriggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                        TriggerGroup currentgroup = colTriggerGroups.FindById(selectedbranch.Id);
                        if (currentgroup.Parent != 0)
                        {
                            //Find the old parent group and remove it as a child
                            TriggerGroup oldgroup = colTriggerGroups.FindOne(x => x.Children.Contains(selectedbranch.Id));
                            oldgroup.RemoveChild(selectedbranch.Id);
                            colTriggerGroups.Update(oldgroup);
                        }
                        //update the parent with the new dropbranch id
                        currentgroup.Parent = droptree.Id;
                        colTriggerGroups.Update(currentgroup);
                        if (droptree.Id != 0)
                        {
                            //Add the triggergroup as child to droptree
                            TriggerGroup newgroup = colTriggerGroups.FindById(droptree.Id);
                            newgroup.AddChild(selectedbranch.Id);
                            colTriggerGroups.Update(newgroup);
                        }
                    }                    
                }
                UpdateTriggerView();
            }
        }
        private void TreeViewTriggers_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(null);
            }
        }
        private void TreeViewTriggers_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseposition = e.GetPosition(null);
            Vector diff = _lastMouseDown - mouseposition;
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                TreeView tree = sender as TreeView;
                TreeViewItem treeitem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (treeitem != null)
                {
                    TreeViewModel treemodel = (TreeViewModel)treeitem.Header;
                    DataObject dragdata = new DataObject("MainTree", treemodel);
                    DragDrop.DoDragDrop(treeitem, dragdata, DragDropEffects.Move);
                }
            }
        }
        private int ImportTriggerGroup(TriggerGroup toimport, int importparent)
        {
            int bsonid = 0;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colTriggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                //create a new group because we need to get bson values from child triggers and group database entries
                //Look for trigger group by name.
                Console.WriteLine($"Inserting Trigger Group: {toimport.TriggerGroupName}");
                TriggerGroup newgroup = new TriggerGroup
                {
                    TriggerGroupName = toimport.TriggerGroupName,
                    Parent = importparent,
                    Comments = toimport.Comments,
                };
                bsonid = colTriggerGroups.Insert(newgroup);
                //If child groups, recursive call
                if (toimport.Children.Count > 0)
                {
                    foreach(int child in toimport.Children)
                    {
                        TriggerGroup childgroup = mergegroups.Find(x => x.Id == child);
                        int gid = ImportTriggerGroup(childgroup, bsonid);
                        newgroup.AddChild(gid);
                    }
                }
                if(toimport.Triggers.Count > 0)
                {
                    foreach(int child in toimport.Triggers)
                    {
                        Trigger childtrigger = mergetriggers[child];
                        childtrigger.Parent = bsonid;
                        childtrigger.Id = 0;
                        int tid = ImportTrigger(childtrigger);
                        newgroup.AddTriggers(tid);
                    }                    
                }
                //Update the database after we've entered in all the children triggers and groups
                colTriggerGroups.Update(newgroup);               
            }
            return bsonid;
        }
        private int ImportTrigger(Trigger toimport)
        {
            int bsonid = 0;
            toimport.Id = 0;
            //Console.WriteLine($"Inserting Trigger: {toimport.Name}");
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colTriggers = db.GetCollection<Trigger>("triggers");
                toimport.TriggerCategory = defaultcategory.Id;
                bsonid = colTriggers.Insert(toimport);
                //Activate trigger for all profiles
                AllProfileEnableTrigger(bsonid);
            }
            return bsonid;
        }
        private void AllProfileEnableTrigger(int bsonid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                Trigger trigger = colTriggers.FindById(bsonid);
                //Activate for every profile
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                IEnumerable<CharacterProfile> profiles = colProfiles.FindAll();
                foreach(CharacterProfile profile in profiles)
                {
                    profile.AddTrigger(bsonid);
                    trigger.Profiles.Add(profile.Id);                    
                    colProfiles.Update(profile);
                }
                colTriggers.Update(trigger);
                stopwatch.Stop();
                Console.WriteLine($"Took {stopwatch.Elapsed.ToString()} to add profiles.");
            }
        }
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
        private void TreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            TreeViewModel hover = (TreeViewModel)((TreeViewItem)sender).DataContext;
            if (hover.Type == "triggergroup" || hover.Name == "All Triggers")
            {
                droptree = hover;
                ((TreeViewItem)sender).Background = Brushes.SpringGreen;
                e.Handled = true;
            }     
        }
        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            TreeViewModel hover = (TreeViewModel)((TreeViewItem)sender).DataContext;
            if(hover.Type == "triggergroup" || hover.Name == "All Triggers")
            {
                ((TreeViewItem)sender).Background = Brushes.LightSteelBlue;
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
                                newChildBranch.Id = getTrigger.Id;
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
            TriggerLoad();
        }
        public void UpdateListView()
        {
            characterProfiles.Clear();
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
            String overlayname = (sender as Fluent.Button).Name;
            OverlayTextEditor newOverlayEditor = new OverlayTextEditor(overlayname);
            newOverlayEditor.Show();
        }
        private void TimerOverlayProperties_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as Fluent.Button).Name;
            OverlayTimerEditor newOverlayEditor = new OverlayTimerEditor(overlayname);
            newOverlayEditor.Show();
        }
        private void TextOverlayDelete_Click(object sender, RoutedEventArgs e)
        {
            String overlayname = (sender as Fluent.Button).Name;
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
            String overlayname = (sender as Fluent.Button).Name;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                overlaytimers.Delete(Query.EQ("Name", overlayname));
            }
            //Kill current overlay if running
            List<OverlayTimerWindow> toremove = new List<OverlayTimerWindow>();
            foreach (OverlayTimerWindow overlay in timerWindows)
            {
                if (overlay.Name == overlayname)
                {
                    toremove.Add(overlay);
                }
            }
            foreach (OverlayTimerWindow removewindow in toremove)
            {
                timerWindows.Remove(removewindow);
                removewindow.Close();
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
                    Fluent.SplitButton overlaytimer = new Fluent.SplitButton();
                    overlaytimer.Header = overlay.Name;
                    overlaytimer.LargeIcon = "Images/Google-Noto-Emoji-Travel-Places-42608-stopwatch.ico";
                    overlaytimer.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
                    Fluent.Button timerProperties = new Fluent.Button();
                    timerProperties.Name = overlay.Name;
                    timerProperties.Header = "Properties";
                    timerProperties.AddHandler(Button.ClickEvent, new RoutedEventHandler(TimerOverlayProperties_Click));
                    timerProperties.Size = Fluent.RibbonControlSize.Middle;
                    timerProperties.HorizontalAlignment = HorizontalAlignment.Right;
                    Fluent.Button timerDelete = new Fluent.Button();
                    timerDelete.Header = "Delete";
                    timerDelete.Name = overlay.Name;
                    timerDelete.Size = Fluent.RibbonControlSize.Middle;
                    timerDelete.HorizontalAlignment = HorizontalAlignment.Right;
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
                    Fluent.SplitButton overlaytext = new Fluent.SplitButton();
                    overlaytext.Header = overlay.Name;
                    overlaytext.LargeIcon = "Images/Oxygen-Icons.org-Oxygen-Actions-document-new.ico";
                    overlaytext.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
                    Fluent.Button textProperties = new Fluent.Button();
                    textProperties.Name = overlay.Name;
                    textProperties.Header = "Properties";
                    textProperties.Size = Fluent.RibbonControlSize.Middle;
                    textProperties.HorizontalAlignment = HorizontalAlignment.Right;
                    textProperties.AddHandler(Button.ClickEvent, new RoutedEventHandler(TextOverlayProperties_Click));
                    Fluent.Button textDelete = new Fluent.Button();
                    textDelete.Header = "Delete";
                    textDelete.Name = overlay.Name;
                    textDelete.Size = Fluent.RibbonControlSize.Middle;
                    textDelete.HorizontalAlignment = HorizontalAlignment.Right;
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
                ribbonMain.SelectedTabIndex = 3;
                Refresh_Categories();
            }
        }
        private void RibbonMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(ribbonMain.SelectedTabIndex)
            {
                case 3:
                    categoriesDocument.IsSelected = true;
                    Refresh_Categories();
                    break;
                case 4:
                    firedtriggerspane.IsActive = true;
                    break;
                default:
                    availabletriggers.IsSelected = true;
                    break;
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
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TextColors = status;
                categoriescol.Update(category);
            }
        }
        private void CategoryColor_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            Console.WriteLine("UserChecked");
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TextColors = status;
                categoriescol.Update(category);
            }
        }
        private void CategoryColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TextThis = status;
                categoriescol.Update(category);
            }
        }
        private void CategoryColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TextThis = status;
                categoriescol.Update(category);
            }
        }
        private void TimerColors_Checked(object sender, RoutedEventArgs e)
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
        private void TimerColors_Unchecked(object sender, RoutedEventArgs e)
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
        private void TimerThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TimerThis = status;
                categoriescol.Update(category);
            }
        }
        private void TimerThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                category.TimerThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideOverlayCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideOverlayCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextOverlayThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorCharacter_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCharacter = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorCharacter_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorCharacter = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTextColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TextColorThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerCategory_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerCategory_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerOverlayThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorCat_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorCat_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCategory = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorChar_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCharacter = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorChar_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorCharacter = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorThis_Checked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorThis = status;
                categoriescol.Update(category);
            }
        }
        private void OverrideTimerColorThis_Unchecked(object sender, RoutedEventArgs e)
        {
            Boolean status = (Boolean)(sender as RadioButton).IsChecked;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Category> categoriescol = db.GetCollection<Category>("categories");
                var category = categoriescol.FindOne(Query.EQ("Name", selectedcategory));
                ((category.CharacterOverrides.Where(x => x.ProfileName == currentprofile.ProfileName)).First()).TimerColorThis = status;
                categoriescol.Update(category);
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
        private void UpdatePushback_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(GlobalVariables.defaultPath + @"\pushback-list.csv"))
            {
                //delete file
                File.Delete(GlobalVariables.defaultPath + @"\pushback-list.csv");
                //Initialize Pushback
                InitializePushback();
            }
            if (File.Exists(GlobalVariables.defaultPath + @"\pushup-list.csv"))
            {
                //delete file
                File.Delete(GlobalVariables.defaultPath + @"\pushup-list.csv");
                //Initialize Pushup
                InitializePushup();
            }
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
                clipboardtype = "triggergroup";
            }
            if (root.Type == "trigger")
            {
                clipboardtype = "trigger";
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
                    DeleteTriggerGroup(root.Id);
                    UpdateView();
                }
            }
            if (root.Type == "trigger")
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteTrigger(root.Id);
                    UpdateView();
                }
            }
        }
        private void MenuItemTriggerEdit_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if(root.Type == "trigger")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    var triggerCollection = db.GetCollection<Trigger>("triggers");
                    var currentTrigger = triggerCollection.FindById(root.Id);
                    TriggerEditor triggerDialog = new TriggerEditor(currentTrigger.Id);
                    triggerDialog.Show();
                }
            }
            if(root.Type == "triggergroup")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    var col = db.GetCollection<TriggerGroup>("triggergroups");
                    TriggerGroup result = col.FindById(root.Id);
                    //TriggerGroup result = col.FindOne(Query.And(Query.EQ("TriggerGroupName", root.Name), Query.EQ("_id", root.Id)));
                    TriggerGroupEdit triggerDialog = new TriggerGroupEdit(result);
                    triggerDialog.Show();
                }
                UpdateView();
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
                if (clipboardtype == "trigger")
                {
                    CopyTrigger(triggerclipboard, root.Id);
                }
                //Add new Trigger Group
                if (clipboardtype == "triggergroup")
                {
                    if (root.Name == "All Triggers")
                    {
                        CopyTriggerGroup(triggergroupclipboard, 0);
                    }
                    else
                    {
                        CopyTriggerGroup(triggergroupclipboard, root.Id);
                    }                    
                }
            }
            UpdateTriggerView();
        }
        private void TreeViewTriggers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root.Type == "trigger")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    var triggerCollection = db.GetCollection<Trigger>("triggers");
                    var currentTrigger = triggerCollection.FindById(root.Id);
                    TriggerEditor triggerDialog = new TriggerEditor(currentTrigger.Id);
                    triggerDialog.Show();
                }
            }
            if (root.Type == "triggergroup")
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    var col = db.GetCollection<TriggerGroup>("triggergroups");
                    TriggerGroup result = col.FindById(root.Id);
                    //TriggerGroup result = col.FindOne(Query.And(Query.EQ("TriggerGroupName", root.Name), Query.EQ("_id", root.Id)));
                    TriggerGroupEdit triggerDialog = new TriggerGroupEdit(result);
                    triggerDialog.Show();
                }
                UpdateView();
            }
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
                    cmTreePaste.IsEnabled = false;
                    cmTreeEdit.IsEnabled = false;
                    cmAddTriggerGroup.IsEnabled = true;
                    cmAddTrigger.IsEnabled = false;
                    if (triggergroupclipboard != 0)
                    {
                        cmTreePaste.IsEnabled = true;
                    }
                }
                else
                {
                    cmTreeCopy.IsEnabled = true;
                    cmTreeDelete.IsEnabled = true;
                    if (root.Type == "triggergroup")
                    {
                        cmAddTriggerGroup.IsEnabled = true;
                        cmAddTrigger.IsEnabled = true;
                        cmTreeEdit.IsEnabled = true;
                        if (triggergroupclipboard != 0 || triggerclipboard != 0)
                        {
                            cmTreePaste.IsEnabled = true;
                        }
                        else
                        {
                            cmTreePaste.IsEnabled = false;
                        }
                    }
                    if (root.Type == "trigger")
                    {
                        cmTreeEdit.IsEnabled = true;
                        cmAddTrigger.IsEnabled = false;
                        cmAddTriggerGroup.IsEnabled = false;
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
                cmTreeEdit.IsEnabled = false;
            }
        }
        private void CmAddTriggerGroup_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerGroupEdit triggerDialog = new TriggerGroupEdit(root);
            triggerDialog.Show();
        }
        private void CmAddTrigger_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel selectedGroup = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerEditor newTrigger = new TriggerEditor(selectedGroup);
            newTrigger.Show();
            TriggerLoad();
        }
        #endregion
        #region Filtering
        private void TextboxTriggerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            firedtriggerspane.IsActive = true;
            ListCollectionView collectionview = CollectionViewSource.GetDefaultView(datagrid_activated.ItemsSource) as ListCollectionView;
            collectionview.IsLiveFiltering = true;
            collectionview.LiveFilteringProperties.Add("Name");
            collectionview.Filter = new Predicate<object>(TriggerContains);
        }

        private void TextboxPushbackSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            pushbackpane.IsActive = true;
            ListCollectionView collectionview = CollectionViewSource.GetDefaultView(datagrid_pushback.ItemsSource) as ListCollectionView;
            collectionview.IsLiveFiltering = true;
            collectionview.LiveFilteringProperties.Add("Spell");
            collectionview.Filter = new Predicate<object>(PushbackContains);
        }
        public bool TriggerContains(object de)
        {
            ActivatedTrigger acttrigger = de as ActivatedTrigger;
            return (acttrigger.Name.IndexOf(textboxTriggerSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        public bool PushbackContains(object de)
        {
            Pushback actpush = de as Pushback;
            return (actpush.Spell.IndexOf(textboxPushbackSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        #endregion
        #region Logfile Maintenance
        private async void SizeMaintenance(String logfile, String folder, int filesize, String compress, String delete)
        {
            await Task.Run(() =>
            {
                while(true)
                {
                    FileInfo fileinfo = new FileInfo(logfile);
                    if(fileinfo.Length > filesize)
                    {
                        ArchiveLog(logfile, folder);
                    }
                    Thread.Sleep(300000);
                }
            });
        }
        private async void ScheduledMaintenance(String logfile, String folder, int filesize, String compress, String delete)
        {
            await Task.Run(() => {
                while(true)
                {
                    FileInfo fileinfo = new FileInfo(logfile);
                }
            });
        }
        private void ArchiveLog(string logfile, string archivefolder)
        {
            //string filedate = (DateTime.Now).ToFileTime().ToString();
            //string[] logsplits = logfile.Split('\\');
            //string[] filesplit = logsplits[2].Split('.');
            //string newfilename = filesplit[0] + "_" + filedate + '.' + filesplit[1];
            //string archivefile = archivefolder + '\\' + newfilename;
            //File.Move(logfile, archivefile);
            //File.Create(logfile);
        }
        private void CompressLog(string archivefolder)
        {

        }

        #endregion
        #region Import Triggers
        private void ImportFromGINA_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "GINA Trigger Package|*.gtp";
            if(fileDialog.ShowDialog() == true)
            {
                using (ZipArchive archive = ZipFile.OpenRead(fileDialog.FileName))
                {
                    ZipArchiveEntry triggersxml = archive.Entries[0];
                    using (StreamReader streamtriggers = new StreamReader(triggersxml.Open()))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(streamtriggers.ReadToEnd());
                        string json = JsonConvert.SerializeXmlNode(doc);
                        JToken jsontoken = JObject.Parse(json);
                        triggergroupid = 0;
                        triggerid = 0;
                        mergetreeView.Clear();
                        mergetriggers.Clear();
                        mergegroups.Clear();
                        ParseGina(jsontoken.SelectToken("SharedData"));                     
                    }                        
                }
            }
        }
        private void ParseGina(JToken jsontoken)
        {
            mergetriggercount = 0;
            mergeview = new TreeViewModel("Triggers to Import");
            mergeview.IsChecked = false;
            mergetreeView.Add(mergeview);
            int result = GetTriggerGroups(jsontoken.SelectToken("TriggerGroups.TriggerGroup"),triggergroupid);
            //build tree
            foreach(TriggerGroup tg in mergegroups)
            {
                if(tg.Parent == 0)
                {
                    TreeViewModel rTree = new TreeViewModel(tg.TriggerGroupName)
                    {
                        Type = "triggergroup",
                        Id = tg.Id
                    };
                    if(tg.Triggers.Count > 0)
                    {
                        foreach(int item in tg.Triggers)
                        {
                            mergetriggercount++;
                            Trigger findtrigger = mergetriggers.Find(x => x.id == item);
                            TreeViewModel newChildBranch = new TreeViewModel(findtrigger.Name)
                            {
                                Type = "trigger"
                            };
                            rTree.Children.Add(newChildBranch);
                        }
                    }
                    if(tg.Children.Count > 0)
                    {
                        mergeview.Children.Add(BuildMergeTree(tg));
                    }
                    else
                    {
                        mergeview.Children.Add(rTree);
                    }
                }
            }
            mergeview.Initialize();
            treemerge.ItemsSource = mergetreeView;
            Console.WriteLine($"{mergetriggercount}");
            treemerge.Visibility = Visibility.Visible;
        }
        private TreeViewModel BuildMergeTree(TriggerGroup branch)
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
                    mergetriggercount++;
                    Trigger findtrigger = mergetriggers.Find(x => x.id == item);
                    TreeViewModel newChildBranch = new TreeViewModel(findtrigger.Name)
                    {
                        Type = "trigger"
                    };
                    rTree.Children.Add(newChildBranch);
                }
            }
            if (branch.Children.Count > 0)
            {
                foreach (int leaf in branch.Children)
                {
                    TriggerGroup leafGroup = GetMergeTriggerGroup(leaf);
                    rTree.Children.Add(BuildMergeTree(leafGroup));
                }
            }
            rTree.VerifyCheckedState();
            return rTree;
        }
        private TriggerGroup GetMergeTriggerGroup(int id)
        {
            return mergegroups.Find(x => x.id == id);
        }
        private int GetTriggerGroups(JToken jsontoken, int parentid)
        {
            int rval = triggergroupid;
            TriggerGroup newgroup = new TriggerGroup
            {
                TriggerGroupName = jsontoken["Name"].ToString(),
                Comments = jsontoken["Comments"].ToString(),
                Id = triggergroupid,
                Parent = parentid
            };
            mergegroups.Add(newgroup);
            foreach (JToken token in jsontoken.Children())
            {
                switch(((JProperty)token).Name)
                {
                    case "TriggerGroups":
                        if ((jsontoken["TriggerGroups"]["TriggerGroup"]).GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                        {
                            foreach (JToken newtoken in ((JArray)(jsontoken["TriggerGroups"]["TriggerGroup"])).Children())
                            {
                                triggergroupid++;
                                newgroup.Children.Add(GetTriggerGroups(newtoken,triggergroupid));
                            }
                        }
                        else
                        {
                            if ((jsontoken["TriggerGroups"]["TriggerGroup"]).GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
                            {
                                triggergroupid++;
                                newgroup.Children.Add(GetTriggerGroups(jsontoken["TriggerGroups"]["TriggerGroup"], triggergroupid));
                            }
                        }
                        break;
                    case "Triggers":
                        if ((jsontoken["Triggers"]["Trigger"]).GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                        {
                            foreach (JToken newtoken in ((JArray)(jsontoken["Triggers"]["Trigger"])).Children())
                            {
                                newgroup.Triggers.Add(GetTrigger(newtoken, triggergroupid));
                            }
                        }
                        else
                        {
                            if ((jsontoken["Triggers"]["Trigger"]).GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
                            {
                                newgroup.Triggers.Add(GetTrigger(jsontoken["Triggers"]["Trigger"], triggergroupid));
                            }
                        }
                        break;
                    default:
                        break;
                }                
            }            
            return rval;
        }
        private int GetTrigger(JToken jsontoken, int parentid)
        {
            int rval = triggerid;
            Dictionary<String, String> timerconversion = new Dictionary<string, string>();
            timerconversion.Add("NoTimer", "No Timer");
            timerconversion.Add("Timer", "Timer(Count Down)");
            timerconversion.Add("Stopwatch", "Timer(Count Up)");
            timerconversion.Add("RepeatingTimer", "Repeating Timer");
            Trigger newtrigger = new Trigger
            {
                id = triggerid,
                name = jsontoken["Name"].ToString(),
                profiles = new ArrayList(),
                searchText = (String)jsontoken["TriggerText"],
                comments = (String)jsontoken["Comments"],
                regex = (bool)jsontoken["EnableRegex"],
                fastcheck = (bool)jsontoken["UseFastCheck"],
                parent = parentid,
                triggerCategory = 0,
                displaytext = (String)jsontoken["DisplayText"],
                clipboardtext = (String)jsontoken["ClipboardText"],
                audioSettings = new Audio(),
                timerType = timerconversion[(String)jsontoken["TimerType"]],
                timerName = (String)jsontoken["TimerName"],
                timerDuration = (int)jsontoken["TimerDuration"],
                triggeredAgain = 2,
                endEarlyText = new BindingList<SearchText>(),
                timerEndingDuration = 0,
                timerEndingDisplayText = "",
                timerEndingClipboardText = "",
                timerEndingAudio = new Audio(),
                timerEnding = (bool)jsontoken["UseTimerEnding"],
                timerEndedClipboardText = "",
                timerEndedDisplayText = "",
                timerEnded = (bool)jsontoken["UseTimerEnded"],
                timerEndedAudio = new Audio(),
                resetCounter = (bool)jsontoken["UseCounterResetTimer"],
                resetCounterDuration = (int)jsontoken["CounterResetDuration"],
            };
            //Set Timer Behavior
            switch ((String)jsontoken["TimerStartBehavior"])
            {
                case "StartNewTimer":
                    newtrigger.TriggeredAgain = 0;
                    break;
                case "RestartTimer":
                    newtrigger.TriggeredAgain = 1;
                    break;
                default:
                    break;
            }
            //Set Audio Settings
            newtrigger.AudioSettings.Interrupt = (bool)jsontoken["InterruptSpeech"];
            if ((bool)jsontoken["UseTextToVoice"])
            {
                newtrigger.AudioSettings.AudioType = "tts";
                newtrigger.AudioSettings.TTS = (String)jsontoken["TextToVoiceText"];
            }
            if ((bool)jsontoken["PlayMediaFile"])
            {
                newtrigger.AudioSettings.AudioType = "file";
                //set audio file
            }

            //Add End Early Text
            if (jsontoken["TimerEarlyEnders"].Count() > 0)
            {
                if (jsontoken["TimerEarlyEnders"]["EarlyEnder"].GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
                {
                    SearchText search = new SearchText
                    {
                        Searchtext = (String)jsontoken["TimerEarlyEnders"]["EarlyEnder"]["EarlyEndText"],
                        Regex = (Boolean)jsontoken["TimerEarlyEnders"]["EarlyEnder"]["EnableRegex"]
                    };
                    newtrigger.EndEarlyText.Add(search);
                }
                else
                {
                    JArray earlyenders = (JArray)jsontoken["TimerEarlyEnders"]["EarlyEnder"];
                    if ((earlyenders.Children()).Count() > 0)
                    {
                        foreach (JToken newtoken in earlyenders.Children())
                        {
                            SearchText search = new SearchText
                            {
                                Searchtext = (String)newtoken["EarlyEndText"],
                                Regex = (Boolean)newtoken["EnableRegex"]
                            };
                            newtrigger.EndEarlyText.Add(search);
                        }
                    }
                }
            }
            //Add Timer Ending Trigger
            if (jsontoken["TimerEndingTrigger"] != null)
            {
                if (jsontoken["TimerEndingTrigger"].Count() > 0)
                {
                    newtrigger.TimerEnding = (bool)jsontoken["TimerEndingTrigger"]["UseText"];
                    newtrigger.TimerEndingDisplayText = (String)jsontoken["TimerEndingTrigger"]["DisplayText"];
                    newtrigger.TimerEndingDuration = (int)jsontoken["TimerEndingTime"];
                    if ((bool)jsontoken["TimerEndingTrigger"]["UseTextToVoice"])
                    {
                        newtrigger.TimerEndingAudio.AudioType = "tts";
                        newtrigger.TimerEndingAudio.TTS = (String)jsontoken["TimerEndingTrigger"]["TextToVoiceText"];
                        newtrigger.TimerEndingAudio.Interrupt = (bool)jsontoken["TimerEndingTrigger"]["InterruptSpeech"];
                    }
                    if ((bool)jsontoken["TimerEndingTrigger"]["PlayMediaFile"])
                    {
                        newtrigger.TimerEndingAudio.AudioType = "file";
                        newtrigger.TimerEndingAudio.Interrupt = (bool)jsontoken["TimerEndingTrigger"]["InterruptSpeech"];
                    }
                }
            }
            if(jsontoken["TimerEndedTrigger"] != null)
            {
                //Add Timer Ended Trigger
                if (jsontoken["TimerEndedTrigger"].Count() > 0)
                {
                    newtrigger.TimerEnded = (bool)jsontoken["TimerEndedTrigger"]["UseText"];
                    newtrigger.TimerEndedDisplayText = (String)jsontoken["TimerEndedTrigger"]["DisplayText"];
                    if ((bool)jsontoken["TimerEndedTrigger"]["UseTextToVoice"])
                    {
                        newtrigger.TimerEndedAudio.AudioType = "tts";
                        newtrigger.TimerEndedAudio.TTS = (String)jsontoken["TimerEndedTrigger"]["TextToVoiceText"];
                        newtrigger.TimerEndedAudio.Interrupt = (bool)jsontoken["TimerEndedTrigger"]["InterruptSpeech"];
                    }
                    if ((bool)jsontoken["TimerEndedTrigger"]["PlayMediaFile"])
                    {
                        newtrigger.TimerEndedAudio.AudioType = "file";
                        newtrigger.TimerEndedAudio.Interrupt = (bool)jsontoken["TimerEndedTrigger"]["InterruptSpeech"];
                        //add media file
                    }
                }
            }
            mergetriggers.Add(newtrigger);
            triggerid++;
            return rval;
        }
        private void ImportTriggers(TreeViewModel importtree)
        {
            //Walk through the tree and verify the node is in the database.
            //check the root node, then walk through the children.
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colTriggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                var colTriggers = db.GetCollection<Trigger>("triggers");
                //We can only drop onto trigger groups, find the object of the one we dropped on
                //If it's All triggers, manually set the id to 0
                TriggerGroup dropTriggerGroup = new TriggerGroup
                {
                    Id = 0
                };
                if (droptree.Name != "All Triggers")
                {
                    dropTriggerGroup = colTriggerGroups.FindById(droptree.Id);
                }
                //If we're importing over a trigger group, walk through the tree
                if (importtree.Type == "triggergroup")
                {
                    //Find our object in mergegroups, add self and children to the database.
                    TriggerGroup rootgroup = mergegroups.Find(x => x.Id == importtree.Id);
                    //Insert trigger group into database get bsonid return value.                    
                    //add triggergroup id to dropnode children and update in the database.
                    int gid = ImportTriggerGroup(rootgroup, droptree.Id);
                    dropTriggerGroup.AddChild(gid);
                    colTriggerGroups.Update(dropTriggerGroup);
                    //Delete the rootgroup out of the mergegroup
                    mergegroups.Remove(rootgroup);
                    //Clean up any groups that might reference rootgroup
                    foreach(TriggerGroup tg in mergegroups)
                    {
                        tg.RemoveChild(rootgroup.Id);
                    }
                }
                //If we're dragging over a single trigger, add it to the group which is not All Triggers
                if (importtree.Type == "trigger" && droptree.Name != "All Triggers")
                {
                    //find the id in our mergetriggers and add it.
                    Trigger roottrigger = mergetriggers.Find(x => x.Name == importtree.Name);
                    roottrigger.Parent = droptree.Id;
                    int bsonid = ImportTrigger(roottrigger);
                    //add the trigger to the drop group
                    dropTriggerGroup.AddTriggers(bsonid);
                    colTriggerGroups.Update(dropTriggerGroup);
                    //Delete the trigger out of mergetriggers
                    mergetriggers.Remove(roottrigger);
                }
            }
            //Delete Imported triggers/groups from import tree
            TreeViewModel toremove = new TreeViewModel("todelete");
            foreach(TreeViewModel tvm in mergetreeView)
            {
                DeleteBranch(tvm, importtree.Id);
                if(tvm.Id == importtree.Id)
                {
                    toremove = tvm;
                }
            }
            if(toremove.Name != "todelete")
            {
                mergetreeView.Remove(toremove);
            }
            treemerge.ItemsSource = mergetreeView;
            //Once we're done with the import, update the trigger view
            UpdateListView();
            TriggerLoad();
            //Keep the merge tree up until the user clears it
            if(mergetreeView.Count > 0)
            {
                treemerge.Visibility = Visibility.Visible;
            }
            else
            {
                treemerge.Visibility = Visibility.Hidden;
            }
            
        }
        private void DeleteBranch(TreeViewModel tvm, int idtodelete)
        {
            TreeViewModel deletetree = new TreeViewModel("deltree");
            foreach (TreeViewModel child in tvm.Children)
            {
                if(child.Children.Count > 0)
                {
                    DeleteBranch(child,idtodelete);
                }
                if(child.Id == idtodelete)
                {
                    deletetree = child;
                }
            }
            if (deletetree.Name != "deltree")
            {
                tvm.RemoveChild(deletetree);
            }
        }


        #endregion
    }
}
