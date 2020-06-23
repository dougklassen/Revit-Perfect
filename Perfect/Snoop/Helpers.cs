using DougKlassen.Revit.Snoop.Models;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop
{
    public static class Helpers
    {
        /// <summary>
        /// Get a user-friendly name for a SnoopTaskType
        /// </summary>
        /// <param name="taskType">The SnoopTaskType</param>
        /// <returns>A user-friendly name</returns>
        public static String GetFriendlyName(this SnoopTaskType taskType)
        {
            switch (taskType)
            {
                case SnoopTaskType.ExportAllData:
                    return "Export Data";
                case SnoopTaskType.Audit:
                    return "Audit";
                case SnoopTaskType.Compact:
                    return "Compact";
                case SnoopTaskType.AuditCompact:
                    return "Audit & Compact";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get an ordered list of all task type friendly names, sorted by order in the enumeration
        /// </summary>
        /// <returns>A list of all available task types</returns>
        public static List<String> GetFriendlyTaskNames()
        {
            List<String> names = new List<String>();
            foreach (SnoopTaskType type in Enum.GetValues(typeof(SnoopTaskType)))
            {
                names.Add(type.GetFriendlyName());
            }
            return names;
        }

        /// <summary>
        /// Extension method 
        /// </summary>
        /// <param name="paramType"></param>
        /// <returns></returns>
        public static String GetFriendlyName(this SnoopParameterType paramType)
        {
            switch (paramType)
            {
                case SnoopParameterType.FileOutputDirectory:
                    return "Output Directory";
                default:
                    return null;
            }
        }
    }
}
