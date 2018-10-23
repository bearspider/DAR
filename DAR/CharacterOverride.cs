using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
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
            TextOverlayId = 0;
            TextColorId = 0;
            TimerOverlayId = 0;
            TimerColorId = 0;
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
        public int TextOverlayId{ get; set; }
        public int TextColorId { get; set; }
        public int TimerOverlayId { get; set; }
        public int TimerColorId { get; set; }
    }
}
