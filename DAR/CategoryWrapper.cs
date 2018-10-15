using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class CategoryWrapper
    {
        public ObservableCollection<Category> CategoryList { get; set; }
        public ObservableCollection<OverlayText> OverlayTexts { get; set; }
        public ObservableCollection<OverlayTimer> OverlayTimers {get; set; }
        public CategoryWrapper()
        {
            CategoryList = new ObservableCollection<Category>();
            OverlayTexts = new ObservableCollection<OverlayText>();
            OverlayTimers = new ObservableCollection<OverlayTimer>();
        }
    }
}
