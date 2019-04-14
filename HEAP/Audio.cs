using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class Audio
    {
        private String audioType;
        private String tts;
        private Boolean interrupt;
        private String soundFileId;

        public Audio()
        {
            audioType = "";
            tts = "";
            interrupt = false;
            soundFileId = "";
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
        public String SoundFileId
        {
            get { return soundFileId; }
            set { soundFileId = value; }
        }
    }
}
