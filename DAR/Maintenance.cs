using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAR
{
    public class Maintenance
    {
        public String AutoArchive { get; set; }
        public String ArchiveFolder { get; set; }
        public String ArchiveSchedule { get; set; }
        public String AutoDelete { get; set; }
        public String CompressArchive { get; set; }
        public int ArchiveDays { get; set; }
        public DateTime LastArchive { get; set; }
        public Maintenance ()
        {
            AutoArchive = "false";
            ArchiveFolder = "";
            ArchiveSchedule = "";
            AutoDelete = "False";
            CompressArchive = "False";
            ArchiveDays = 90;
            LastArchive = DateTime.Now.AddDays(-90);
        }
    }
}
