using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DougKlassen.Revit.Snoop.Models
{
    public class SnoopConfig
    {
        public String HomeDirectoryPath { get; set; }
        public String ToDoListFilePath { get; set; }
        public String CurrentTaskFilePath { get; set; }
        public IEnumerable<SnoopTask> ToDoList { get; set; }
        public IEnumerable<String> ActiveProjects { get; set; }
    }
}
