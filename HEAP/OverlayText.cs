﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class OverlayText
    {
        public OverlayText()
        {
            Name = "default";
            Font = "Segoe UI";
            FontColor = "Black";
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
        public String FontColor { get; set; }
        public int Size { get; set; }
        public int Delay { get; set; }
        public String BG { get; set; }
        public String Faded { get; set; }
        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }
        public double WindowX { get; set; }
        public double WindowY { get; set; }
        public bool Equals(OverlayText compareto)
        {
            bool rval = false;
            if(
                Id == compareto.Id
                && Name == compareto.Name
                && Font == compareto.Font
                && FontColor == compareto.FontColor
                && Size == compareto.Size
                && Delay == compareto.Delay
                && BG == compareto.BG
                && Faded == compareto.Faded
                && WindowHeight == compareto.WindowHeight
                && WindowWidth == compareto.WindowWidth
                && WindowX == compareto.WindowX
                && WindowY == compareto.WindowY
                )
            {
                rval = true;
            }
            return rval;
        }
    }
}
