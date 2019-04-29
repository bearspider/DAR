using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HEAP
{
    public class Category
    {
        private int _id;
        private String _name;
        private String _textOverlay;
        private String _timerOverlay;
        private String _textFontColor;
        private String _timerFontColor;
        private String _timerBarColor;
        private Boolean _defaultCategory;
        private Boolean _textColors;
        private Boolean _textThis;
        private Boolean _timerColors;
        private Boolean _timerThis;
        private ObservableCollection<CharacterOverride> _characteroverrides;
        private ObservableCollection<OverlayText> _availabletextoverlays;
        private ObservableCollection<OverlayTimer> _availabletimeroverlays;

        public Category()
        {
            Name = "Default";
            TextOverlay = "Default";
            TimerOverlay = "Default";
            TextFontColor = "Yellow";
            TimerFontColor = "Gray";
            TimerBarColor = "Blue";
            DefaultCategory = false;
            TextColors = false;
            TextThis = true;
            TimerColors = false;
            TimerThis = true;
            CharacterOverrides = new ObservableCollection<CharacterOverride>();
            AvailableTextOverlays = new ObservableCollection<OverlayText>();
            AvailableTimerOverlays = new ObservableCollection<OverlayTimer>();
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public String TextOverlay
        {
            get { return _textOverlay; }
            set { _textOverlay = value; }
        }
        public String TimerOverlay
        {
            get { return _timerOverlay; }
            set { _timerOverlay = value; }
        }
        public String TextFontColor
        {
            get { return _textFontColor; }
            set { _textFontColor = value; }
        }
        public String TimerFontColor
        {
            get { return _timerFontColor; }
            set { _timerFontColor = value; }
        }
        public String TimerBarColor
        {
            get { return _timerBarColor; }
            set { _timerBarColor = value; }
        }
        public Boolean DefaultCategory
        {
            get { return _defaultCategory; }
            set { _defaultCategory = value; }
        }
        public Boolean TextColors
        {
            get { return _textColors; }
            set { _textColors = value; }
        }
        public Boolean TextThis
        {
            get { return _textThis; }
            set { _textThis = value; }
        }
        public Boolean TimerColors
        {
            get { return _timerColors; }
            set { _timerColors = value; }
        }
        public Boolean TimerThis
        {
            get { return _timerThis; }
            set { _timerThis = value; }
        }
        public ObservableCollection<CharacterOverride> CharacterOverrides
        {
            get { return _characteroverrides; }
            set { _characteroverrides = value; }
        }
        public ObservableCollection<OverlayText> AvailableTextOverlays
        {
            get { return _availabletextoverlays; }
            set { _availabletextoverlays = value; }
        }
        public ObservableCollection<OverlayTimer> AvailableTimerOverlays
        {
            get { return _availabletimeroverlays; }
            set { _availabletimeroverlays = value; }
        }
        public bool Equals(Category compareto)
        {
            bool rval = false;
            if(
                _id == compareto.Id
                && _name == compareto.Name
                && _textOverlay == compareto.TextOverlay
                && _timerOverlay == compareto.TimerOverlay
                && _textFontColor == compareto.TextFontColor
                && _timerFontColor == compareto.TimerFontColor
                && _timerBarColor == compareto.TimerBarColor
                && _defaultCategory == compareto.DefaultCategory
                && _textColors == compareto.TextColors
                && _textThis == compareto.TextThis
                && _timerColors == compareto.TimerColors
                && _timerThis == compareto.TimerThis)
            {
                rval = true;
            }
            else
            {
                return false;
            }
            int overridecount = CharacterOverrides.Count;
            int textoverlaycount = AvailableTextOverlays.Count;
            int timeroverlaycount = AvailableTimerOverlays.Count;
            int compareoverridecount = compareto.CharacterOverrides.Count;
            int comparetextoverlaycount = compareto.AvailableTextOverlays.Count;
            int comparetimeroverlaycount = compareto.AvailableTimerOverlays.Count;
            if(
                overridecount == compareoverridecount
                && textoverlaycount == comparetextoverlaycount
                && timeroverlaycount == comparetimeroverlaycount
                )
            {
                for(int i = 0; i < overridecount; i++)
                {
                    if(CharacterOverrides[i].Equals(compareto.CharacterOverrides[i]))
                    {
                        rval = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                for(int j = 0; j < textoverlaycount; j++)
                {
                    if(AvailableTextOverlays[j].Equals(compareto.AvailableTextOverlays[j]))
                    {
                        rval = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                for(int k = 0; k < timeroverlaycount; k++)
                {
                    if(AvailableTimerOverlays[k].Equals(compareto.AvailableTimerOverlays[k]))
                    {
                        rval = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                rval = true;
            }
            else
            {
                return false;
            }
            return rval;
        }
        public int? GetIndex(string profilename)
        {
            int counter = 0;
            foreach (CharacterOverride charoverride in _characteroverrides)
            {
                if(charoverride.ProfileName == profilename)
                {
                    return counter;
                }
                counter++;
            }
            return null;
        }
    }
}
