using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class Trigger
    {
        public String name;
        public String regex;
        public String parent;
        public ArrayList child;
        public Trigger()
        {
            name = "New Trigger";
            regex = "";
            parent = "root";
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public String Regex
        {
            get { return regex; }
            set { regex = value; }
        }
        public String Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public ArrayList Child
        {
            get { return child; }
            set { child = value; }
        }
        public void AddChild(String grandChild)
        {
            child.Add(grandChild);
        }
    }
}
