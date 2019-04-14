using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class ActivatedTrigger
    {
        public String Name { get; set; }
        public String FromLog { get; set; }
        public String MatchText { get; set; }
        public String TriggerTime { get; set; }
        public int Id { get; set; }
        public DateTime TriggerFired { get; set; }

        public ActivatedTrigger()
        {
            Name = "";
            FromLog = "";
            MatchText = "";
            TriggerTime = "";
            Id = 0;
            TriggerFired = DateTime.Now;
        }
    }
}
