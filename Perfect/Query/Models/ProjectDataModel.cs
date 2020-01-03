using Autodesk.Revit.DB;
using System;

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

        public ProjectDataModel(Document dbDoc)
        {
            ProjectInfo info = dbDoc.ProjectInformation;
            Name = info.Name;
            Address = info.Address;
            ProjectNumber = info.Number;
            Status = info.Status;
            ClientName = info.ClientName;
            FilePath = dbDoc.PathName;
        }
    }
}
