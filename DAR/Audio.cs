using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class Audio
    {
        private String audioType;
        private String tts;
        private Boolean interrupt;
        private int soundFileId;

        public Audio()
        {
            audioType = "";
            tts = "";
            interrupt = false;
            soundFileId = 0;
        }

        public String AudioType
        {
            get { return audioType; }
            set { audioType = value; }
        }
        public String TTS
        {
            get { return tts; }
            set { tts = value; }
        }
        public Boolean Interrupt
        {
            get { return interrupt; }
            set { interrupt = value; }
        }
        public int SoundFileId
        {
            get { return soundFileId; }
            set { soundFileId = value; }
        }
    }
}
