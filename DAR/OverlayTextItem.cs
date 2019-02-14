using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DAR
{
    public class OverlayTextItem:INotifyPropertyChanged
    {
        public Trigger TriggerObject { get; set; }
        public int FontSize { get; set; }
        public String FontColor { get; set; }
        public String FontFamily { get; set; }
        public int Duration { get; set; }
        public ProgressBar progress { get; set; }
        public double progressValue { get; set; }
        public DispatcherTimer dispatcherTimer { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public OverlayTextItem()
        {
            TriggerObject = new Trigger();
            dispatcherTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)//hours,minutes,seconds
            };
            FontSize = 20;
            FontColor = "black";
            FontFamily = "Seqoe UI";
            Duration = 10;
            progress = new ProgressBar()
            {
                Minimum = 0,
                Maximum = 1
            };
        }
        public double Minimum
        {
            get { return progress.Minimum; }
        }
        public double Maximum
        {
            get { return progress.Maximum; }
        }
        public double GetProgress()
        {
            return progress.Value;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            progress.Value--;
            NotifyPropertyChanged("Value");
        }
        public void SetTimer(int duration)
        {
            Duration = duration;
            progress.Value = duration;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }
        public void StartTimer()
        {
            dispatcherTimer.Start();
        }
        public void StopTimer()
        {
            dispatcherTimer.Stop();
        }
        public void SetProgress(int minimum, int maximum)
        {
            progress.Minimum = minimum;
            progress.Maximum = maximum;
        }
        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
