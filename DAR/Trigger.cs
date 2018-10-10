using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DAR
{
    public class Trigger
    {
        public int id;
        public String name;
        public ArrayList profiles;
        public String searchText;
        public String comments;
        public Boolean regex;
        public Boolean fastcheck;
        public int parent;
        public Category triggerCategory;
        public String displaytext;
        public String clipboardtext;
        public Audio audioSettings;
        public String timerType;
        public String timerName;
        public int timerDuration;
        public int triggeredAgain;
        public BindingList<SearchText> endEarlyText;
        public int timerEndingDuration;
        public String timerEndingDisplayText;
        public String timerEndingClipboardText;
        public Audio timerEndingAudio;
        public Boolean timerEnding;
        public String timerEndedDisplayText;
        public String timerEndedClipboardText;
        public Boolean timerEnded;
        public Audio timerEndedAudio;
        public Boolean resetCounter;
        public int resetCounterDuration;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public Trigger()
        {
            id = 0;
            name = "New Trigger";
            profiles = new ArrayList();

            searchText = "";
            comments = "";
            regex = false;
            fastcheck = false;
            parent = 0;
            triggerCategory = new Category();
            displaytext = "";
            clipboardtext = "";
            audioSettings = new Audio();
            timerType = "";
            timerName = "";
            timerDuration = 0;
            triggeredAgain = 2;
            endEarlyText = new BindingList<SearchText>();
            //endEarlyText = "";
            timerEndingDuration = 0;
            timerEndingDisplayText = "";
            timerEndingClipboardText = "";
            timerEndingAudio = new Audio();
            timerEnding = false;
            timerEndedClipboardText = "";
            timerEndedDisplayText = "";
            timerEnded = false;
            timerEndedAudio = new Audio();
            resetCounter = false;
            resetCounterDuration = 0;
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public ArrayList Profiles
        {
            get { return profiles; }
            set { profiles = value; }
        }
        public String Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        public String SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }
        public Boolean Regex
        {
            get { return regex; }
            set { regex = value; }
        }
        public Boolean Fastcheck
        {
            get { return fastcheck; }
            set { fastcheck = value; }
        }
        public String Displaytext
        {
            get { return displaytext; }
            set { displaytext = value; }
        }
        public String Clipboardtext
        {
            get { return clipboardtext; }
            set { clipboardtext = value; }
        }
        public int Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public Category TriggerCategory
        {
            get { return triggerCategory; }
            set { triggerCategory = value; }
        }
        public Audio AudioSettings
        {
            get { return audioSettings; }
            set { audioSettings = value; }
        }
        public String TimerType
        {
            get { return timerType; }
            set { timerType = value; }
        }
        public String TimerName
        {
            get { return timerName; }
            set { timerName = value; }
        }
        public int TimerDuration
        {
            get { return timerDuration; }
            set { timerDuration = value; }
        }
        public int TriggeredAgain
        {
            get { return triggeredAgain; }
            set { triggeredAgain = value; }
        }
        public BindingList<SearchText> EndEarlyText
        {
            get { return endEarlyText; }
            set { endEarlyText = value; }
        }
        /*public String EndEarlyText
        {
            get { return endEarlyText; }
            set { endEarlyText = value; }
        }*/
        public int TimerEndingDuration
        {
            get { return timerEndingDuration; }
            set { timerEndingDuration = value; }

        }
        public String TimerEndingDisplayText
        {
            get { return timerEndingDisplayText; }
            set { timerEndingDisplayText = value; }
        }
        public String TimerEndingClipboardText
        {
            get { return timerEndingClipboardText; }
            set { timerEndingClipboardText = value; }
        }
        public Audio TimerEndingAudio
        {
            get { return timerEndingAudio; }
            set { timerEndingAudio = value; }
        }
        public Boolean TimerEnding
        {
            get { return timerEnding; }
            set { timerEnding = value; }
        }
        public String TimerEndedDisplayText
        {
            get { return timerEndedDisplayText; }
            set { timerEndedDisplayText = value; }
        }
        public String TimerEndedClipboardText
        {
            get { return timerEndedClipboardText; }
            set { timerEndedClipboardText = value; }
        }
        public Boolean TimerEnded
        {
            get { return timerEnded; }
            set { timerEnded = value; }
        }
        public Audio TimerEndedAudio
        {
            get { return timerEndedAudio; }
            set { timerEndedAudio = value; }
        }
        public Boolean ResetCounter
        {
            get { return resetCounter; }
            set { resetCounter = value; }
        }
        public int ResetCounterDuration
        {
            get { return resetCounterDuration; }
            set { resetCounterDuration = value; }
        }
        public void AddProfile(int profileId)
        {
            profiles.Add(profileId);
        }
    }
}
