using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class Category
    {
        public int id;
        public String name;
        public Hashtable text;
        public Hashtable timers;
        public Hashtable overrides;

        public Category()
        {
            name = "";
            text = new Hashtable();
            timers = new Hashtable();
            overrides = new Hashtable();
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

    }
}
