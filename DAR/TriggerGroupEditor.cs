using LiteDB;
using System;
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
        private String function;
        private TriggerGroup parent;
        private BsonValue newChild;

        public TriggerGroupEditor()
        {
            InitializeComponent();
        }

        public TriggerGroupEditor(TriggerGroup editTrigger)
        {
            InitializeComponent();
            origGroupName = editTrigger.TriggerGroupName;
            textBoxName.Text = editTrigger.TriggerGroupName;
            textBoxComments.Text = editTrigger.Comments;
            checkBoxEnable.Checked = editTrigger.DefaultEnabled;
        }

        public TriggerGroupEditor(TriggerGroup parentGroup, String function)
        {
            InitializeComponent();
            parent = parentGroup;
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

                //If we changed the name of the trigger, Delete the trigger Group and then recreate it.
                //if (textBoxName.Text != editTrigger.TriggerGroupName)
                //{
                //    var dbdelete = col.Delete(Query.EQ("TriggerGroupName", editTrigger.TriggerGroupName));
                //}
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
                    newChild = col.Insert(triggerGroup);
                }
            }
        }
        private void AddChild()
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var col = db.GetCollection<TriggerGroup>("triggergroups");
                var parentGroup = col.FindById(parent.Id);
                parentGroup.AddChild(newChild);
                col.Update(parentGroup);
            }
        }
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            AddTriggerGroup();
            if (function == "AddChild")
            {
                AddChild();
            }
            
            this.Close();
        }
    }
}
