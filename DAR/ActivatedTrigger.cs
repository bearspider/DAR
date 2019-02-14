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
        public DateTime TriggerTime { get; set; }

        public ActivatedTrigger()
        {
            Name = "";
            FromLog = "";
            TriggerTime = DateTime.Now;
        }
    }
}
