using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace DAR
{
    public class CharacterProfile
    {
        public string characterName;
        public string logFile;
        public string profileName;
        public bool monitor;
        public string textFontColor;
        public string timerFontColor;
        public string timerBarColor;
        public int volumeValue;
        public int speechRate;
        public string voice;
        private SpeechSynthesizer synth;
        //Constructor
        public CharacterProfile()
        {
            synth = new SpeechSynthesizer();
            synth.Rate = 0;
            synth.Volume = 90;
            synth.SelectVoice("Microsoft David Desktop");
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
        public void Speak(string output)
        {
            synth.Speak(output);
        }
    }


}
