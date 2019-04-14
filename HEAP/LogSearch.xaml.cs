using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HEAP
{
    /// <summary>
    /// Interaction logic for LogSearch.xaml
    /// </summary>
    public partial class LogSearch : Window
    {
        private ObservableCollection<SearchResult> logsearchlist = new ObservableCollection<SearchResult>();

        public LogSearch(ObservableCollection<CharacterProfile> profiles)
        {
            InitializeComponent();
            comboCharacter.ItemsSource = profiles;
            gridSearch.ItemsSource = logsearchlist;
            DateTime time = DateTime.Now;
            dateFrom.Value = time.AddHours(-1);
            dateTo.Value = time;
            for (int i = 0; i < 100; i++)
            {
                SearchResult newlog = new SearchResult
                {
                    Logtime = new DateTime(),
                    Trigger = new Trigger(),
                    Matchedtext = "Matched Text"
                };
                logsearchlist.Add(newlog);
            }
        }
    }
}
