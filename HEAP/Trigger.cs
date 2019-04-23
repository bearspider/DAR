using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace HEAP
{
    public class Trigger
    {
        private int _id;
        private String _uniqueid;
        private String _name;
        private ArrayList _profiles;
        private String _searchText;
        private String _comments;
        private Boolean _regex;
        private Boolean _fastcheck;
        private String _digest;
        private string _parent;
        private int _triggerCategory;
        private String _displaytext;
        private String _clipboardtext;
        private Audio _audioSettings;
        private String _timerType;
        private String _timerName;
        private int _timerDuration;
        private int _triggeredAgain;
        private BindingList<SearchText> _endEarlyText;
        private int _timerEndingDuration;
        private String _timerEndingDisplayText;
        private String _timerEndingClipboardText;
        private Audio _timerEndingAudio;
        private Boolean _timerEnding;
        private String _timerEndedDisplayText;
        private String _timerEndedClipboardText;
        private Boolean _timerEnded;
        private Audio _timerEndedAudio;
        private Boolean _resetCounter;
        private int _resetCounterDuration;
        public string UniqueId
        {
            get { return _uniqueid; }
            set { _uniqueid = value; }
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public Trigger()
        {
            _id = 0;
            _name = "New Trigger";
            _profiles = new ArrayList();
            _uniqueid = Guid.NewGuid().ToString();
            _searchText = "";
            _comments = "";
            _regex = false;
            _fastcheck = false;
            _digest = "";
            _parent = "0";
            _triggerCategory = 0;
            _displaytext = "";
            _clipboardtext = "";
            _audioSettings = new Audio();
            _timerType = "";
            _timerName = "";
            _timerDuration = 0;
            _triggeredAgain = 2;
            _endEarlyText = new BindingList<SearchText>();
            //endEarlyText = "";
            _timerEndingDuration = 0;
            _timerEndingDisplayText = "";
            _timerEndingClipboardText = "";
            _timerEndingAudio = new Audio();
            _timerEnding = false;
            _timerEndedClipboardText = "";
            _timerEndedDisplayText = "";
            _timerEnded = false;
            _timerEndedAudio = new Audio();
            _resetCounter = false;
            _resetCounterDuration = 0;
        }
        public String Digest
        {
            get { return _digest; }
            set { _digest = value; }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public ArrayList Profiles
        {
            get { return _profiles; }
            set { _profiles = value; }
        }
        public String Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }
        public String SearchText
        {
            get { return _searchText; }
            set { _searchText = value; }
        }
        public Boolean Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }
        public Boolean Fastcheck
        {
            get { return _fastcheck; }
            set
            {
                _fastcheck = value;
            }
        }
        public String Displaytext
        {
            get { return _displaytext; }
            set { _displaytext = value; }
        }
        public String Clipboardtext
        {
            get { return _clipboardtext; }
            set { _clipboardtext = value; }
        }
        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        public int TriggerCategory
        {
            get { return _triggerCategory; }
            set { _triggerCategory = value; }
        }
        public Audio AudioSettings
        {
            get { return _audioSettings; }
            set { _audioSettings = value; }
        }
        public String TimerType
        {
            get { return _timerType; }
            set { _timerType = value; }
        }
        public String TimerName
        {
            get { return _timerName; }
            set { _timerName = value; }
        }
        public int TimerDuration
        {
            get { return _timerDuration; }
            set { _timerDuration = value; }
        }
        public int TriggeredAgain
        {
            get { return _triggeredAgain; }
            set { _triggeredAgain = value; }
        }
        public BindingList<SearchText> EndEarlyText
        {
            get { return _endEarlyText; }
            set { _endEarlyText = value; }
        }
        /*public String EndEarlyText
        {
            get { return endEarlyText; }
            set { endEarlyText = value; }
        }*/
        public int TimerEndingDuration
        {
            get { return _timerEndingDuration; }
            set { _timerEndingDuration = value; }

        }
        public String TimerEndingDisplayText
        {
            get { return _timerEndingDisplayText; }
            set { _timerEndingDisplayText = value; }
        }
        public String TimerEndingClipboardText
        {
            get { return _timerEndingClipboardText; }
            set { _timerEndingClipboardText = value; }
        }
        public Audio TimerEndingAudio
        {
            get { return _timerEndingAudio; }
            set { _timerEndingAudio = value; }
        }
        public Boolean TimerEnding
        {
            get { return _timerEnding; }
            set { _timerEnding = value; }
        }
        public String TimerEndedDisplayText
        {
            get { return _timerEndedDisplayText; }
            set { _timerEndedDisplayText = value; }
        }
        public String TimerEndedClipboardText
        {
            get { return _timerEndedClipboardText; }
            set { _timerEndedClipboardText = value; }
        }
        public Boolean TimerEnded
        {
            get { return _timerEnded; }
            set { _timerEnded = value; }
        }
        public Audio TimerEndedAudio
        {
            get { return _timerEndedAudio; }
            set { _timerEndedAudio = value; }
        }
        public Boolean ResetCounter
        {
            get { return _resetCounter; }
            set { _resetCounter = value; }
        }
        public int ResetCounterDuration
        {
            get { return _resetCounterDuration; }
            set { _resetCounterDuration = value; }
        }
        public void AddProfile(int profileId)
        {
            _profiles.Add(profileId);
        }
    }
}
