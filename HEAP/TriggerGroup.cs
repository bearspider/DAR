using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEAP
{
    public class TriggerGroup
    {
        private int _id;
        private String _uniqueid;
        private String _triggerGroupName;
        private String _comments;
        private Boolean _defaultEnabled;
        private ArrayList _children;
        private ArrayList _triggers;
        private String _parent;

        public TriggerGroup()
        {
            _parent = "0";
            _triggerGroupName = "New Group";
            _comments = "";
            _defaultEnabled = true;
            _children = new ArrayList();
            _triggers = new ArrayList();
            _uniqueid = Guid.NewGuid().ToString();
        }
        public String UniqueId
        {
            get { return _uniqueid; }
            set { _uniqueid = value; }
        }
        public Boolean DefaultEnabled
        {
            get { return _defaultEnabled; }
            set { _defaultEnabled = value; }
        }
        public String Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }
        public String TriggerGroupName
        {
            get { return _triggerGroupName; }
            set { _triggerGroupName = value; }
        }
        public ArrayList Children
        {
            get { return _children; }
            set { _children = value; }
        }
        public ArrayList Triggers
        {
            get { return _triggers; }
            set { _triggers = value; }
        }
        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        public void AddChild(string UniqueId)
        {
            _children.Add(UniqueId);
        }
        public void AddTriggers(string newTrigger)
        {
            _triggers.Add(newTrigger);
        }
        public void RemoveTrigger(string removeTrigger)
        {
            _triggers.Remove(removeTrigger);
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public void RemoveChild(int todelete)
        {
            Boolean dodelete = false;
            foreach(int child in Children)
            {
                if(child == todelete)
                {
                    dodelete = true;
                }
            }
            if(dodelete)
            {
                _children.Remove(todelete);
            }           
        }
        public void RemoveChild( String delGroup )
        {
            _children.Remove(delGroup);
            //Delete trigger group from database
        }
    }
}
