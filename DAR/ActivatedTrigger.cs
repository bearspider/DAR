using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class ActivatedTrigger
    {
        public String Name { get; set; }
        public String FromLog { get; set; }
        public String MatchText { get; set; }
        public String TriggerTime { get; set; }
        public DateTime TriggerFired { get; set; }

        public ActivatedTrigger()
        {
            Name = "";
            FromLog = "";
            MatchText = "";
            TriggerTime = "";
            TriggerFired = DateTime.Now;
        }
    }
}
