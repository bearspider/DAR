using LiteDB;
using System;
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
                characterProfiles.EnsureIndex((CharacterProfile x) => x.Id, true);
                triggerGroups.EnsureIndex((TriggerGroup y) => y.Id, true);
            }
            UpdateListView();
            UpdateTriggerView();

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
            TreeViewModel rTree = new TreeViewModel(branch.TriggerGroupName);
            if (branch.Children.Count > 0)
            {
                foreach (int leaf in branch.Children)
                {
                    TriggerGroup leafGroup = GetTriggerGroup(leaf);
                    rTree.Children.Add(BuildTree(leafGroup));
                }
            }
            if (branch.triggers.Count > 0)
            {
                foreach (Trigger item in branch.triggers)
                {
                    rTree.Children.Add(new TreeViewModel(item.Name));
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
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                foreach (var doc in col.FindAll())
                {
                    if (doc.Parent == 0)
                    {
                        if(doc.Children.Count > 0)
                        {
                            tv.Children.Add(BuildTree(doc));
                        }
                        else
                        {
                            tv.Children.Add(new TreeViewModel(doc.TriggerGroupName));
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
        }
        private void TriggerAdd_Click(object sender, RoutedEventArgs e)
        {
            //Build new Trigger
            AddTrigger newTrigger = new AddTrigger();
            newTrigger.ShowDialog();
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
                triggerGroupsEdit.IsEnabled = true;
                triggerGroupsRemove.IsEnabled = true;
                triggerGroupsAddSelected.IsEnabled = true;
                triggerAdd.IsEnabled = true;
            }
            else
            {
                triggerAdd.IsEnabled = false;
                triggerGroupsEdit.IsEnabled = false;
                triggerGroupsRemove.IsEnabled = false;
                triggerGroupsAddSelected.IsEnabled = false;
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
    }
}
