using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;

namespace DAR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new LiteDatabase(@"C:\Temp\eqtriggers.db"))
            {
                var col = db.GetCollection<CharacterProfile>("profiles");

                var player = new CharacterProfile
                {
                    Id = 2,
                    Name = "Houkaa",
                    ProfileName = "Houkaa(luclin)",
                    LogFile = @"C:\users\jared.haddix\desktop\eqlog_Dhurgan_luclin_20170917_190951.txt",
                    SpeechRate = 0,
                    VolumeValue = 90,
                    TimerFontColor = "Lime",
                    TimerBarColor = "Red",
                    TextFontColor = "Blue"
                };
                //col.Insert(player);
                col.EnsureIndex(x => x.Name);
                var r = col.FindOne(x => x.Name.Contains("Houkaa"));
                
            }
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterEditor newCharacter = new CharacterEditor();
            newCharacter.ShowDialog();
        }

    }
}
