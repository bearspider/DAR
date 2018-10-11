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
    /// Interaction logic for OverlayTextWindow.xaml
    /// </summary>
    public partial class OverlayTextWindow : Window
    {
        public OverlayText windowproperties {get; set;}
        public List<String> triggers { get; set; }
        public OverlayTextWindow()
        {
            InitializeComponent();
            listviewTriggers.ItemsSource = triggers;
        }
        public void SetProperties(OverlayText overlay)
        {
            windowproperties = overlay;
            this.Left = windowproperties.WindowX;
            this.Top = windowproperties.WindowY;
            this.Height = windowproperties.WindowHeight;
            this.Width = windowproperties.WindowWidth;
            SetBackground(windowproperties.Faded);
        }
        private void SetBackground(String bgcolor)
        {
            var windowcolor = ColorConverter.ConvertFromString(bgcolor);
            Brush brush = new SolidColorBrush((Color)windowcolor);
            this.Background = brush;
        }
        public void AddTrigger(String newtext)
        {
            triggers.Add(newtext);
        }
    }
}
