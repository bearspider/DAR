using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Globalization;
using System.ComponentModel;
using System.Collections.Specialized;

namespace HEAP
{
    /// <summary>
    /// Interaction logic for OverlayTextWindow.xaml
    /// </summary>
    public partial class OverlayTextWindow : Window
    {
        public OverlayText windowproperties { get; set; }
        private ObservableCollection<OverlayTextItem> triggers = new ObservableCollection<OverlayTextItem>();
        public int FSize { get; set; }
        public FontFamily FontName { get; set; }
        public OverlayTextWindow()
        {
            InitializeComponent();
            var listener = OcPropertyChangedListener.Create(triggers);
            icTriggers.ItemsSource = triggers;
        }
        public void SetProperties(OverlayText overlay)
        {
            FSize = overlay.Size;
            FontName = new FontFamily(overlay.Font);
            windowproperties = overlay;
            this.Name = overlay.Name;
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
        public void AddTrigger(Trigger newtrigger)
        {
            using (var db = new LiteDatabase(GlobalVariables.defaultDB))
            {
                var categoryCollection = db.GetCollection<Category>("categories");
                Category selectedcategory = categoryCollection.FindById(newtrigger.TriggerCategory);
                OverlayTextItem oti = new OverlayTextItem();
                oti.TriggerObject = newtrigger;
                oti.FontColor = selectedcategory.TextFontColor;
                oti.FontSize = windowproperties.Size;
                oti.FontFamily = windowproperties.Font;
                oti.Duration = windowproperties.Delay;
                oti.SetProgress(0,windowproperties.Delay);
                oti.SetTimer(windowproperties.Delay);
                oti.PropertyChanged += Listener_PropertyChanged;
                oti.StartTimer();
                triggers.Add(oti);
                icTriggers.DataContext = triggers;                
            }
        }
        public void RemoveTrigger(Trigger oldtrigger)
        {
            List<OverlayTextItem> toremove = new List<OverlayTextItem>();
            foreach (OverlayTextItem textitem in triggers)
            {
                if(textitem.TriggerObject == oldtrigger)
                {
                    toremove.Add(textitem);
                }
            }
            foreach(OverlayTextItem removeitem in toremove)
            {
                triggers.Remove(removeitem);
            }

        }
        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OverlayTextItem s = (OverlayTextItem)sender;

            if (s.progress.Value == 0)
            {
                s.StopTimer();
                triggers.Remove(s);
            }
        }
    }
}