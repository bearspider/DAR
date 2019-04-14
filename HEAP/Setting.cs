using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class Setting
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }

        public Setting()
        {
            Id = 0;
            Name = "";
            Value = "";
        }
    }
}
