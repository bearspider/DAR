using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAR
{
    public partial class TriggerGroupEditor : Form
    {
        private string origGroupName;
        private TreeViewModel parentTree;
        private Boolean addChild;

        public TriggerGroupEditor()
        {
            InitializeComponent();
            addChild = false;
        }
        public TriggerGroupEditor(TriggerGroup editTrigger)
        {
            InitializeComponent();
            addChild = false;
            origGroupName = editTrigger.TriggerGroupName;
            textBoxName.Text = editTrigger.TriggerGroupName;
            textBoxComments.Text = editTrigger.Comments;
            checkBoxEnable.Checked = editTrigger.DefaultEnabled;
        }
        public TriggerGroupEditor(TreeViewModel parentObject)
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
                if (textBoxName.Modified)
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", origGroupName));
                }
                else
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", textBoxName.Text));
                }

                IEnumerator<TriggerGroup> enumerator = recordSearch.GetEnumerator();
                enumerator.MoveNext();
                var editTrigger = (enumerator.Current);
                if (recordSearch.Count<TriggerGroup>() > 0)
                {
                    //Update Record instead
                    editTrigger.TriggerGroupName = textBoxName.Text;
                    editTrigger.Comments = textBoxComments.Text;
                    editTrigger.DefaultEnabled = checkBoxEnable.Checked;
                    col.Update(editTrigger);
                }
                else
                {
                    //Insert new record
                    var triggerGroup = new TriggerGroup
                    {
                        TriggerGroupName = textBoxName.Text,
                        Comments = textBoxComments.Text,
                        DefaultEnabled = checkBoxEnable.Checked,
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
                if (textBoxName.Modified)
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", origGroupName));
                }
                else
                {
                    recordSearch = col.Find(Query.EQ("TriggerGroupName", textBoxName.Text));
                }

                IEnumerator<TriggerGroup> enumerator = recordSearch.GetEnumerator();
                enumerator.MoveNext();
                var editTrigger = (enumerator.Current);
                if (recordSearch.Count<TriggerGroup>() > 0)
                {
                    //Update Record instead
                    editTrigger.TriggerGroupName = textBoxName.Text;
                    editTrigger.Comments = textBoxComments.Text;
                    editTrigger.DefaultEnabled = checkBoxEnable.Checked;
                    col.Update(editTrigger);
                }
                else
                {
                    //Insert new record
                    //No Children since new Trigger Group
                    var triggerGroup = new TriggerGroup
                    {
                        TriggerGroupName = textBoxName.Text,
                        Comments = textBoxComments.Text,
                        DefaultEnabled = checkBoxEnable.Checked
                    };
                    col.Insert(triggerGroup);
                }
            }
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if(addChild)
            {
                AddTriggerGroup(parentTree);
            }
            else
            {
                AddTriggerGroup();
            }      
            this.Close();
        }
    }
}
