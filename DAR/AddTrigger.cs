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
        public AddTrigger()
        {
            InitializeComponent();
            (tabControl1.TabPages[2] as TabPage).Enabled = false;
            (tabControl1.TabPages[3] as TabPage).Enabled = false;
            comboBoxTimerType.SelectedIndex = 1;
        }
    }
}
