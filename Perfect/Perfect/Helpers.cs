using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect
{
    public static class Helpers
    {
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
    }
}