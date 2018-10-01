using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class SearchText
    {
        public String searchtext;
        public Boolean regexEnabled;
        public SearchText()
        {
            searchtext = "";
            regexEnabled = false;
        }
        public String Searchtext
        {
            get { return searchtext; }
            set { searchtext = value; } 
        }
        public Boolean Regex
        {
            get { return regexEnabled; }
            set { regexEnabled = value; }
        }
    }
}
