using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class Pushback
    {
        public String Character {get; set;}
        public String Spell {get; set;}
        public String PushType {get; set;}
        public String FromCharacter { get; set; }
        public Double Distance { get; set; }
        public DateTime TriggerTime { get; set; }
        public int Id { get; set; }

        public Pushback()
        {
            Id = 0;
            Character = "";
            Spell = "";
            PushType = "";
            FromCharacter = "";
            Distance = 0.0;
            TriggerTime = DateTime.Now;
        }
    }
}
