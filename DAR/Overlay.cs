using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAR
{
    public partial class Overlay : Form
    {
        private ProgressBar progress = new ProgressBar();
        private Timer timer1 = new Timer();
        private List<ProgressBar> timers = new List<ProgressBar>();
        private bool mouseDown;
        private Point lastLocation;
        public Overlay()
        {
            InitializeComponent();
            using (InstalledFontCollection col = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = col.Families;
                List<string> fonts = new List<string>();
                foreach (FontFamily font in fontFamilies)
                {
                    comboBoxFont.Items.Add(font.Name);
                }
            }
            for(int j = 8; j < 70; j++)
            {
                comboBoxFontSize.Items.Add(j);
            }
            comboBoxFont.SelectedItem = "Segoe UI";
            comboBoxFontSize.SelectedIndex = 20;
            
            timer1.Enabled = true;
            timer1.Start();
            timer1.Interval = 1000;

            progress.Maximum = 10;
            timer1.Tick += Timer1_Tick;
            this.Controls.Add(progress);
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Overlay_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            panel1.Visible = true;
            panel1.Enabled = true;
        }

        private void Overlay_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Enabled = false;
            panel1.Visible = false;
            mouseDown = true;
            lastLocation = e.Location;
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(progress.Value != 10)
            {
                progress.Value++;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void ComboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            FontFamily family = new FontFamily(comboBoxFont.SelectedItem.ToString());
            if (comboBoxFontSize.SelectedIndex > 0)
            {
                Font selectedFont = new Font(family, (comboBoxFontSize.SelectedIndex + 11));
                labelOverlayName.Font = selectedFont;
            }
            else
            {
                Font selectedFont = new Font(family,20);
                labelOverlayName.Font = selectedFont;
            }
            
        }

        private void ComboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxFontSize.SelectedIndex > 0)
            {
                FontFamily family = new FontFamily(comboBoxFont.SelectedItem.ToString());
                Font selectedFont = new Font(family, (comboBoxFontSize.SelectedIndex + 11));
                labelOverlayName.Font = selectedFont;
            }

        }
    }
}
