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
