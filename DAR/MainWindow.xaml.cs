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
    public class MonitoringStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String rString = "";
            if((Boolean)value)
            {
                rString = "Knob-Remove-icon.png";
            }
            else
            {
                rString = "Knob-Remove-Red-icon.png";
            }
            return rString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class MainWindow : Window
    {
        private TreeViewModel tv;
        private List<TreeViewModel> treeView;
        private ObservableCollection<CharacterProfile> characterProfiles = new ObservableCollection<CharacterProfile>();
        private String currentSelection;
        private Dictionary<String, FileSystemWatcher> watchers = new Dictionary<String, FileSystemWatcher>();
        private Dictionary<Trigger,ArrayList> activeTriggers = new Dictionary<Trigger,ArrayList>();
        public MainWindow()
        {
            InitializeComponent();
            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = System.IO.Directory.Exists(GlobalVariables.defaultPath);
            if (!mainPath)
            {
                System.IO.Directory.CreateDirectory(GlobalVariables.defaultPath);
            }
            //Prep and/or load database
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<CharacterProfile> characterProfiles = db.GetCollection<CharacterProfile>("profiles");
                LiteCollection<TriggerGroup> triggerGroups = db.GetCollection<TriggerGroup>("triggergroups");
                LiteCollection<Category> categories = db.GetCollection<Category>("categories");
                LiteCollection<Trigger> triggers = db.GetCollection<Trigger>("triggers");
                characterProfiles.EnsureIndex((CharacterProfile x) => x.Id, true);
                triggerGroups.EnsureIndex((TriggerGroup y) => y.Id, true);
                categories.EnsureIndex((Category z) => z.Id, true);
                triggers.EnsureIndex((Trigger w) => w.Id, true);
            }
            UpdateListView();
            UpdateTriggerView();
            //Start Monitoring Enabled Profiles
            foreach(CharacterProfile character in characterProfiles)
            {
                MonitorCharacter(character);
            }
            /* Match Groups             
            String inputTest = "[Sun Nov 23 15:27:39 2014] You must purchase a Trader's Satchel before you can sell items.";
            MatchCollection matches = Regex.Matches(inputTest, GlobalVariables.eqRegex, RegexOptions.IgnoreCase);
            foreach(Match match in matches)
            {
                MessageBox.Show(match.Groups["stringToMatch"].Value);
            }
            */

        }
        private void MonitorCharacter(CharacterProfile character)
        {
            if (character.Monitor)
            {
                Thread t = new Thread(() =>
                {
                    using (FileStream filestream = File.Open(character.LogFile, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        filestream.Seek(0, SeekOrigin.End);
                        using (StreamReader streamReader = new StreamReader(filestream))
                        {
                            for (; ; )
                            {
                                Thread.Sleep(TimeSpan.FromMilliseconds(500));
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
                                                foreach (int profile in doc.Profiles)
                                                {
                                                    var characters = db.GetCollection<CharacterProfile>("profiles");
                                                    var currentProfile = characters.FindById(profile);
                                                    currentProfile.Speak(doc.SearchText);
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
                currentProfile.Triggers.Remove(currentTrigger.id);
                currentTrigger.Profiles.Remove(selectedCharacter.Id);
                colProfiles.Update(currentProfile);
                colTriggers.Update(currentTrigger);
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
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ribbonMain.Width = ActualWidth;
        }
        private void RibbonButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CharacterEditor newCharacter = new CharacterEditor();
            newCharacter.ShowDialog();
            UpdateView();
        }
        private TreeViewModel BuildTree(TriggerGroup branch)
        {
            TreeViewModel rTree = new TreeViewModel(branch.TriggerGroupName)
            {
                Type = "triggergroup",
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
        private void UpdateView()
        {
            CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
            currentSelection = selectedCharacter.ProfileName;
            UpdateListView();
            UpdateTriggerView();
        }
        private void UpdateTriggerView()
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
                            Type = "triggergroup"
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
        private void UpdateListView()
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
        private void RibbonButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            String selectedCharacter = listviewCharacters.SelectedItem.ToString();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                var result = col.Find(Query.EQ("ProfileName",selectedCharacter));
                IEnumerator<CharacterProfile> enumerator = result.GetEnumerator();
                enumerator.MoveNext();
                var character = (enumerator.Current);
                CharacterEditor editCharacter = new CharacterEditor(character);
                editCharacter.ShowDialog();
            }
            UpdateView();
        }
        private void RibbonButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                String selectedCharacter = ((CharacterProfile)listviewCharacters.SelectedItem).ProfileName;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {selectedCharacter}","Confirmation",MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    var dbdelete = col.Delete(Query.EQ("ProfileName", selectedCharacter));
                    UpdateView();
                }
            }
        }
        private void ListviewCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ribbonCharEdit.IsEnabled = true;
            ribbonCharRemove.IsEnabled = true;
            //Update TriggerView with selected triggers from profile
            if(listviewCharacters.Items.Count > 0)
            {
                CharacterProfile selectedCharacter = (CharacterProfile)listviewCharacters.SelectedItem;
                currentSelection = selectedCharacter.ProfileName;
                UpdateTriggerView();
            }
        }
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            String selectedGroup = ((TreeViewModel)treeViewTriggers.SelectedItem).Name;
            AddTrigger newTrigger = new AddTrigger(selectedGroup);
            newTrigger.ShowDialog();
            UpdateView();
        }
        private void TriggerGroupsAdd_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEditor triggerDialog = new TriggerGroupEditor();
            triggerDialog.ShowDialog();
            e.Handled = true;
            UpdateView();
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
                    foreach(var child in childContains)
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
            TriggerGroupEditor triggerDialog = new TriggerGroupEditor(root);
            triggerDialog.ShowDialog();
            e.Handled = true;
            UpdateView();
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
        private void TriggerGroupsEdit_Click(object sender, RoutedEventArgs e)
        {
            TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                var result = col.Find(Query.EQ("TriggerGroupName", root.Name));
                IEnumerator<TriggerGroup> enumerator = result.GetEnumerator();
                enumerator.MoveNext();
                var selectedGroup = (enumerator.Current);
                TriggerGroupEditor triggerDialog = new TriggerGroupEditor(selectedGroup);
                triggerDialog.ShowDialog();
            }
            UpdateView();
        }
        private void TriggerGroupsAddTopLevel_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEditor triggerDialog = new TriggerGroupEditor();
            triggerDialog.ShowDialog();
            e.Handled = true;
            UpdateView();
        }
        private void TriggerRemove_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                TreeViewModel root = (TreeViewModel)treeViewTriggers.SelectedItem;
                var col = db.GetCollection<Trigger>("triggers");
                var triggergroup = db.GetCollection<TriggerGroup>("triggergroups");
                var getTrigger = col.FindOne(Query.EQ("Name",root.Name));
                var getGroup = triggergroup.FindById(getTrigger.Parent);
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {root.Name}", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    RemoveTrigger(root.Name);
                    getGroup.RemoveTrigger(getTrigger.Id);
                    triggergroup.Update(getGroup);
                    col.Delete(getTrigger.Id);
                }
                
            }            
            UpdateView();
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
                if(currentProfile.Monitor)
                {
                    MonitorCharacter(currentProfile);
                }
            }

            UpdateListView();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var colProfiles = db.GetCollection<CharacterProfile>("profiles");
                foreach (CharacterProfile doc in colProfiles.FindAll())
                {
                    if(doc.MonitorAtStartup != doc.Monitor)
                    {
                        doc.Monitor = doc.MonitorAtStartup;
                        colProfiles.Update(doc);
                    }
                }
            }
            Environment.Exit(Environment.ExitCode);
        }

        private void TextOverlayAddRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                OverlayTimers newOverlay = new OverlayTimers();
                newOverlay.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
           
        }
    }
}
