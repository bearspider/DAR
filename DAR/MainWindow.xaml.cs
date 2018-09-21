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
        public MainWindow()
        {
            InitializeComponent();
            //Check if EQAudioTriggers folder exists, if not create.
            bool mainPath = System.IO.Directory.Exists(GlobalVariables.defaultPath);
            if(!mainPath)
            {
                System.IO.Directory.CreateDirectory(GlobalVariables.defaultPath);
            }
            UpdateListView();
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
    }
}
