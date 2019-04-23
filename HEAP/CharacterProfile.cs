using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.ComponentModel;

namespace HEAP
{
    public class CharacterProfile : INotifyPropertyChanged
    {
        private int _id;
        private string _characterName;
        private string _logFile;
        private string _profileName;
        private bool _monitor;
        private bool _monitorAtStartup;
        private bool _fileexists;
        private string _textFontColor;
        private string _timerFontColor;
        private string _timerBarColor;
        private int _volumeValue;
        private int _speechRate;
        private string _voice;
        private ArrayList _triggers;
        private SpeechSynthesizer _synth;

        public event PropertyChangedEventHandler PropertyChanged;

        //Constructor
        public CharacterProfile()
        {
            _id = 0;
            _characterName = "Beastmaster";
            _monitor = true;
            _monitorAtStartup = true;
            _fileexists = false;
            _textFontColor = "Black";
            _timerFontColor = "Blue";
            _timerBarColor = "Lime";
            _voice = "Microsoft David Desktop";
            _volumeValue = 90;
            _speechRate = 0;
            _synth = new SpeechSynthesizer();
            _synth.Rate = _speechRate;
            _synth.Volume = _volumeValue;
            _synth.SelectVoice(_voice);
            _triggers = new ArrayList();
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public ArrayList Triggers
        {
            get { return _triggers; }
            set { _triggers = value; }
        }
        public string Name
        {
            get { return _characterName; }
            set
            {
                if(_characterName != value)
                {
                    _characterName = value;
                    this.NotifyPropertyChanged("Name");
                }                
            }
        }
        public string LogFile
        {
            get { return _logFile; }
            set
            {
                _logFile = value;
                if (_logFile != value)
                {
                    _logFile = value;
                    this.NotifyPropertyChanged("LogFile");
                }
            }
        }
        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName != value)
                {
                    _profileName = value;
                    this.NotifyPropertyChanged("ProfileName");
                }
            }
        }
        public bool Monitor
        {
            get { return _monitor; }
            set
            {
                if (_monitor != value)
                {
                    _monitor = value;
                    this.NotifyPropertyChanged("Monitor");
                }
            }
        }
        public bool MonitorAtStartup
        {
            get { return _monitorAtStartup; }
            set
            {
                if (_monitorAtStartup != value)
                {
                    _monitorAtStartup = value;
                    this.NotifyPropertyChanged("MonitorAtStartup");
                }
            }
        }
        public int VolumeValue
        {
            get { return _volumeValue; }
            set
            {
                if (_volumeValue != value)
                {
                    _volumeValue = value;
                    this.NotifyPropertyChanged("VolumeValue");
                }
            }
        }
        public int SpeechRate
        {
            get { return _speechRate; }
            set
            {
                if (_speechRate != value)
                {
                    _speechRate = value;
                    this.NotifyPropertyChanged("SpeechRate");
                }
            }
        }
        public string Voice
        {
            get { return _voice; }
            set
            {
                if (_voice != value)
                {
                    _voice = value;
                    this.NotifyPropertyChanged("Voice");
                }
            }
        }
        public string TextFontColor
        {
            get { return _textFontColor; }
            set
            {
                if (_textFontColor != value)
                {
                    _textFontColor = value;
                    this.NotifyPropertyChanged("TextFontColor");
                }
            }
        }
        public string TimerFontColor
        {
            get { return _timerFontColor; }
            set
            {
                if (_timerFontColor != value)
                {
                    _timerFontColor = value;
                    this.NotifyPropertyChanged("TimerFontColor");
                }
            }
        }
        public string TimerBarColor
        {
            get { return _timerBarColor; }
            set
            {
                if (_timerBarColor != value)
                {
                    _timerBarColor = value;
                    this.NotifyPropertyChanged("TimerBarColor");
                }
            }
        }
        public Boolean FileExists
        {
            get { return _fileexists; }
            set
            {
                if (_fileexists != value)
                {
                    _fileexists = value;
                    this.NotifyPropertyChanged("FileExists");
                }
            }
        }
        public async void Speak(string output)
        {
            await Task.Run(() =>
            {
                _synth.Speak(output);
            });            
        }
        public void AddTrigger(string triggerId)
        {
            _triggers.Add(triggerId);
            this.NotifyPropertyChanged("AddTrigger");
        }
        public void UpdateVolume(int volume)
        {
            int newvolume = (VolumeValue / 100) * (volume / 100);
            _synth.Volume = newvolume;
            this.NotifyPropertyChanged("VolumeChanged");
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }


}
