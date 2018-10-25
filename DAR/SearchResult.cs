using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class SearchResult
    {
        public SearchResult()
        {

        }
        public DateTime Logtime { get; set; }
        public Trigger Trigger { get; set; }
        public String Matchedtext{get; set;}
    }
}
