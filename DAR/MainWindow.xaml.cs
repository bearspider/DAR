using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
    }
    public partial class MainWindow : Window
    {
        private TreeViewModel tv;
        private List<TreeViewModel> treeView;
        private int activeTriggers;
        private String currentSelection;
        public MainWindow()
        {
            InitializeComponent();
            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = System.IO.Directory.Exists(GlobalVariables.defaultPath);
            if(!mainPath)
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

            //Select Index 0 for CharacterProfiles

            //Start Monitoring Enabled Profiles

        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ribbonMain.Width = ActualWidth;
        }
        private void RibbonButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CharacterEditor newCharacter = new CharacterEditor();
            newCharacter.ShowDialog();
            UpdateListView();
        }
        private TreeViewModel BuildTree(TriggerGroup branch)
        {
            TreeViewModel rTree = new TreeViewModel(branch.TriggerGroupName)
            {
                Type = "triggergroup"
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
                            String selectedCharacter = listviewCharacters.SelectedItem.ToString();
                            var profiles = db.GetCollection<CharacterProfile>("profiles");
                            CharacterProfile profile = profiles.FindOne(Query.EQ("ProfileName", selectedCharacter));
                            if (profile.Triggers.Contains(item))
                            {
                                isChecked = true;
                            }
                        }
                        TreeViewModel newChildBranch = new TreeViewModel(getTrigger.Name)
                        {
                            Type = "trigger"
                        };
                        newChildBranch.IsChecked = isChecked; 
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
                                    String selectedCharacter = listviewCharacters.SelectedItem.ToString();
                                    var profiles = db.GetCollection<CharacterProfile>("profiles");
                                    CharacterProfile profile = profiles.FindOne(Query.EQ("ProfileName", selectedCharacter));
                                    if (profile.Triggers.Contains(item))
                                    {
                                        isChecked = true;
                                    }
                                }
                                TreeViewModel newChildBranch = new TreeViewModel(getTrigger.Name)
                                {
                                    Type = "trigger"
                                };
                                newChildBranch.IsChecked = isChecked;
                                rTree.Children.Add(newChildBranch);
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
            treeViewTriggers.ItemsSource = treeView;
        }
        private void UpdateListView()
        {
            listviewCharacters.Items.Clear();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                foreach (var doc in col.FindAll())
                {
                    listviewCharacters.Items.Add(doc.ProfileName);
                }
            }
            if (listviewCharacters.SelectedItem == null && listviewCharacters.Items.Count > 0)
            {
                listviewCharacters.SelectedIndex = 0;
                currentSelection = listviewCharacters.SelectedItem.ToString();
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
            UpdateListView();
        }
        private void RibbonButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                String selectedCharacter = listviewCharacters.SelectedItem.ToString();
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to Delete {selectedCharacter}","Confirmation",MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    var dbdelete = col.Delete(Query.EQ("ProfileName", selectedCharacter));
                    UpdateListView();
                }
            }
        }
        private void ListviewCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ribbonCharEdit.IsEnabled = true;
            ribbonCharRemove.IsEnabled = true;
            //Update TriggerView with selected triggers from profile
            String selectedCharacter = listviewCharacters.SelectedItem.ToString();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");
                var result = col.FindOne(Query.EQ("ProfileName", selectedCharacter));
                foreach(int checkedTrigger in result.Triggers)
                {
                    activeTriggers = checkedTrigger;
                }
            }
        }
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            String selectedGroup = ((TreeViewModel)treeViewTriggers.SelectedItem).Name;
            AddTrigger newTrigger = new AddTrigger(selectedGroup);
            newTrigger.ShowDialog();
            UpdateTriggerView();
        }
        private void TriggerGroupsAdd_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEditor triggerDialog = new TriggerGroupEditor();
            triggerDialog.ShowDialog();
            e.Handled = true;
            UpdateTriggerView();
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
                    UpdateTriggerView();
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
            UpdateTriggerView();
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
            UpdateTriggerView();
        }
        private void TriggerGroupsAddTopLevel_Click(object sender, RoutedEventArgs e)
        {
            TriggerGroupEditor triggerDialog = new TriggerGroupEditor();
            triggerDialog.ShowDialog();
            e.Handled = true;
            UpdateTriggerView();
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
                    getGroup.RemoveTrigger(getTrigger.Id);
                    triggergroup.Update(getGroup);
                    col.Delete(getTrigger.Id);
                }
            }
            UpdateTriggerView();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
        }
    }
}
