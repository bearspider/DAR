using LiteDB;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Xceed.Wpf.AvalonDock.Themes;

namespace HEAP
{
    /// <summary>
    /// Global Variables that are used throughout the program
    /// </summary>
    public class GlobalVariables
    {
        public static string defaultPath = @"C:\EQAudioTriggers";
        public static string defaultDB = $"{defaultPath}\\eqtriggers.db";
        public static string backupDB = $"{defaultPath}\\BackupDB";
        public static Regex eqRegex = new Regex(@"\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\](?<stringToMatch>.*)",RegexOptions.Compiled);
        public static Regex shareRegex = new Regex(@".*?\{HEAP:(?<GUID>.*?)\}",RegexOptions.Compiled);
        public static Regex eqspellRegex = new Regex(@"(\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\])\s((?<character>\w+)\sbegin\s(casting|singing)\s(?<spellname>.*)\.)|(\[(?<eqtime>\w+\s\w+\s+\d+\s\d+:\d+:\d+\s\d+)\])\s(?<character>\w+)\s(begins\sto\s(cast|sing)\s.*\<(?<spellname>.*)\>)", RegexOptions.Compiled);
        //public static string pathRegex = @"(?<logdir>.*\\)(?<logname>eqlog_.*\.txt)";
        public static string pushbackurl = @"https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushback.csv";
        public static string pushupurl = @"https://raw.githubusercontent.com/bearspider/EQ-LogParsers/master/pushup.csv";
        public static string litedbfileprefix = @"$/triggersounds/";
        public static string apiserver = @"heapapi.azurewebsites.net";
        public static string restbase = @"/api/heap";
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
    public class ViewConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if ((Boolean)value)
            {
                rString = "Visible";
            }
            else
            {
                rString = "Hidden";
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
        private List<int> addedtriggers = new List<int>();
        private TreeViewModel mergeview;
        private TreeViewModel droptree;
        private int mergetriggercount = 0;
        private List<String> exportsounds = new List<String>();

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
        private List<TriggerGroup> dballgroups = new List<TriggerGroup>();
        private List<Trigger> dballtriggers = new List<Trigger>();
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
        private String shareguid = "";

        //Maintenance
        private Maintenance logmaintenance = new Maintenance();

        //Settings variables
        private ObservableCollection<String> trustedsenders = new ObservableCollection<String>();
        private String logmatchlocation = "";
        private Boolean soundenabled = true;
        private Boolean textenabled = true;
        private Boolean timerenabled = true;
        private Boolean stopfirstmatch = true;
        private Boolean logmatchestofile = true;
        private String clipboard = @"{C}";
        private Int32 mastervolume = 100;

        //System Tray Icons
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;

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
            //Check if database exists, if it does, make a backup
            if(File.Exists(GlobalVariables.defaultDB))
            {
                if (!Directory.Exists(GlobalVariables.backupDB))
                {
                    Directory.CreateDirectory(GlobalVariables.backupDB);
                }
                File.Copy(GlobalVariables.defaultDB, (GlobalVariables.backupDB + @"\eqtriggers.db"),true);
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
                    foreach (Setting programsetting in settings.FindAll())
                    {
                        programsettings.Add(programsetting);
                    }
                    LoadSettingsTab();
                }
            }
            //Setup the system tray
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/HEAP;component/Images/Tonev-Windows-7-Windows-7-headphone.ico")).Stream;
            MyNotifyIcon.Icon = new System.Drawing.Icon(iconStream);
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
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
            TriggerLoad("Main Program");

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

                //Load all groups and triggers into List so we don't have to go back and forth to the database.
                GenerateMasterList("Main Program");
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
        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if((bool)checkboxMinimize.IsChecked)
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.ShowInTaskbar = false;
                    MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                    MyNotifyIcon.BalloonTipText = "Minimized the app ";
                    MyNotifyIcon.ShowBalloonTip(400);
                    MyNotifyIcon.Visible = true;
                }
                else if (this.WindowState == WindowState.Normal)
                {
                    MyNotifyIcon.Visible = false;
                    this.ShowInTaskbar = true;
                }
            }
        }
        private void GenerateMasterList(String callingfunction)
        {
            Console.WriteLine($"Updating Master List From {callingfunction}");
            dballgroups.Clear();
            dballtriggers.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<TriggerGroup> triggergroups = db.GetCollection<TriggerGroup>("triggergroups");
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                dballgroups = (triggergroups.FindAll()).ToList();
                dballtriggers = (triggers.FindAll()).ToList();
            }
        }
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
            ProfileEditor editCharacter = new ProfileEditor(selectedCharacter);
            editCharacter.Show();
            UpdateView();
        }
        private void RibbonButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            DeleteCharacter((CharacterProfile)listviewCharacters.SelectedItem);
        }
        private void DeleteCharacter(CharacterProfile character)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                var triggers = db.GetCollection<Trigger>("triggers");
                var categories = db.GetCollection<Category>("categories");
                String selectedCharacter = (character).ProfileName;
                int profileid = (character).Id;
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
                }
            }
            UpdateView();
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
            selected.Monitor = !selected.Monitor;
            if (selected.Monitor)
            {
                MonitorCharacter(selected, logmaintenance);
            }
            //update the monitor variable in the database in case we have to refresh the character list, then we know the current state of it's monitor.
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<CharacterProfile> profiles = db.GetCollection<CharacterProfile>("profiles");
                CharacterProfile tochange = profiles.FindOne(Query.EQ("Name", selected.Name));
                tochange.Monitor = selected.Monitor;
                profiles.Update(tochange);
            }
        }
        private void MenuItemCharEdit_Click(object sender, RoutedEventArgs e)
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            ProfileEditor editCharacter = new ProfileEditor(selectedCharacter);
            editCharacter.Show();
            UpdateView();
        }
        private void MenuItemCharDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCharacter((CharacterProfile)listviewCharacters.SelectedItem);
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
                    MonitorCharacter(currentProfile, logmaintenance);
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
        private void UpdateVolume()
        {
            foreach(CharacterProfile profile in characterProfiles)
            {
                profile.UpdateVolume(mastervolume);
            }
        }
        #endregion
        #region Monitoring
        private void StartMonitoring()
        {
            //Start Monitoring Enabled Profiles

            foreach (CharacterProfile character in characterProfiles)
            {
                AddResetRibbon();
                if (File.Exists(character.LogFile) && character.Monitor)
                {
                    MonitorCharacter(character, logmaintenance);
                }
                else
                {
                    //Don't monitor character
                }
            }
        }
        private void StopMonitoring()
        {
            foreach (CharacterProfile profile in characterProfiles)
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
                otw.AddTimer((Trigger)o, updown, charname, actcategory);
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
        private async void MonitorCharacter(CharacterProfile character, Maintenance logmaintenance)
        {
            Console.WriteLine($"Monitoring {character.Name}");
            //Start Log Maintenance before monitoring
            if (logmaintenance.AutoArchive == "true")
            {
                ScheduledMaintenance(character.LogFile, logmaintenance);
            }
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
                                //Console.WriteLine($"Matching line {capturedLine}");
                                Stopwatch stopwatch = new Stopwatch();
                                stopwatch.Start();
                                UpdateLineCount(1);
                                if (capturedLine.Contains(@"{HEAP:"))
                                {
                                    Match sharingmatch = GlobalVariables.shareRegex.Match(capturedLine);
                                    if(sharingmatch.Success)
                                    {
                                        GetShare(sharingmatch.Groups["GUID"].Value.ToString());
                                    }
                                }
                                else
                                {
                                    Parallel.ForEach(listoftriggers, (KeyValuePair<Trigger, ArrayList> doc, ParallelLoopState state) =>
                                    {
                                        //Do regex match if enabled otherwise string.contains
                                        Boolean foundmatch = false;
                                        if (doc.Key.Regex)
                                        {
                                            Boolean checkregex = true;
                                            if(doc.Key.Fastcheck)
                                            {
                                                if(!capturedLine.Contains(doc.Key.Digest))
                                                {
                                                    checkregex = false;
                                                }
                                            }
                                            if(checkregex)
                                            {
                                                foundmatch = (Regex.Match(capturedLine, doc.Key.SearchText, RegexOptions.IgnoreCase)).Success;
                                            }                                            
                                        }
                                        else
                                        {
                                            String ucaselog = capturedLine.ToUpper();
                                            String ucasetrigger = doc.Key.SearchText.ToUpper();
                                            foundmatch = ucaselog.Contains(ucasetrigger);
                                        }
                                        if (doc.Key.EndEarlyText.Count > 0)
                                        {
                                            Boolean endearly = false;
                                            foreach (SearchText earlyend in doc.Key.EndEarlyText)
                                            {
                                                if (earlyend.Regex)
                                                {
                                                    endearly = (Regex.Match(capturedLine, Regex.Escape(earlyend.Searchtext), RegexOptions.IgnoreCase)).Success;
                                                }
                                                else
                                                {
                                                    String ucaselog = capturedLine.ToUpper();
                                                    String ucasetrigger = earlyend.Searchtext.ToUpper();
                                                    endearly = ucaselog.Contains(ucasetrigger);
                                                }
                                            //TO DO: Probably implement extra stuff on a early end trigger
                                            if (endearly)
                                                {
                                                    Console.WriteLine($"Early end for {doc.Key.Name} => {endearly}");
                                                    ClearTimer(doc.Key);
                                                }
                                            }
                                        }
                                        if (foundmatch && doc.Value.Contains(character.Id))
                                        {
                                            if (stopfirstmatch)
                                            {
                                                state.Break();
                                            }
                                            Match eqline = GlobalVariables.eqRegex.Match(capturedLine);
                                            Console.WriteLine($"Matched Trigger {doc.Key.Id}");
                                            Stopwatch firetrigger = new Stopwatch();
                                            firetrigger.Start();
                                            FireTrigger(doc.Key, character, capturedLine);
                                            firetrigger.Stop();
                                            Console.WriteLine($"Fired Trigger in {firetrigger.Elapsed.Seconds}");
                                        }
                                    });
                                    stopwatch.Stop();
                                    //Console.WriteLine($"Trigger matched in: {stopwatch.Elapsed.ToString()}");
                                    if (pushbackToggle)
                                    {
                                        PushMonitor(capturedLine, character.ProfileName);
                                    }
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
        private void ClearTimer(Trigger activetrigger)
        {
            Console.WriteLine($"Clearing Timer for {activetrigger.TimerName}");
            Category triggeredcategory = categorycollection.Single<Category>(i => i.Id == activetrigger.TriggerCategory);
            lock (_timersLock)
            {
                switch (activetrigger.TimerType)
                {
                    case "Timer(Count Down)":
                        OverlayTimerWindow timerwindowdown = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                        syncontext.Post(new SendOrPostCallback(o =>
                        {
                            timerwindowdown.RemoveTimer(((Trigger)o).Id);
                        }), activetrigger);
                        break;
                    case "Stopwatch(Count Up)":
                        OverlayTimerWindow timerwindowup = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                        syncontext.Post(new SendOrPostCallback(o =>
                        {
                            timerwindowup.RemoveTimer(((Trigger)o).Id);
                        }), activetrigger);
                        break;
                    case "Repeating Timer":
                        break;
                    default:
                        break;
                }
            }
        }
        private void FireTrigger(Trigger activetrigger, CharacterProfile character, String matchline)
        {
            //Add stopwatch info for trigger
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ActivatedTrigger newactive = new ActivatedTrigger
            {
                Name = activetrigger.Name,
                FromLog = character.ProfileName,
                MatchText = matchline,
                Id = activetrigger.Id
            };
            //TO DO: Fix write by accessing calling thread
            if (logmatchestofile && logmatchlocation != "")
            {
                syncontext.Post(new SendOrPostCallback(o =>
                {
                    using (StreamWriter sw = File.AppendText((String)o))
                    {
                        AddText(sw, $"{character.ProfileName}[{activetrigger.Name}]-{matchline}");
                    }
                }),logmatchlocation);
            }
            lock (_triggerLock)
            {
                activatedTriggers.Add(newactive);
            }
            if (activetrigger.AudioSettings.AudioType == "tts" && soundenabled)
            { character.Speak(activetrigger.AudioSettings.TTS); }
            if (activetrigger.AudioSettings.AudioType == "file" && soundenabled)
            { PlaySound(activetrigger.AudioSettings.SoundFileId); }
            //Add Timer code
            Category triggeredcategory = categorycollection.Single<Category>(i => i.Id == activetrigger.TriggerCategory);
            String overlayname = triggeredcategory.TextOverlay;
            Stopwatch texttimer = new Stopwatch();
            texttimer.Start();
            if (activetrigger.Displaytext != null && textenabled)
            {
                OverlayTextWindow otw = textWindows.Single<OverlayTextWindow>(i => i.windowproperties.Name == triggeredcategory.TextOverlay);
                UpdateText(otw, activetrigger);
            }
            texttimer.Stop();
            Console.WriteLine($"Text: {texttimer.Elapsed.ToString()}");
            Stopwatch countertimer = new Stopwatch();
            countertimer.Start();
            /*TriggeredAgain
             * 0 - Start a new timer
             * 1 - Do Nothing
             */
            if(timerenabled && activetrigger.TriggeredAgain != 1)
            {
                lock (_timersLock)
                {
                    foreach (OverlayTimerWindow timerwindow in timerWindows)
                    {
                        syncontext.Post(new SendOrPostCallback(o =>
                        {
                            timerwindow.ContainsTimer((Trigger)o, true);
                        }), activetrigger);
                    }
                    switch (activetrigger.TimerType)
                    {
                        case "Timer(Count Down)":
                            OverlayTimerWindow timerwindowdown = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                            UpdateTimer(timerwindowdown, activetrigger, false, character.Name, triggeredcategory);
                            break;
                        case "Stopwatch(Count Up)":
                            OverlayTimerWindow timerwindowup = timerWindows.Single<OverlayTimerWindow>(i => i.windowproperties.Name == triggeredcategory.TimerOverlay);
                            UpdateTimer(timerwindowup, activetrigger, true, character.Name, triggeredcategory);
                            break;
                        case "Repeating Timer":
                            break;
                        default:
                            break;
                    }
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
        private static void AddText(TextWriter w, string value)
        {
            w.WriteLine(value);
        }
        #endregion
        #region Triggers
        private Trigger FindTrigger(int bsonid)
        {
            return dballtriggers.Find(x => x.Id == bsonid);
        }
        private void TriggerLoad(String callingfunction)
        {
            Console.WriteLine($"Calling TriggerLoad from {callingfunction}");
            Stopwatch loadwatch = new Stopwatch();
            loadwatch.Start();
            listoftriggers.Clear();
            foreach (Trigger trigger in dballtriggers)
            {
                listoftriggers.Add(trigger, trigger.profiles);
            }
            loadwatch.Stop();
            Console.WriteLine($"Loaded Triggers in {loadwatch.Elapsed}");
            //Update the masterlist of groups and triggers from database
            GenerateMasterList("TriggerLoad");
        }
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            TreeViewModel selectedGroup = (TreeViewModel)treeViewTriggers.SelectedItem;
            TriggerEditor newTrigger = new TriggerEditor(selectedGroup);
            newTrigger.Show();
            TriggerLoad("TriggerAdd_Click");
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
            TriggerLoad("RemoveTrigger");
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
            TriggerLoad("AddTrigger");
        }
        private void TriggerRemove_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteTrigger(root.Id);
                UpdateView();
                TriggerLoad("TriggerRemove_Click");
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
            TriggerLoad("DeleteTrigger by name");
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
            TriggerLoad("DeleteTrigger by id");
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
            TriggerLoad("CopyTrigger");
        }
        #endregion
        #region Trigger Groups
        private TriggerGroup FindTriggerGroup(int bsonid)
        {
            return dballgroups.Find(x => x.Id == bsonid);
        }
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
            GenerateMasterList("Delete TriggerGroup by name");
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
            GenerateMasterList("Delete TriggerGroup by id");
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
            GenerateMasterList("Copy TriggerGroup");
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
                    Boolean isChecked = false;
                    Trigger getTrigger = dballtriggers.Find(x => x.Id == item);
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
            return dballgroups.Find(x => x.Id == id);
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

            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                TreeView tree = sender as TreeView;
                TreeViewItem treeitem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (treeitem != null)
                {
                    TreeViewModel treemodel = (TreeViewModel)treeitem.Header;
                    DataObject dragdata = new DataObject("TreeViewModel", treemodel);
                    DragDrop.DoDragDrop(treeitem, dragdata, DragDropEffects.Move);
                }
            }
        }
        private void TreeViewTriggers_DragEnter(object sender, DragEventArgs e)
        {
            if (sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void TreeViewTriggers_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("TreeViewModel"))
            {
                TreeViewModel copytriggers = e.Data.GetData("TreeViewModel") as TreeViewModel;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ImportTriggers(copytriggers);
                stopwatch.Stop();
                Console.WriteLine($"Imported Triggers in {stopwatch.Elapsed.ToString()}");
            }
            if (e.Data.GetDataPresent("MainTree"))
            {
                TreeViewModel selectedbranch = e.Data.GetData("MainTree") as TreeViewModel;
                if (selectedbranch.Type == "trigger" && droptree.Id != 0 && selectedbranch.Name != droptree.Name)
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
                if (selectedbranch.Type == "triggergroup" && selectedbranch.Name != droptree.Name)
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
                if(toimport.UniqueId == "")
                {
                    toimport.UniqueId = Guid.NewGuid().ToString();
                }
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
                    foreach (int child in toimport.Children)
                    {
                        TriggerGroup childgroup = mergegroups.Find(x => x.Id == child);
                        int gid = ImportTriggerGroup(childgroup, bsonid);
                        newgroup.AddChild(gid);
                    }
                }
                if (toimport.Triggers.Count > 0)
                {
                    foreach (int child in toimport.Triggers)
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
                if(toimport.uniqueid == "")
                {
                    toimport.UniqueId = Guid.NewGuid().ToString();
                }
                bsonid = colTriggers.Insert(toimport);
                //Activate trigger for all profiles
                //AllProfileEnableTrigger(bsonid);
            }
            addedtriggers.Add(bsonid);
            return bsonid;
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
            if (hover.Type == "triggergroup" || hover.Name == "All Triggers")
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
            Setting darkmode = new Setting
            {
                Name = "DarkMode",
                Value = "false"
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
                settings.Insert(darkmode);
            }
        }
        private void PlaySound(string soundid)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                Stream soundfile = new System.IO.MemoryStream();
                db.FileStorage.Download($"{GlobalVariables.litedbfileprefix}{soundid}", soundfile);
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

            //Get Activated Triggers
            foreach (var doc in dballgroups)
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
                            Trigger getTrigger = dballtriggers.Find(x => x.Id == item);
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
            //Build Tree
            tv.Initialize();
            tv.VerifyCheckedState();
            treeViewTriggers.ItemsSource = treeView;
            TriggerLoad("UpdateTriggerView");
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
            switch (ribbonMain.SelectedTabIndex)
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
            GenerateMasterList("CategoryRemove_Click");
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
            if (root.Type == "trigger")
            {
                int bsonid = (dballtriggers.Find(x => x.Id == root.Id)).Id;
                TriggerEditor triggerDialog = new TriggerEditor(bsonid);
                triggerDialog.Show();
            }
            if (root.Type == "triggergroup")
            {
                TriggerGroup editgroup = dballgroups.Find(x => x.Id == root.Id);
                TriggerGroupEdit triggerDialog = new TriggerGroupEdit(editgroup);
                triggerDialog.Show();
                UpdateView();
            }
        }
        private void MenuItemTriggerPaste_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
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
            UpdateTriggerView();
        }
        private void TreeViewTriggers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            if (root != null)
            {
                if (root.Type == "trigger")
                {
                    int bsonid = FindTrigger(root.Id).Id;
                    TriggerEditor triggerDialog = new TriggerEditor(bsonid);
                    triggerDialog.Show();
                }
                if (root.Type == "triggergroup")
                {
                    TriggerGroup result = FindTriggerGroup(root.Id);
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
            TriggerLoad("cmAddTrigger_Click");
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
        private void ScheduledMaintenance(String logfile, Maintenance logmaintenance)
        {
            Boolean dailycheck = false;
            TimeSpan duration = DateTime.Now - logmaintenance.LastArchive;
            if (duration.Days > 1)
            {
                dailycheck = true;
            }
            if (dailycheck || ((DateTime.Now - logmaintenance.LastArchive).Days >= 7))
            {
                //check if archive folder exists and create it if missing
                if (!Directory.Exists(logmaintenance.ArchiveFolder))
                {
                    Directory.CreateDirectory(logmaintenance.ArchiveFolder);
                }
                //move file
                String filepostfix = DateTime.Now.ToString("MMddyyyy_HHmmss");
                Regex regexfilename = new Regex(@"(?<path>.*?\\)(?<filename>eqlog.*?)(?<extension>\.txt)", RegexOptions.Compiled);
                Match matchfile = regexfilename.Match(logfile);
                String newpath = "";
                if (matchfile.Success)
                {
                    newpath = logmaintenance.ArchiveFolder + @"\" + matchfile.Groups["filename"].Value.ToString() + "_" + filepostfix + ".txt";
                    File.Move(logfile, newpath);
                    //create new file
                    FileStream created = File.Create(logfile);
                    created.Close();
                }
            }
            if (logmaintenance.CompressArchive == "true")
            {
                CompressLog(logmaintenance.ArchiveFolder);
            }
            if (logmaintenance.AutoDelete == "true")
            {
                AutoDeleteArchive(logmaintenance.ArchiveFolder, logmaintenance.ArchiveDays);
            }

            Properties.Settings.Default.LastLogMaintenance = DateTime.Now;
            Properties.Settings.Default.Save();
        }
        private void AutoDeleteArchive(string archivefolder, int archivedays)
        {
            string[] archivelogs = Directory.GetFiles(archivefolder);
            foreach (string archivelog in archivelogs)
            {
                TimeSpan duration = DateTime.Now - File.GetCreationTime(archivelog);
                if (duration.Days > archivedays)
                {
                    File.Delete(archivelog);
                }
            }
        }
        private void CompressLog(string archivefolder)
        {
            //Go through all of the files and determine if it needs compressed
            string[] archivelogs = Directory.GetFiles(archivefolder);
            Regex regexfilename = new Regex(@"(?<directory>.*)\\(?<filename>.*\.txt)", RegexOptions.Compiled);
            foreach (string archivelog in archivelogs)
            {
                //create archive
                String newzip = archivelog.Replace(@".txt", @".zip");
                Match namematch = regexfilename.Match(archivelog);
                if (namematch.Success)
                {
                    using (FileStream fs = new FileStream(newzip, System.IO.FileMode.Create))
                    {
                        using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                        {
                            arch.CreateEntryFromFile(archivelog, namematch.Groups["filename"].Value.ToString());
                        }
                    }
                    //Delete log
                    File.Delete(archivelog);
                }
            }
        }
        #endregion
        #region Import Triggers
        public void ImportFromShare(string json)
        {
            dynamic back2json = JsonConvert.DeserializeObject(json);
            JToken token = ((JToken)back2json).SelectToken("rootnodes");
            string stop = "";
            //Go through each object
            //if trigger group -> check if the uniqueid matches any existing groups
            //  if match -> update any group info else create new group
            //if trigger -> check if the uniqueid matches any existing triggers
            //  if match -> update any trigger info else create new trigger
        }
        public void ImportZip(string filename, Boolean share)
        {
            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                //Load the json
                if (archive.Entries.Count > 0)
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name == "DataExport.json")
                        {
                            using (StreamReader streamtriggers = new StreamReader(entry.Open()))
                            {
                                if (share)
                                {
                                    ImportFromShare(streamtriggers.ReadToEnd());
                                }
                                else
                                {
                                    ImportFromExternal(streamtriggers.ReadToEnd());
                                }
                            }
                        }
                        else if (entry.Name.Contains(@".wav"))
                        {
                            entry.ExtractToFile($"{GlobalVariables.defaultPath}\\ImportedSounds\\{entry.Name}");
                        }
                    }
                }
            }
        }
        private void ImportClick()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "EQ Audio Trigger Package|*.zip";
            if (fileDialog.ShowDialog() == true)
            {
                ImportZip(fileDialog.FileName,false);
            }
        }
        private void SplitButtonImport_Click(object sender, RoutedEventArgs e)
        {
            ImportClick();
        }
        private void ImportAudioTriggers_Click(object sender, RoutedEventArgs e)
        {
            ImportClick();
        }
        private void ImportFromExternal(string json)
        {
            ClearMergeInfo();
            mergeview = new TreeViewModel("Triggers to Import");
            mergeview.IsChecked = false;
            mergetreeView.Add(mergeview);
            dynamic back2json = JsonConvert.DeserializeObject(json);
            JToken token = ((JToken)back2json).SelectToken("rootnodes");
            int result = BuildImport(token, triggergroupid);
            InitializeTree();
        }
        private void ClearMergeInfo()
        {
            triggergroupid = 0;
            mergetriggercount = 0;
            mergetriggers.Clear();
            mergegroups.Clear();
            mergetreeView.Clear();
        }
        private int BuildImport(JToken json, int parentid)
        {
            int rval = ++triggergroupid;
            //Check if its an array or object
            if (json.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
            {
                foreach(JToken group in json)
                {
                    TriggerGroup newgroup = new TriggerGroup
                    {
                        TriggerGroupName = (string)group.SelectToken("TriggerGroupName"),
                        Id = triggergroupid,
                        Parent = parentid
                    };
                    JToken comments = group.SelectToken("Comments");
                    if (comments != null)
                    {
                        newgroup.Comments = (string)comments;
                    }
                    mergegroups.Add(newgroup);
                    foreach (JToken token in group["children"])
                    {                        
                        newgroup.Children.Add(BuildImport(token, triggergroupid));
                    }
                    JToken triggers = group.SelectToken("triggers");
                    if(triggers != null)
                    {
                        foreach (JToken triggertoken in triggers)
                        {
                            newgroup.Triggers.Add(BuildTrigger(triggertoken, triggergroupid));
                        }
                    }
                }
            }
            else if (json.GetType().ToString() == "Newtonsoft.Json.Linq.JObject")
            {
                TriggerGroup newgroup = new TriggerGroup
                {
                    TriggerGroupName = (string)json.SelectToken("TriggerGroupName"),
                    Id = triggergroupid,
                    Parent = parentid
                };
                JToken comments = json.SelectToken("Comments");
                if (comments != null)
                {
                    newgroup.Comments = (string)comments;
                }
                mergegroups.Add(newgroup);
                foreach (JToken token in json["children"])
                {
                    newgroup.Children.Add(BuildImport(token, triggergroupid));
                }
                JToken triggers = json.SelectToken("triggers");
                if (triggers != null)
                {
                    foreach (JToken triggertoken in triggers)
                    {
                        newgroup.Triggers.Add(BuildTrigger(triggertoken, triggergroupid));
                    }
                }
            }

            return rval;
        }
        private int BuildTrigger(JToken jsontoken, int parentid)
        {
            int rval = triggerid;
            Trigger newtrigger = new Trigger
            {
                id = triggerid,
                name = jsontoken["name"].ToString(),
                profiles = new ArrayList(),
                searchText = (String)jsontoken["searchText"],
                comments = (String)jsontoken["comments"],
                regex = (bool)jsontoken["regex"],
                fastcheck = (bool)jsontoken["fastcheck"],
                parent = parentid,
                triggerCategory = 0,
                displaytext = (String)jsontoken["displaytext"],
                clipboardtext = (String)jsontoken["clipboardtext"],
                audioSettings = new Audio(),
                timerType = (String)jsontoken["timerType"],
                timerName = (String)jsontoken["timerName"],
                timerDuration = (int)jsontoken["timerDuration"],
                triggeredAgain = (int)jsontoken["triggeredAgain"],
                endEarlyText = new BindingList<SearchText>(),
                timerEndingDuration = (int)jsontoken["timerEndingDuration"],
                timerEndingDisplayText = (String)jsontoken["timerEndingDisplayText"],
                timerEndingClipboardText = (String)jsontoken["timerEndingClipboardText"],
                timerEndingAudio = new Audio(),
                timerEnding = (bool)jsontoken["timerEnding"],
                timerEndedClipboardText = (String)jsontoken["timerEndedClipboardText"],
                timerEndedDisplayText = (String)jsontoken["timerEndedDisplayText"],
                timerEnded = (bool)jsontoken["timerEnded"],
                timerEndedAudio = new Audio(),
                resetCounter = (bool)jsontoken["resetCounter"],
                resetCounterDuration = (int)jsontoken["resetCounterDuration"],
            };
            newtrigger.audioSettings.AudioType = (String)jsontoken["audioSettings"]["audioType"];
            newtrigger.audioSettings.TTS = (String)jsontoken["audioSettings"]["tts"];
            newtrigger.audioSettings.Interrupt = (bool)jsontoken["audioSettings"]["interrupt"];
            newtrigger.audioSettings.SoundFileId = (String)jsontoken["audioSettings"]["soundfile"];
            newtrigger.TimerEndingAudio.AudioType = (String)jsontoken["timerEndingAudio"]["audioType"];
            newtrigger.TimerEndingAudio.TTS = (String)jsontoken["timerEndingAudio"]["tts"];
            newtrigger.TimerEndingAudio.Interrupt = (bool)jsontoken["timerEndingAudio"]["interrupt"];
            newtrigger.TimerEndingAudio.SoundFileId = (String)jsontoken["timerEndingAudio"]["soundfile"];
            newtrigger.TimerEndedAudio.AudioType = (String)jsontoken["timerEndedAudio"]["audioType"];
            newtrigger.TimerEndedAudio.TTS = (String)jsontoken["timerEndedAudio"]["tts"];
            newtrigger.TimerEndedAudio.Interrupt = (bool)jsontoken["timerEndedAudio"]["interrupt"];
            newtrigger.TimerEndedAudio.SoundFileId = (String)jsontoken["timerEndedAudio"]["soundfile"];
            foreach(JToken earlytoken in jsontoken["endEarlyText"])
            {
                SearchText st = new SearchText
                {
                    regexEnabled = (bool)earlytoken["regexEnabled"],
                    Searchtext = (String)earlytoken["searchtext"]
                };
                newtrigger.EndEarlyText.Add(st);                
            }
            mergetriggers.Add(newtrigger);
            triggerid++;
            return rval;
        }
        private void ImportFromGINA_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "GINA Trigger Package|*.gtp";
            if (fileDialog.ShowDialog() == true)
            {
                using (ZipArchive archive = ZipFile.OpenRead(fileDialog.FileName))
                {
                    //Load the xml
                    if (archive.Entries.Count > 0)
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.Name == "ShareData.xml")
                            {
                                ZipArchiveEntry triggersxml = entry;
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
                            if (entry.Name.Contains("wav"))
                            {
                                //Check if EQAudioTriggers folder exists, if not create.
                                bool mainPath = Directory.Exists($"{GlobalVariables.defaultPath}\\ImportedSounds");
                                if (!mainPath)
                                {
                                    Directory.CreateDirectory($"{GlobalVariables.defaultPath}\\ImportedSounds");
                                }
                                //export file to export sound dir
                                String extracttofile = $"{GlobalVariables.defaultPath}\\ImportedSounds\\{entry.Name}";
                                entry.ExtractToFile(extracttofile);
                            }
                        }
                    }

                }
            }
        }
        private void InitializeTree()
        {
            //build tree
            foreach (TriggerGroup tg in mergegroups)
            {
                if (tg.Parent == 0)
                {
                    TreeViewModel rTree = new TreeViewModel(tg.TriggerGroupName)
                    {
                        Type = "triggergroup",
                        Id = tg.Id
                    };
                    if (tg.Triggers.Count > 0)
                    {
                        foreach (int item in tg.Triggers)
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
                    if (tg.Children.Count > 0)
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
            buttonDoneMerge.Visibility = Visibility.Visible;
        }
        private void ParseGina(JToken jsontoken)
        {
            ClearMergeInfo();
            mergeview = new TreeViewModel("Triggers to Import");
            mergeview.IsChecked = false;
            mergetreeView.Add(mergeview);
            int result = GetTriggerGroups(jsontoken.SelectToken("TriggerGroups.TriggerGroup"), triggergroupid);
            InitializeTree();
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
                switch (((JProperty)token).Name)
                {
                    case "TriggerGroups":
                        if ((jsontoken["TriggerGroups"]["TriggerGroup"]).GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                        {
                            foreach (JToken newtoken in ((JArray)(jsontoken["TriggerGroups"]["TriggerGroup"])).Children())
                            {
                                triggergroupid++;
                                newgroup.Children.Add(GetTriggerGroups(newtoken, triggergroupid));
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
                case "DoNothing":
                    newtrigger.TriggeredAgain = 1;
                    break;
                case "StartNewTimer":
                    newtrigger.TriggeredAgain = 0;
                    break;
                case "RestartTimer":
                    newtrigger.TriggeredAgain = 0;
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
            if (jsontoken["TimerEndedTrigger"] != null)
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
            //clear addedtriggers list
            addedtriggers.Clear();
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
                //If we're importing a trigger group, walk through the tree
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
                    foreach (TriggerGroup tg in mergegroups)
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
            //Enable all imported Triggers on all profiles
            if (addedtriggers.Count > 0)
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                    var colTriggers = db.GetCollection<Trigger>("triggers");

                    //Activate for every profile
                    IEnumerable<CharacterProfile> profiles = colProfiles.FindAll();
                    foreach (CharacterProfile profile in profiles)
                    {
                        //Add the profile to the trigger
                        //add the trigger to the profile
                        foreach (int bsonid in addedtriggers)
                        {
                            Trigger trigger = colTriggers.FindById(bsonid);
                            trigger.Profiles.Add(profile.Id);
                            colTriggers.Update(trigger);
                            profile.AddTrigger(bsonid);
                        }
                        //update the profile when all the triggers are done.
                        colProfiles.Update(profile);
                    }
                }
            }
            //Delete Imported triggers/groups from import tree
            TreeViewModel toremove = new TreeViewModel("todelete");
            foreach (TreeViewModel tvm in mergetreeView)
            {
                DeleteBranch(tvm, importtree.Id);
                if (tvm.Id == importtree.Id)
                {
                    toremove = tvm;
                }
            }
            if (toremove.Name != "todelete")
            {
                mergetreeView.Remove(toremove);
            }
            treemerge.ItemsSource = mergetreeView;
            //Once we're done with the import, update the trigger view
            UpdateListView();
            UpdateTriggerView();
            TriggerLoad("ImportTriggers");
            //Keep the merge tree up until the user clears it
            if (mergetreeView.Count > 0)
            {
                treemerge.Visibility = Visibility.Visible;
                buttonDoneMerge.Visibility = Visibility.Visible;
            }
            else
            {
                treemerge.Visibility = Visibility.Hidden;
                buttonDoneMerge.Visibility = Visibility.Hidden;
            }
        }
        private void DeleteBranch(TreeViewModel tvm, int idtodelete)
        {
            TreeViewModel deletetree = new TreeViewModel("deltree");
            foreach (TreeViewModel child in tvm.Children)
            {
                if (child.Children.Count > 0)
                {
                    DeleteBranch(child, idtodelete);
                }
                if (child.Id == idtodelete)
                {
                    deletetree = child;
                }
            }
            if (deletetree.Name != "deltree")
            {
                tvm.RemoveChild(deletetree);
            }
        }
        private void ButtonDoneMerge_Click(object sender, RoutedEventArgs e)
        {
            ClearMergeInfo();
            treemerge.Visibility = Visibility.Hidden;
            buttonDoneMerge.Visibility = Visibility.Hidden;
        }
        #endregion
        #region Export Triggers
        private JObject ExportGroup(int groupid)
        {
            TriggerGroup group = new TriggerGroup();
            JArray triggers = new JArray();
            JArray subgroups = new JArray();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<TriggerGroup> triggergroups = db.GetCollection<TriggerGroup>("triggergroups");
                group = triggergroups.FindById(groupid);
                
                if (group.Triggers.Count > 0)
                {
                    foreach (int triggerid in group.Triggers)
                    {
                        triggers.Add(GetExportTrigger(triggerid));
                    }
                }
                
                if (group.Children.Count > 0)
                {
                    foreach (int subgroupid in group.Children)
                    {
                        subgroups.Add(ExportGroup(subgroupid));
                    }
                }
            }
            JObject rval = new JObject(
                new JProperty("Id",group.Id),
                new JProperty("Type","triggergroup"),
                new JProperty("TriggerGroupName",group.TriggerGroupName),
                new JProperty("Comments",group.Comments),
                new JProperty("DefaultEnabled",group.DefaultEnabled),
                new JProperty("Parent",group.Parent),
                new JProperty("children",subgroups),
                new JProperty("triggers", triggers)                
                );
            return rval;
        }
        private JObject GetExportTrigger(int triggerid)
        {
            Trigger exporttrigger = new Trigger();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                exporttrigger = triggers.FindById(triggerid);
            }
            if(exporttrigger.AudioSettings.SoundFileId != null)
            {
                exportsounds.Add(exporttrigger.audioSettings.SoundFileId);
            }
            JObject rval = new JObject(
                new JProperty("type","trigger"),
                new JProperty("id",exporttrigger.Id),
                new JProperty("name",exporttrigger.Name),
                new JProperty("searchText",exporttrigger.SearchText),
                new JProperty("comments",exporttrigger.Comments),
                new JProperty("regex",exporttrigger.Regex),
                new JProperty("fastcheck",exporttrigger.Fastcheck),
                new JProperty("parent",exporttrigger.Parent),
                new JProperty("triggerCategory",exporttrigger.TriggerCategory),
                new JProperty("displaytext",exporttrigger.Displaytext),
                new JProperty("clipboardtext",exporttrigger.Clipboardtext),
                new JProperty("audioSettings", 
                    new JObject(
                        new JProperty("audioType",exporttrigger.AudioSettings.AudioType),
                        new JProperty("tts",exporttrigger.AudioSettings.TTS),
                        new JProperty("interrupt",exporttrigger.AudioSettings.Interrupt),
                        new JProperty("soundfile",exporttrigger.AudioSettings.SoundFileId)
                        )
                    ),
                new JProperty("timerType",exporttrigger.TimerType),
                new JProperty("timerName",exporttrigger.TimerName),
                new JProperty("timerDuration",exporttrigger.TimerDuration),
                new JProperty("triggeredAgain",exporttrigger.TriggeredAgain),
                new JProperty("endEarlyText", 
                    new JArray(
                        from p in exporttrigger.EndEarlyText
                        select new JObject(
                            new JProperty("searchtext",p.Searchtext),
                            new JProperty("regexEnabled",p.Regex)
                            )
                        )
                    ),
                new JProperty("timerEndingDuration",exporttrigger.TimerEndingDuration),
                new JProperty("timerEndingDisplayText",exporttrigger.TimerEndingDisplayText),
                new JProperty("timerEndingClipboardText",exporttrigger.TimerEndingClipboardText),
                new JProperty("timerEndingAudio", 
                    new JObject(    
                        new JProperty("audioType", exporttrigger.TimerEndingAudio.AudioType),
                        new JProperty("tts", exporttrigger.TimerEndingAudio.TTS),
                        new JProperty("interrupt", exporttrigger.TimerEndingAudio.Interrupt),
                        new JProperty("soundfile", exporttrigger.TimerEndingAudio.SoundFileId)
                        )
                    ),
                new JProperty("timerEnding",exporttrigger.TimerEnding),
                new JProperty("timerEndedDisplayText",exporttrigger.TimerEndedDisplayText),
                new JProperty("timerEndedClipboardText",exporttrigger.TimerEndedClipboardText),
                new JProperty("timerEnded",exporttrigger.TimerEnded),
                new JProperty("timerEndedAudio",
                    new JObject(
                        new JProperty("audioType", exporttrigger.TimerEndedAudio.AudioType),
                        new JProperty("tts", exporttrigger.TimerEndedAudio.TTS),
                        new JProperty("interrupt", exporttrigger.TimerEndedAudio.Interrupt),
                        new JProperty("soundfile", exporttrigger.TimerEndedAudio.SoundFileId)
                        )
                    ),
                new JProperty("resetCounter",exporttrigger.resetCounter),
                new JProperty("resetCounterDuration",exporttrigger.ResetCounterDuration)
                );
            string stop = "";
            return rval;
        }
        private Boolean ExportTriggers(TreeViewModel startnode, Boolean share)
        {
            Boolean rval = false;
            //Get the folder location to export to
            String exportfolder = GlobalVariables.defaultPath;
            if(!share)
            {
                exportfolder = SelectFolder(textboxDataFolder.Text);
            }            
            String zipname = "";
            if (exportfolder != "false")
            {
                rval = true;
                //build the zip file name
                StringBuilder sb = new StringBuilder();
                sb.Append(@"dataexport_");
                sb.Append(DateTime.Now.ToString("MMddyyyy_HHmmss"));
                sb.Append(@".zip");
                //build the json
                JArray rootnodes = new JArray();
                if (startnode.Type == "triggergroup")
                {
                    JObject jobject = new JObject(
                        new JProperty("TriggerGroupName", startnode.Name),
                        new JProperty("Type", "triggergroup"),
                        new JProperty("Id", 0),
                        new JProperty("children", new JArray()),
                        new JProperty("triggers", new JArray())
                        );
                    foreach (TreeViewModel child in startnode.Children)
                    {
                        Console.WriteLine($"Exporting {child.Name}");
                        if(child.Type == "triggergroup")
                        {
                            ((JArray)jobject["children"]).Add(ExportGroup(child.Id));
                        }
                        else if(child.Type == "trigger")
                        {
                            ((JArray)jobject["triggers"]).Add(GetExportTrigger(child.Id));
                        }

                    }
                    rootnodes.Add(jobject);
                }
                else if (startnode.Id == 0)
                {
                    foreach (TreeViewModel child in startnode.Children)
                    {
                        Console.WriteLine($"Exporting {child.Name}");
                        rootnodes.Add(ExportGroup(child.Id));
                    }
                }
                else if (startnode.Type == "trigger")
                {
                    Console.WriteLine($"Exporting single trigger {startnode.Name}");
                    rootnodes.Add(GetExportTrigger(startnode.Id));
                }

                JObject exportjson = new JObject(
                    new JProperty("rootnodes", rootnodes));
                string json = JsonConvert.SerializeObject(exportjson);
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        if (exportsounds.Count > 0)
                        {
                            foreach (String soundid in exportsounds)
                            {
                                if (soundid != "")
                                {
                                    using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                                    {
                                        LiteFileInfo file = db.FileStorage.FindById($"{GlobalVariables.litedbfileprefix}{soundid}");
                                        var soundfile = archive.CreateEntry(soundid);
                                        using (var soundstream = soundfile.Open())
                                        {
                                            file.CopyTo(soundstream);
                                        }
                                    }
                                }
                            }
                        }
                        var newfile = archive.CreateEntry("DataExport.json");

                        using (var entryStream = newfile.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(json);
                            }
                        }
                    }
                    zipname = sb.ToString();
                    String newzip = exportfolder + @"\" + zipname;
                    using (var fileStream = new FileStream(newzip, System.IO.FileMode.Create))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                }
                //Reset the export sound list
                exportsounds.Clear();
                if(share)
                {
                    Guid guid = Guid.NewGuid();
                    String token = Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", Properties.Settings.Default.ApiUsername, Properties.Settings.Default.ApiPassword)));
                    //Send Rest call to insert package
                    var client = new RestClient($"http://{GlobalVariables.apiserver}{GlobalVariables.restbase}/package");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("accept", "application/json");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("Authorization", $"Basic {token}");
                    Package package = new Package
                    {
                        Guid = guid.ToString(),
                        Payload = zipname
                    };
                    request.AddJsonBody(package);
                    IRestResponse response = client.Execute(request);
                    dynamic responsetoken = JsonConvert.DeserializeObject(response.Content);

                    //Send Rest call to insert payload
                    var payloadclient = new RestClient($"http://{GlobalVariables.apiserver}{GlobalVariables.restbase}/payload");
                    var payloadrequest = new RestRequest(Method.POST);
                    payloadrequest.AddFile("file",exportfolder + "\\" + zipname);
                    payloadrequest.AddHeader("Authorization", $"Basic {token}");
                    IRestResponse payloadresponse = payloadclient.Execute(payloadrequest);
                    dynamic payloadtoken = JsonConvert.DeserializeObject(response.Content);
                    //return the guid
                    shareguid = @"{HEAP:" + guid.ToString() + @"}";
                    Clipboard.SetText(shareguid);
                    Console.WriteLine($"{shareguid}");
                    //delete the zip file
                    File.Delete(exportfolder + zipname);
                }
            }
            return rval;
        }
        private void Export(Boolean share)
        {
            Boolean export = false;
            if ((TreeViewModel)treeViewTriggers.SelectedItem != null)
            {
                export = ExportTriggers((TreeViewModel)treeViewTriggers.SelectedItem, share);
            }
            else
            {
                export = ExportTriggers(treeView[0],share);
            }
            if(export && !share)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Export Complete", "Data Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if(export && share)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"{shareguid}", "Share Upload", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Export(false);
        }
        private void CmExportFile_Click(object sender, RoutedEventArgs e)
        {
            Export(false);
        }
        #endregion
        #region Fluent Backstage
        private String SelectFolder(String currentpath)
        {
            String rval = currentpath;
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = currentpath;
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                rval = folderDialog.SelectedPath;
            }
            else
            {
                rval = "false";
            }
            return rval;
        }
        private void ButtonLoadArchive_Click(object sender, RoutedEventArgs e)
        {
            String archive = SelectFolder(textboxArchiveFolder.Text);
            if(archive != "false")
            {
                textboxArchiveFolder.Text = archive;
            }            
        }
        private void ButtonSaveArchive_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                //update settings
                Setting autoarchive = settings.FindOne(Query.EQ("Name", "AutoArchive"));
                autoarchive.Value = checkboxAutoArchive.IsChecked.ToString();
                settings.Update(autoarchive);
                Setting archivefolder = settings.FindOne(Query.EQ("Name", "LogArchiveFolder"));
                archivefolder.Value = textboxArchiveFolder.Text;
                settings.Update(archivefolder);
                Setting archiveschedule = settings.FindOne(Query.EQ("Name", "ArchiveSchedule"));
                archiveschedule.Value = comboboxArchiveSchedule.Text;
                settings.Update(archiveschedule);
                Setting autodelete = settings.FindOne(Query.EQ("Name", "AutoDelete"));
                autodelete.Value = checkboxDeleteArchive.IsChecked.ToString();
                settings.Update(autodelete);
                Setting compressarchive = settings.FindOne(Query.EQ("Name", "CompressArchive"));
                compressarchive.Value = checkboxCompress.IsChecked.ToString();
                settings.Update(compressarchive);
                Setting archivedays = settings.FindOne(Query.EQ("Name", "DeleteArchives"));
                archivedays.Value = textboxArchiveDays.Text;
                settings.Update(archivedays);
            }
        }
        private void ButtonSaveShare_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                Setting shareuri = settings.FindOne(Query.EQ("Name", "ShareServiceURI"));
                shareuri.Value = textboxShareURI.Text;
                settings.Update(shareuri);
                Setting selfref = settings.FindOne(Query.EQ("Name", "Reference"));
                selfref.Value = textboxSelfReference.Text;
                settings.Update(selfref);
                Setting enabledebug = settings.FindOne(Query.EQ("Name", "EnableDebug"));
                enabledebug.Value = checkboxShareDebug.IsChecked.ToString();
                settings.Update(enabledebug);
            }
        }
        private void LoadSettingsTab()
        {
            textboxArchiveFolder.Text = logmaintenance.ArchiveFolder = programsettings.Single<Setting>(i => i.Name == "LogArchiveFolder").Value;
            logmaintenance.ArchiveFolder = programsettings.Single<Setting>(i => i.Name == "LogArchiveFolder").Value;
            comboboxArchiveSchedule.Text = logmaintenance.ArchiveSchedule = programsettings.Single<Setting>(i => i.Name == "ArchiveSchedule").Value;
            logmaintenance.AutoDelete = programsettings.Single<Setting>(i => i.Name == "AutoDelete").Value;
            logmaintenance.CompressArchive = programsettings.Single<Setting>(i => i.Name == "CompressArchive").Value;
            logmaintenance.ArchiveDays = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "DeleteArchives").Value);
            logmaintenance.LastArchive = Properties.Settings.Default.LastLogMaintenance;
            checkboxDarkmode.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "DarkMode").Value);
            checkboxDeleteArchive.IsChecked = Convert.ToBoolean(logmaintenance.AutoDelete);
            logmaintenance.AutoArchive = programsettings.Single<Setting>(i => i.Name == "AutoArchive").Value;
            checkboxAutoArchive.IsChecked = Convert.ToBoolean(logmaintenance.AutoArchive);
            checkboxCompress.IsChecked = Convert.ToBoolean(logmaintenance.CompressArchive);
            textboxArchiveDays.Text = logmaintenance.ArchiveDays.ToString();
            textboxShareURI.Text = programsettings.Single<Setting>(i => i.Name == "ShareServiceURI").Value;
            textboxSelfReference.Text = programsettings.Single<Setting>(i => i.Name == "Reference").Value;
            checkboxShareDebug.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "EnableDebug").Value);
            checkboxEnableSharing.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "SharingEnabled").Value);
            int shareindex = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "AcceptInvitationsFrom").Value);
            switch (shareindex)
            {
                case 0:
                    radioShareNobody.IsChecked = true;
                    break;
                case 1:
                    radioShareTrusted.IsChecked = true;
                    break;
                case 2:
                    radioShareAnybody.IsChecked = true;
                    break;
                default:
                    radioShareNobody.IsChecked = true;
                    break;
            }
            int mergeindex = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "MergeFrom").Value);
            switch (mergeindex)
            {
                case 0:
                    radioMergeNobody.IsChecked = true;
                    break;
                case 1:
                    radioMergeTrusted.IsChecked = true;
                    break;
                case 2:
                    radioMergeAnybody.IsChecked = true;
                    break;
                default:
                    radioMergeNobody.IsChecked = true;
                    break;
            }
            String senders = programsettings.Single<Setting>(i => i.Name == "TrustedSenderList").Value;
            if(senders != "")
            {
                string[] senderarray = senders.Split(',');
                foreach (string sender in senderarray)
                {
                    trustedsenders.Add(sender);
                }
            }
            listviewSenderList.ItemsSource = trustedsenders;
            checkboxSoundEnable.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "EnableSound").Value);
            sliderMasterVol.Value = Convert.ToInt32(programsettings.Single<Setting>(i => i.Name == "MasterVolume").Value);
            checkboxEnableText.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "EnableText").Value);
            checkboxEnableTimer.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "EnableTimers").Value);
            checkboxStopTrigger.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "StopTriggerSearch").Value);
            checkboxMinimize.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "Minimize").Value);
            checkboxMatchLog.IsChecked = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "DisplayMatchLog").Value);
            checkboxLogMatches.IsChecked = logmatchestofile = Convert.ToBoolean(programsettings.Single<Setting>(i => i.Name == "LogMatchesToFile").Value);
            textboxLogMatches.Text = logmatchlocation = programsettings.Single<Setting>(i => i.Name == "LogMatchFilename").Value;
            textboxClipboard.Text = programsettings.Single<Setting>(i => i.Name == "Clipboard").Value;
            textboxEQFolder.Text = programsettings.Single<Setting>(i => i.Name == "EQFolder").Value;
            textboxMediaFolder.Text = programsettings.Single<Setting>(i => i.Name == "ImportedMediaFolder").Value;
            textboxDataFolder.Text = programsettings.Single<Setting>(i => i.Name == "DataFolder").Value;
            textboxMaxEntries.Text = programsettings.Single<Setting>(i => i.Name == "MaxLogEntry").Value;
        }
        private void ButtonSaveSharing_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                Setting enablesharing = settings.FindOne(Query.EQ("Name", "SharingEnabled"));
                enablesharing.Value = checkboxEnableSharing.IsChecked.ToString();
                settings.Update(enablesharing);
                Setting acceptfrom = settings.FindOne(Query.EQ("Name", "AcceptInvitationsFrom"));
                //Get which radio button is selected
                if ((bool)radioShareNobody.IsChecked)
                {
                    acceptfrom.Value = "0";
                }
                if ((bool)radioShareTrusted.IsChecked)
                {
                    acceptfrom.Value = "1";
                }
                if ((bool)radioShareAnybody.IsChecked)
                {
                    acceptfrom.Value = "2";
                }
                settings.Update(acceptfrom);
                Setting mergefrom = settings.FindOne(Query.EQ("Name", "MergeFrom"));
                if ((bool)radioMergeNobody.IsChecked)
                {
                    mergefrom.Value = "0";
                }
                if ((bool)radioMergeTrusted.IsChecked)
                {
                    mergefrom.Value = "1";
                }
                if ((bool)radioMergeAnybody.IsChecked)
                {
                    mergefrom.Value = "2";
                }
                settings.Update(mergefrom);
                Setting trustedlist = settings.FindOne(Query.EQ("Name", "TrustedSenderList"));
                trustedlist.Value = String.Join(",", trustedsenders.Select(x => x.ToString()).ToArray());
                settings.Update(trustedlist);
            }
        }
        private void ButtonRemoveSender_Click(object sender, RoutedEventArgs e)
        {
            fluentbackstage.IsOpen = true;
            if (listviewSenderList.SelectedItem != null)
            {
                trustedsenders.Remove(listviewSenderList.SelectedItem.ToString());
            }
        }
        private void ButtonAddSender_Click(object sender, RoutedEventArgs e)
        {
            fluentbackstage.IsOpen = true;
            if (textboxAddSender.Text != null)
            {
                trustedsenders.Add(textboxAddSender.Text);
                textboxAddSender.Text = "";
            }
        }
        private void ButtonSaveGeneral_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkboxLogMatches.IsChecked && textboxLogMatches.Text == "")
            {
                buttonSaveGeneral.IsDefinitive = false;
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid Match Log Location!", "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using (var db = new LiteDatabase(GlobalVariables.defaultDB))
                {
                    LiteCollection<Setting> settings = db.GetCollection<Setting>("settings");
                    Setting enablesound = settings.FindOne(Query.EQ("Name", "EnableSound"));
                    enablesound.Value = checkboxSoundEnable.IsChecked.ToString();
                    settings.Update(enablesound);
                    soundenabled = (bool)checkboxSoundEnable.IsChecked;
                    Setting mastervol = settings.FindOne(Query.EQ("Name", "MasterVolume"));
                    mastervol.Value = sliderMasterVol.Value.ToString();
                    settings.Update(mastervol);
                    mastervolume = Convert.ToInt32(sliderMasterVol.Value);
                    UpdateVolume();
                    Setting enabletext = settings.FindOne(Query.EQ("Name", "EnableText"));
                    enabletext.Value = checkboxEnableText.IsChecked.ToString();
                    settings.Update(enabletext);
                    textenabled = (bool)checkboxEnableText.IsChecked;
                    Setting enabletimers = settings.FindOne(Query.EQ("Name", "EnableTimers"));
                    enabletimers.Value = checkboxEnableTimer.IsChecked.ToString();
                    settings.Update(enabletimers);
                    timerenabled = (bool)checkboxEnableTimer.IsChecked;
                    Setting stoptrigger = settings.FindOne(Query.EQ("Name", "StopTriggerSearch"));
                    stoptrigger.Value = checkboxStopTrigger.IsChecked.ToString();
                    settings.Update(stoptrigger);
                    stopfirstmatch = (bool)checkboxStopTrigger.IsChecked;
                    Setting minimize = settings.FindOne(Query.EQ("Name", "Minimize"));
                    minimize.Value = checkboxMinimize.IsChecked.ToString();
                    settings.Update(minimize);
                    Setting displaymatch = settings.FindOne(Query.EQ("Name", "DisplayMatchLog"));
                    displaymatch.Value = checkboxMatchLog.IsChecked.ToString();
                    settings.Update(displaymatch);
                    Setting logmatches = settings.FindOne(Query.EQ("Name", "LogMatchesToFile"));
                    logmatches.Value = checkboxLogMatches.IsChecked.ToString();
                    settings.Update(logmatches);
                    logmatchestofile = (bool)checkboxLogMatches.IsChecked;
                    Setting logmatchfilename = settings.FindOne(Query.EQ("Name", "LogMatchFilename"));
                    logmatchfilename.Value = textboxLogMatches.Text;
                    settings.Update(logmatchfilename);
                    Setting clipboard = settings.FindOne(Query.EQ("Name", "Clipboard"));
                    clipboard.Value = textboxClipboard.Text;
                    settings.Update(clipboard);
                    Setting eqfolder = settings.FindOne(Query.EQ("Name", "EQFolder"));
                    eqfolder.Value = textboxEQFolder.Text;
                    settings.Update(eqfolder);
                    Setting importmedia = settings.FindOne(Query.EQ("Name", "ImportedMediaFolder"));
                    importmedia.Value = textboxMediaFolder.Text;
                    settings.Update(importmedia);
                    Setting datafolder = settings.FindOne(Query.EQ("Name", "DataFolder"));
                    datafolder.Value = textboxDataFolder.Text;
                    settings.Update(datafolder);
                    Setting maxlogentry = settings.FindOne(Query.EQ("Name", "MaxLogEntry"));
                    maxlogentry.Value = textboxMaxEntries.Text;
                    settings.Update(maxlogentry);
                    Setting darkmode = settings.FindOne(Query.EQ("Name", "DarkMode"));
                    darkmode.Value = checkboxDarkmode.IsChecked.ToString();
                    settings.Update(darkmode);
                }
            }            
        }
        private void ButtonEQFolder_Click(object sender, RoutedEventArgs e)
        {
            String eqfolder = SelectFolder(textboxEQFolder.Text);
            if(eqfolder != "false")
            {
                textboxEQFolder.Text = eqfolder;
            }            
        }
        private void ButtonMediaFolder_Click(object sender, RoutedEventArgs e)
        {
            String mediafolder = SelectFolder(textboxMediaFolder.Text);
            if(mediafolder != "false")
            {
                textboxMediaFolder.Text = "false";
            }            
        }
        private void ButtonDataFolder_Click(object sender, RoutedEventArgs e)
        {
            String datafolder = SelectFolder(textboxDataFolder.Text);
            if(datafolder != "false")
            {
                textboxDataFolder.Text = datafolder;
            }            
        }
        private void CheckboxLogMatches_Checked(object sender, RoutedEventArgs e)
        {
            textboxLogMatches.Text = $"{textboxDataFolder.Text}\\triggermatches.log";
        }
        private void CheckboxLogMatches_Unchecked(object sender, RoutedEventArgs e)
        {
            textboxLogMatches.Text = "";
        }
        private void CheckboxDarkmode_Checked(object sender, RoutedEventArgs e)
        {
            Vs2013LightTheme lighttheme = new Vs2013LightTheme();
            dockingmanager.Theme = lighttheme;
            Fluent.ThemeManager.ChangeTheme(this, "Light.Blue");
        }
        private void CheckboxDarkmode_Unchecked(object sender, RoutedEventArgs e)
        {
            Vs2013DarkTheme darktheme = new Vs2013DarkTheme();
            dockingmanager.Theme = darktheme;
            Fluent.ThemeManager.ChangeTheme(this, "Dark.Blue");
        }
        #endregion
        #region Sharing
        private void CmShare_Click(object sender, RoutedEventArgs e)
        {
            Export(true);
        }
        private void GetShare(string guid)
        {
            string tempfile = $"{GlobalVariables.defaultPath}\\sharedownload.zip";
            using (var writer = new FileStream(tempfile, System.IO.FileMode.Create))
            {
                var client = new RestClient($"http://{GlobalVariables.apiserver}{GlobalVariables.restbase}/{guid}");
                var request = new RestRequest(Method.GET);
                request.ResponseWriter = (responsestream) => responsestream.CopyTo(writer);
                var response = client.DownloadData(request);
            }
            ImportZip(tempfile,true);
            File.Delete(tempfile);
        }
        #endregion

    }
}
