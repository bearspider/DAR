using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace DAR
{
    public partial class CharacterEditor : Form
    {
        public CharacterEditor()
        {
            InitializeComponent();
            //CharacterProfile houkaa = new CharacterProfile();
            //houkaa.Speak("Greetings");
            labelVolumeValue.Text = trackBarVolume.Value.ToString();
            labelRateValue.Text = trackBarRate.Value.ToString();
            SpeechSynthesizer voicesynth = new SpeechSynthesizer();
            foreach( System.Speech.Synthesis.InstalledVoice installedVoice in voicesynth.GetInstalledVoices())
            {
                comboBoxVoice.Items.Add(installedVoice.VoiceInfo.Name);
            }
            if(comboBoxVoice.Items.Count > 0)
            {
                comboBoxVoice.SelectedIndex = 0;
            }
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            labelVolumeValue.Text = trackBarVolume.Value.ToString();
        }

        private void trackBarRate_Scroll(object sender, EventArgs e)
        {
            labelRateValue.Text = trackBarRate.Value.ToString();
        }

        private void buttonNamePlay_Click(object sender, EventArgs e)
        {
            
        }
    }
}
