using Autodesk.Revit.DB;
using DougKlassen.Revit.Snoop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Snoop
{
    /// <summary>
    /// A collection of helper methods. These are focused on tasks such as retrieving
    /// collections of elements from the document, getting information about the local
    /// environment, or generating labels for use in naming Snoop data export files
    /// </summary>
    public static class SnoopHelpers
    {
        /// <summary>
        /// Return all elements in the specified document
        /// </summary>
        /// <param name="dbDoc">The specified documents</param>
        /// <returns>A collection of all elements in the document</returns>
        public static IEnumerable<Element> GetAllElements(this Document dbDoc)
        {
            IEnumerable<Element> allElements = new List<Element>();

            ElementFilter allElementsFilter = new LogicalOrFilter(new ElementIsElementTypeFilter(false), new ElementIsElementTypeFilter(true));
            allElements = new FilteredElementCollector(dbDoc).WherePasses(allElementsFilter);

            return allElements;
        }

        /// <summary>
        /// Return all parameter definitions in the project
        /// </summary>
        /// <param name="dbDoc">The specified document</param>
        /// <returns>All parameter definitions in the project</returns>
        public static IEnumerable<Definition> GetAllDefinitions(this Document dbDoc)
        {
            string msg = string.Empty;
            IEnumerable<Definition> allDefinitions = new List<Definition>();

            IEnumerable<Element> allElements = dbDoc.GetAllElements();

            foreach (Element e in allElements)
            {
                List<Definition> defs = new List<Definition>();
                foreach (Parameter p in e.Parameters)
                {
                    defs.Add(p.Definition);
                }

                allDefinitions = allDefinitions.Union(defs);
            }

            return allDefinitions;
        }

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
        /// Get an ordered list of all task units friendly names, sorted by order in the enumeration
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
        /// Get name with a numerical postfix. If the name is already postfixed, the postfix is incremented. If not, a postfix is added
        /// </summary>
        /// <param name="proposedName">The name to be incremented</param>
        /// <returns>A new version of the name</returns>
        public static String GetIncrementedName(String originalName)
        {
            String prefix;
            Int32 postFixValue;

            Regex postfixRegEx = new Regex(@"(.*)\.([\d]{3})$");
            Match m = postfixRegEx.Match(originalName);
            if (m.Success)
            {
                prefix = m.Groups[1].Value;
                postFixValue = Int32.Parse(m.Groups[2].Value) + 1;
            }
            else
            {
                prefix = originalName;
                postFixValue = 0;
            }

            return String.Format("{0}.{1}", prefix, postFixValue.ToString("D3"));
        }

        /// <summary>
        /// Method to generate a unique view name by appending or incrementing a number
        /// </summary>
        /// <param name="dbDoc">the document to use</param>
        /// <param name="newName">the name to be made unique</param>
        /// <returns>either the same string, or the string with an incremented number appended</returns>
        public static String GetUniqueViewName(this Document dbDoc, String newName)
        {
            Regex sameNameWithOptionalNumber = new Regex(newName + "([(](?<num>[0-9]+)[)]$)?");
            FilteredElementCollector c = new FilteredElementCollector(dbDoc);
            c.OfCategory(BuiltInCategory.OST_Views);
            var q = from View v in c where sameNameWithOptionalNumber.IsMatch(v.Name) select sameNameWithOptionalNumber.Match(v.Name);
            if (q.Count() == 0)
            {
                return newName;
            }
            else
            {
                Int32 hightestSuffix = q.Max(
                    match =>
                    {
                        //note: it's ok if both newName and newName(0) exist.
                        if (String.Empty == match.Groups["num"].Value)
                        {
                            return 0;
                        }
                        else
                        {
                            return Int32.Parse(match.Groups["num"].Value);
                        }
                    });
                return newName + "(" + (hightestSuffix + 1) + ")";
            }
        }

        /// <summary>
        /// Add a time stamp to a file path
        /// </summary>
        /// <param name="filePath">The original file path</param>
        /// <returns>The file path with an added timestamp</returns>
        public static String GetTimeStampFileName(String filePath)
        {
            Regex fileNameRegEx = new Regex(@"(.*)\.(.*)");
            Match match = fileNameRegEx.Match(filePath);

            return String.Format("{0}-{1}.{2}",
                match.Groups[1],
                GetTimeStamp(),
                match.Groups[2]);
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
