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
        public string character;
        public DispatcherTimer dispatcherTimer;
        public String timerDescription;
        public int timerDuration;
        public ProgressBar progress;
        public Boolean direction; //false is count down, true is count up
        public double progressValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public TriggerTimer ()
        {
            dispatcherTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)//hours,minutes,seconds
            };
            timerDescription = "New Timer";
            timerDuration = 0;
            direction = false;
            character = "";
            progress = new ProgressBar()
            {
                Minimum = 0,
                Maximum = 1
            };
        }
        public String Character
        {
            get { return character; }
            set { character = value; }
        }
        public Boolean Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public double Minimum
        {
            get { return progress.Minimum; }
        }
        public double Maximum
        {
            get { return progress.Maximum; }
        }
        public double ProgressValue
        {
            get { return progressValue; }
            set { progressValue = value; }
        }
        public ProgressBar Progress
        {
            get { return progress; }
            set { progress = value; }
        }
        public DispatcherTimer DispatcherTimer
        {
            get { return dispatcherTimer; }
            set { dispatcherTimer = value; }
        }
        public String TimerDescription
        {
            get { return timerDescription; }
            set { timerDescription = value; }
        }
        public int TimerDuration
        {
            get { return timerDuration; }
            set { timerDuration = value; }
        }
        public double GetProgress()
        {
            return progress.Value;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(direction)
            {
                progress.Value++;                
            }
            else
            {
                progress.Value--;
            }

            NotifyPropertyChanged("Value");
        }
        public void SetTimer(String description, int duration, Boolean count)
        {
            timerDescription = description;
            timerDuration = duration;
            if(count)
            {
                Progress.Value = 0;
            }
            else
            {
                Progress.Value = duration;
            }
            direction = count;
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
