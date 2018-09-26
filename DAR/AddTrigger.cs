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
    public partial class AddTrigger : Form
    {
        public AddTrigger(String database)
        {
            InitializeComponent();
            (tabControl1.TabPages[2] as TabPage).Enabled = false;
            (tabControl1.TabPages[3] as TabPage).Enabled = false;
            comboBoxTimerType.SelectedIndex = 1;
            using (var db = new LiteDatabase(database))
            {
                LiteCollection<CharacterProfile> characterProfiles = db.GetCollection<CharacterProfile>("profiles");
                var col = db.GetCollection<CharacterProfile>("profiles");
                foreach (var doc in col.FindAll())
                {
                    listviewCharacters.Items.Add(doc.ProfileName);
                }
            }
        }
    }
}
