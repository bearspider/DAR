using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DAR
{
    public class TriggerTimer : INotifyPropertyChanged
    {
        public string Character { get; set; }
        public DispatcherTimer WindowTimer;
        public String TimerDescription { get; set; }
        public int TimerDuration { get; set; }
        public ProgressBar Progress { get; set; }
        public Boolean Direction { get; set; }//false is count down, true is count up
        public double ProgressValue { get; set; }
        public string Barcolor { get; set; }
        public string Textcolor { get; set; }
        private DateTime TriggeredTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public TriggerTimer ()
        {
            WindowTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)//hours,minutes,seconds
            };
            TriggeredTime = DateTime.Now;
            TimerDescription = "New Timer";
            TimerDuration = 0;
            Direction = false;
            Character = "";
            Barcolor = "";
            Textcolor = "";
            Progress = new ProgressBar()
            {
                Minimum = 0,
                Maximum = 1
            };
        }
        public double Minimum
        {
            get { return Progress.Minimum; }
        }
        public double Maximum
        {
            get { return Progress.Maximum; }
        }
        public double GetProgress()
        {
            return Progress.Value;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = TriggeredTime - DateTime.Now;
            
            if (Direction)
            {
                Console.WriteLine(Convert.ToInt32(Math.Abs(elapsed.TotalSeconds)));
                Progress.Value = Convert.ToInt32(Math.Abs(elapsed.TotalSeconds));
            }
            else
            {
                Progress.Value = Convert.ToInt32(elapsed.TotalSeconds) + TimerDuration;
               
            }

            NotifyPropertyChanged("Value");
        }
        public void SetTimer(String description, int duration, Boolean count)
        {
            TimerDescription = description;
            TimerDuration = duration;
            if(count)
            {
                Progress.Value = 0;
            }
            else
            {
                Progress.Value = duration;
            }
            Direction = count;            
            WindowTimer.Tick += DispatcherTimer_Tick;
        }
        public void StartTimer()
        {
            WindowTimer.Start();            
        }
        public void StopTimer()
        {
            WindowTimer.Stop();
        }
        public void SetProgress(int minimum, int maximum)
        {
            Progress.Minimum = minimum;
            Progress.Maximum = maximum;
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
