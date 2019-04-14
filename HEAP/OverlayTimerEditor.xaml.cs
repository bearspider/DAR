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

namespace HEAP
{
    /// <summary>
    /// Interaction logic for OverlayTimerEditor.xaml
    /// </summary>
    public partial class OverlayTimerEditor : Window
    {

        public OverlayTimerEditor()
        {
            InitializeComponent();
            comboFont.Text = "Segoe UI";
            comboSort.Text = "Order Triggered";
        }
        public OverlayTimerEditor(String toedit)
        {
            InitializeComponent();
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                OverlayTimer window = overlaytimers.FindOne(Query.EQ("Name", toedit));
                this.Width = window.WindowWidth;
                this.Height = window.WindowHeight;
                this.Left = window.WindowX;
                this.Top = window.WindowY;
                if (textDemo != null) { textDemo.Text = window.Name; }
                if (comboFont != null) { comboFont.Text = window.Font; }
                if (sliderSize != null) { sliderSize.Value = window.Size; }
                if (checkTimer != null) { checkTimer.IsChecked = window.Showtimer; }
                if (checkStandardize != null) { checkStandardize.IsChecked = window.Standardize; }
                if (checkGroup != null) { checkGroup.IsChecked = window.Group; }
                if (comboSort != null) { comboSort.Text = window.Sortby; }
                if (ClrPckerBg != null) { ClrPckerBg.SelectedColor = (Color)ColorConverter.ConvertFromString(window.BG); }
                if (ClrPckerEmpty != null) { ClrPckerEmpty.SelectedColor = (Color)ColorConverter.ConvertFromString(window.Emptycolor); }
                if (ClrPckerFaded != null) { ClrPckerFaded.SelectedColor = (Color)ColorConverter.ConvertFromString(window.Emptycolor); }
                SetBackground(window.Faded);
            }
        }
        private void ClrPckerBg_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }
        private void SetBackground(String bgcolor)
        {
            var windowcolor = ColorConverter.ConvertFromString(bgcolor);
            Brush brush = new SolidColorBrush((Color)windowcolor);
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
                Sortby = comboSort.SelectionBoxItem.ToString(),
                WindowHeight = this.Height,
                WindowWidth = this.Width,
                WindowX = this.Left,
                WindowY = this.Top
            };
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                LiteCollection<OverlayTimer> overlaytimers = db.GetCollection<OverlayTimer>("overlaytimers");
                var getTimer = overlaytimers.FindOne(Query.EQ("Name", timer.Name));
                if(getTimer != null)
                {
                    //update Timer
                    int timerid = getTimer.Id;
                    getTimer = timer;
                    getTimer.Id = timerid;
                    overlaytimers.Update(getTimer);
                }
                else
                {
                    //Insert new timer
                    overlaytimers.Insert(timer);
                }
            }
            var main = App.Current.MainWindow as MainWindow;
            main.OverlayTimer_Refresh();
            //Deploy overlay
            this.Close();
        }
        private void ClrPckerFaded_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SetBackground(ClrPckerFaded.SelectedColorText);
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

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
