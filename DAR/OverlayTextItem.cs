using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class OverlayTextItem
    {
        public Trigger TriggerObject { get; set; }
        public int FontSize { get; set; }
        public String FontColor { get; set; }
        public String FontFamily { get; set; }
        public OverlayTextItem()
        {
            TriggerObject = new Trigger();
            FontSize = 20;
            FontColor = "black";
            FontFamily = "Seqoe UI";
        }
    }
}
