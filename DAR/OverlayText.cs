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
            WindowHeight = 450;
            WindowWidth = 800;
            WindowX = 0;
            WindowY = 0;
        }
        public int Id { get; set; }
        public String Name { get; set; }
        public String Font { get; set; }
        public int Size { get; set; }
        public int Delay { get; set; }
        public String BG { get; set; }
        public String Faded { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public double WindowX { get; set; }
        public double WindowY { get; set; }
    }
}
