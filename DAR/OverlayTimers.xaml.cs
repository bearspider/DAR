using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DAR
{
    /// <summary>
    /// Interaction logic for OverlayTimers.xaml
    /// </summary>
    public partial class OverlayTimers : Window
    {
        public ObservableCollection<ProgressBar> timerBars = new ObservableCollection<ProgressBar>();
        private ProgressBar progress = new ProgressBar();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public OverlayTimers()
        {
            InitializeComponent();
            progress.Minimum = 1;
            progress.Maximum = 10;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            timerBars.Add(progress);
            listviewTimers.ItemsSource = timerBars;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            progress.Value++;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.Opacity = 1.0;
                this.DragMove();
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.Opacity = .4;
            }
        }
    }
}
