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

        /// <summary>
        /// Generates a timestamp string in the format "yyyyMMdd-HHmmss"
        /// </summary>
        /// <returns>A timestamp string in the format "yyyyMMdd-HHmmss"</returns>
        public static String GetTimeStamp()
        {
            DateTime now = DateTime.Now;
            return String.Format("{0}-{1}",
                now.ToString("yyyyMMdd"),
                now.ToString("HHmmss"));
        }

        /// <summary>
        /// Return a collections of all Revit versions that may exist
        /// </summary>
        /// <returns>A collection of Revit versions denoted by year</returns>
        public static IEnumerable<String> GetRevitVersions()
        {
            int lowRange = 2014;
            Int32 highRange = 2030;
            for (int y = lowRange; y < highRange; y++)
            {
                yield return y.ToString();
            }
        }
    }
}
