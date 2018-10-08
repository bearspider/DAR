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
    /// Interaction logic for OverlayTimerEditor.xaml
    /// </summary>
    public partial class OverlayTimerEditor : Window
    {

        public OverlayTimerEditor()
        {
            InitializeComponent();
            
        }

        private void ClrPckerBg_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            OverlayTimer timer = new OverlayTimer
            {
                Name = textDemo.Text,
                Font = comboFont.SelectedItem.ToString(),
                Size = Convert.ToInt32(sliderSize.Value),
                BG = ClrPckerBg.SelectedColorText,
                Faded = ClrPckerFaded.SelectedColorText,
                Showtimer = (Boolean)checkTimer.IsChecked,
                Emptycolor = ClrPckerFaded.SelectedColorText,
                Standardize = (Boolean)checkStandardize.IsChecked,
                Group = (Boolean)checkGroup.IsChecked,
                Sortby = comboSort.SelectionBoxItem.ToString()
        };
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                var getTimerGroup = overlaytimers.FindOne(Query.EQ("Name", timer.Name));
                if(getTimerGroup != null)
                {
                    //update Timer
                }
                else
                {
                    //Insert new timer
                    overlaytimers.Insert(timer);
                }
            }
            this.Close();
        }
        private void ClrPckerFaded_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Brush brush = new SolidColorBrush((Color)ClrPckerFaded.SelectedColor);
            if (brush.ToString() == "#00FFFFFF")
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
