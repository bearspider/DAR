using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class TriggerGroup
    {
        public int id;
        public String triggerGroupName;
        public String comments;
        public Boolean defaultEnabled;
        public ArrayList children;
        public ArrayList triggers;
        public int parent;

        public TriggerGroup()
        {
            parent = 0;
            triggerGroupName = "New Group";
            comments = "";
            defaultEnabled = true;
            children = new ArrayList();
            triggers = new ArrayList();
        }
        public Boolean DefaultEnabled
        {
            get { return defaultEnabled; }
            set { defaultEnabled = value; }
        }
        public String Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        public String TriggerGroupName
        {
            get { return triggerGroupName; }
            set { triggerGroupName = value; }
        }
        public ArrayList Children
        {
            get { return children; }
            set { children = value; }
        }
        public int Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public void AddChild( int Id )
        {
            children.Add(Id);
        }
        public void AddTriggers(Trigger newTrigger)
        {
            triggers.Add(newTrigger);
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public void RemoveChild( String delGroup )
        {
            children.Remove(delGroup);
            //Delete trigger group from database
        }
    }
}
