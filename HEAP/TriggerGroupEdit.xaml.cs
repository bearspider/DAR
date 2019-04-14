using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace HEAP
{
    /// <summary>
    /// Interaction logic for TriggerGroupEdit.xaml
    /// </summary>
    public partial class TriggerGroupEdit : Window
    {
        private string origGroupName;
        private TreeViewModel parentTree;
        private Boolean addChild;
        private int editGroupId;

        public TriggerGroupEdit()
        {
            InitializeComponent();
        }
        public TriggerGroupEdit(TriggerGroup editTrigger)
        {
            InitializeComponent();
            addChild = false;
            origGroupName = editTrigger.TriggerGroupName;
            textboxName.Text = editTrigger.TriggerGroupName;
            textboxComments.Text = editTrigger.Comments;
            checkboxEnable.IsChecked = editTrigger.DefaultEnabled;
            editGroupId = editTrigger.Id;
        }
        public TriggerGroupEdit(TreeViewModel parentObject)
        {
            InitializeComponent();
            addChild = true;
            parentTree = parentObject;
        }
        private void AddTriggerGroup(TreeViewModel parentObject)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                BsonValue newChild = new BsonValue();
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                var record = col.FindOne(Query.EQ("TriggerGroupName", parentObject.Name));
                IEnumerable<TriggerGroup> recordSearch;
                if (textboxName.Text != origGroupName)
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", origGroupName));
                }
                else
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", textboxName.Text));
                }

                IEnumerator<TriggerGroup> enumerator = recordSearch.GetEnumerator();
                if (recordSearch.Count<TriggerGroup>() > 0)
                {
                    for (int i = 0; i < recordSearch.Count<TriggerGroup>(); i++)
                    {
                        enumerator.MoveNext();
                        var editTrigger = (enumerator.Current);
                        //Update Record instead
                        if (editTrigger.Parent == record.Id)
                        {
                            editTrigger.TriggerGroupName = textboxName.Text;
                            editTrigger.Comments = textboxComments.Text;
                            editTrigger.DefaultEnabled = (Boolean)checkboxEnable.IsChecked;
                            col.Update(editTrigger);
                        }
                    }
                }
                else
                {
                    //Insert new record
                    var triggerGroup = new TriggerGroup
                    {
                        TriggerGroupName = textboxName.Text,
                        Comments = textboxComments.Text,
                        DefaultEnabled = (Boolean)checkboxEnable.IsChecked,
                        Parent = record.Id,
                        Children = new ArrayList()
                    };
                    newChild = col.Insert(triggerGroup);
                }
                //Add new trigger group to it's parent list
                var newrecord = col.FindById(newChild.AsDecimal);
                record.Children.Add(newrecord.Id);
                col.Update(record);
            }
        }
        private void AddTriggerGroup()
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                IEnumerable<TriggerGroup> recordSearch;
                if (textboxName.Text != origGroupName)
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", origGroupName));
                }
                else
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", textboxName.Text));
                }

                IEnumerator<TriggerGroup> enumerator = recordSearch.GetEnumerator();
                if (recordSearch.Count<TriggerGroup>() > 0)
                {
                    for (int i = 0; i < recordSearch.Count<TriggerGroup>(); i++)
                    {
                        enumerator.MoveNext();
                        var editTrigger = (enumerator.Current);
                        //Update Record instead
                        if (editTrigger.Id == editGroupId)
                        {
                            editTrigger.TriggerGroupName = textboxName.Text;
                            editTrigger.Comments = textboxComments.Text;
                            editTrigger.DefaultEnabled = (Boolean)checkboxEnable.IsChecked;
                            col.Update(editTrigger);
                        }
                    }
                }
                else
                {
                    //Insert new record
                    //No Children since new Trigger Group
                    var triggerGroup = new TriggerGroup
                    {
                        TriggerGroupName = textboxName.Text,
                        Comments = textboxComments.Text,
                        DefaultEnabled = (Boolean)checkboxEnable.IsChecked
                    };
                    col.Insert(triggerGroup);
                }
            }
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (addChild)
            {
                //I don't know if this ever gets called.
                AddTriggerGroup(parentTree);
            }
            else
            {
                AddTriggerGroup();
            }
            var main = App.Current.MainWindow as MainWindow;
            main.UpdateTriggerView();
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
