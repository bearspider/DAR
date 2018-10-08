using LiteDB;
using System;
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

namespace DAR
{
    /// <summary>
    /// Interaction logic for OverlayTextEditor.xaml
    /// </summary>
    public partial class OverlayTextEditor : Window
    {
        public OverlayTextEditor()
        {
            InitializeComponent();
        }

        private void ClrPckerFaded_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Brush brush = new SolidColorBrush((Color)ClrPckerFaded.SelectedColor);
            if(brush.ToString() == "#00FFFFFF")
            {
                brush = Brushes.LightGray;
                this.Opacity = 0.7;
            }
            else
            {
                this.Opacity = 1.0;
            }
            this.Background = brush;
        }

        private void ClrPckerBg_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            OverlayText overlaytext = new OverlayText
            {
                Name = textDemo.Text,
                Font = comboFont.SelectedItem.ToString(),
                Size = Convert.ToInt32(sliderSize.Value),
                Delay = Convert.ToInt32(sliderDelay.Value),
                BG = ClrPckerBg.SelectedColorText,
                Faded = ClrPckerFaded.SelectedColorText
            };
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayText> overlaytexts = db.GetCollection<OverlayText>("overlaytexts");
                var getTextGroup = overlaytexts.FindOne(Query.EQ("Name", overlaytext.Name));
                if (getTextGroup != null)
                {
                    //update Timer
                }
                else
                {
                    //Insert new timer
                    overlaytexts.Insert(overlaytext);
                }
            }
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {

            }
        }
    }
}
