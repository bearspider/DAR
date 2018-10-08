using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class OverlayText
    {
        public OverlayText()
        {
            Name = "default";
            Font = "Segoe UI";
            Size = 20;
            Delay = 10;
            BG = "Blue";
            Faded = "Transparent";
        }
        public int Id { get; set; }
        public String Name { get; set; }
        public String Font { get; set; }
        public int Size { get; set; }
        public int Delay { get; set; }
        public String BG { get; set; }
        public String Faded { get; set; }
    }
}
