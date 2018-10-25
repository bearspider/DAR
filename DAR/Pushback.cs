using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class Pushback
    {
        public String Character {get; set;}
        public String Spell {get; set;}
        public String PushType {get; set;}
        public String FromCharacter { get; set; }

        public Pushback()
        {
            Character = "";
            Spell = "";
            PushType = "";
            FromCharacter = "";
        }
    }
}
