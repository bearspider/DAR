using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class OverlayTimer
    {
        public OverlayTimer()
        {
            Name = "default";
            Font = "Segoe UI";
            Size = 20;
            BG = "GhostWhite";
            Faded = "Transparent";
            Showtimer = true;
            Emptycolor = "Gray";
            Standardize = false;
            Group = false;
            Sortby = "Time Remaining";
        }
        public int Id { get; set; }
        public String Name { get; set; }
        public String Font { get; set; }
        public int Size { get; set; }
        public String BG { get; set; }
        public String Faded { get; set; }
        public Boolean Showtimer { get; set; }
        public String Emptycolor { get; set; }
        public Boolean Standardize { get; set; }
        public Boolean Group { get; set; }
        public String Sortby { get; set; }
    }
}
