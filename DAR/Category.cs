using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DAR
{
    public class Category
    {
        public int id;
        public String name;
        public String textOverlay;
        public String timerOverlay;
        public String textFontColor;
        public String timerFontColor;
        public String timerBarColor;
        public Boolean defaultCategory;
        public Boolean textColors;
        public Boolean timerColors;
        public ObservableCollection<CharacterOverride> characteroverrides;
        public ObservableCollection<OverlayText> availabletextoverlays;
        public ObservableCollection<OverlayTimer> availabletimeroverlays;

        public Category()
        {
            Name = "Default";
            TextOverlay = "Default";
            TimerOverlay = "Default";
            TextFontColor = "Yellow";
            TimerFontColor = "Gray";
            TimerBarColor = "Blue";
            DefaultCategory = false;
            textColors = false;
            timerColors = false;
            CharacterOverrides = new ObservableCollection<CharacterOverride>();
            AvailableTextOverlays = new ObservableCollection<OverlayText>();
            AvailableTimerOverlays = new ObservableCollection<OverlayTimer>();
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public String TextOverlay { get; set; }
        public String TimerOverlay { get; set; }
        public String TextFontColor { get; set; }
        public String TimerFontColor { get; set; }
        public String TimerBarColor { get; set; }
        public Boolean DefaultCategory { get; set; }
        public ArrayList Listtest { get; set; }
        public ObservableCollection<CharacterOverride> CharacterOverrides { get; set; }
        public ObservableCollection<OverlayText> AvailableTextOverlays { get; set; }
        public ObservableCollection<OverlayTimer> AvailableTimerOverlays { get; set; }
    }
}
