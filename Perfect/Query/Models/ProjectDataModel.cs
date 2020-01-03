using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DougKlassen.Revit.Query.Models
{
    public class ProjectDataModel
    {
        public String Name { get; set; }
        public String Address { get; set; }
        public String ProjectNumber { get; set; }
        public String Status { get; set; }
        public String ClientName { get; set; }
        public String FilePath { get; set; }
        public Int32? FileSize { get; set; }
        public Int32? ElementCount { get; set; }
    }
}
