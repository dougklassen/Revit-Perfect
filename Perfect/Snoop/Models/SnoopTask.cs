using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Models
{
    public enum SnoopTasks
    {
        ExportAllData,
        Audit,
        Compact,
        AuditCompact
    }

    public class SnoopTask
    {
        public SnoopTasks Task { get; set; }

        public Dictionary<String, String> Parameters { get; set; }
    }
}