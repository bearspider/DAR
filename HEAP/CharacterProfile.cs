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
        public int id;
        public string characterName;
        public string logFile;
        public string profileName;
        public bool monitor;
        public bool monitorAtStartup;
        public bool fileexists;
        public string textFontColor;
        public string timerFontColor;
        public string timerBarColor;
        public int volumeValue;
        public int speechRate;
        public string voice;
        public ArrayList triggers;
        private SpeechSynthesizer synth;

        public event PropertyChangedEventHandler PropertyChanged;

        //Constructor
        public CharacterProfile()
        {
            id = 0;
            characterName = "Beastmaster";
            monitor = true;
            monitorAtStartup = true;
            fileexists = false;
            textFontColor = "Black";
            timerFontColor = "Blue";
            timerBarColor = "Lime";
            voice = "Microsoft David Desktop";
            volumeValue = 90;
            speechRate = 0;
            synth = new SpeechSynthesizer();
            synth.Rate = speechRate;
            synth.Volume = volumeValue;
            synth.SelectVoice(voice);
            triggers = new ArrayList();
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public ArrayList Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }
        public string Name
        {
            get { return characterName; }
            set { characterName = value; }
        }
        public string LogFile
        {
            get { return logFile; }
            set { logFile = value; }
        }
        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }
        public bool Monitor
        {
            get { return monitor; }
            set { monitor = value; }
        }
        public bool MonitorAtStartup
        {
            get { return monitorAtStartup; }
            set { monitorAtStartup = value; }
        }
        public int VolumeValue
        {
            get { return volumeValue; }
            set { volumeValue = value; }
        }
        public int SpeechRate
        {
            get { return speechRate; }
            set { speechRate = value; }
        }
        public string Voice
        {
            get { return voice; }
            set { voice = value; }
        }
        public string TextFontColor
        {
            get { return textFontColor; }
            set { textFontColor = value; }
        }
        public string TimerFontColor
        {
            get { return timerFontColor; }
            set { timerFontColor = value; }
        }
        public string TimerBarColor
        {
            get { return timerBarColor; }
            set { timerBarColor = value; }
        }
        public async void Speak(string output)
        {
            await Task.Run(() =>
            {
                synth.Speak(output);
            });            
        }
        public void AddTrigger(int triggerId)
        {
            triggers.Add(triggerId);
        }
        public Boolean FileExists
        {
            get { return fileexists; }
            set {fileexists = value;}
        }
        public void UpdateVolume(int volume)
        {
            int newvolume = (VolumeValue / 100) * (volume / 100);
            synth.Volume = newvolume;
        }
    }


}
