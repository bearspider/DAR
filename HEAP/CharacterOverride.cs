using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class CharacterOverride
    {
        public CharacterOverride()
        {
            ProfileName = "";
            TextOverlaySelection = "Use category overlay";
            TextColorSelection = "Use category colors";
            TextColorFont = "Aquamarine";
            TextOverlay = "Default";
            TextColorSelection = "Aquamarine";
            TimerOverlaySelection = "Use category overlay";
            TimerColorSelection = "Use category colors";
            TimerOverlay = "Default";
            TimerColorFont = "Aquamarine";
            TimerColorBar = "Blue";
            TextOverlayCategory = true;
            TextOverlayThis = false;
            TextColorCategory = true;
            TextColorCharacter = false;
            TextColorThis = false;
            TimerOverlayCategory = true;
            TimerOverlayThis = false;
            TimerColorCategory = true;
            TimerColorCharacter = false;
            TimerColorThis = false;
        }
        public string ProfileName { get; set; }
        public string TextOverlaySelection { get; set; }
        public string TextColorSelection { get; set; }
        public string TimerOverlaySelection { get; set; }
        public string TimerColorSelection { get; set; }
        public string TextOverlay { get; set; }
        public string TextColorFont { get; set; }
        public string TimerOverlay { get; set; }
        public string TimerColorFont { get; set; }
        public string TimerColorBar { get; set; }
        public Boolean TextOverlayCategory { get; set; }
        public Boolean TextOverlayThis { get; set; }
        public Boolean TextColorCategory { get; set; }
        public Boolean TextColorCharacter { get; set; }
        public Boolean TextColorThis { get; set; }
        public Boolean TimerOverlayCategory { get; set; }
        public Boolean TimerOverlayThis { get; set; }
        public Boolean TimerColorCategory { get; set; }
        public Boolean TimerColorCharacter { get; set; }
        public Boolean TimerColorThis { get; set; }

    }
}
