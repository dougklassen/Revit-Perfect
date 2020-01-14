using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Perfect
{
    public static class Helpers
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
        /// Create a quantity schedule in the specified document using a template loaded from an external source.
        /// If there are errors in creating the schedule, the errors will be returned and the schedule won't be created.
        /// </summary>
        /// <param name="dbDoc">The document to which the schedule will be added</param>
        /// <param name="template">The template to use in creating the </param>
        /// <returns>A list of errors that occured during schedule creation. A zero length list indicates sucessful creation
        /// of the schedule</returns>
        public static IEnumerable<String> CreateTemplate(Document dbDoc, QuantityScheduleTemplate template)
        {
            List<String> errors = new List<String>();

            using (Transaction t = new Transaction(dbDoc))
            {
                t.Start("Create quantity schedule");
                Category cat = Category.GetCategory(dbDoc, (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), template.ElementCategory));
                ElementId catId = cat.Id;

                //TODO: make sure the name isn't already in use
                ViewSchedule sched = ViewSchedule.CreateSchedule(dbDoc, catId);

                sched.Name = template.FilterParameterValue + " - " + template.FilterParameterValueLabel;

                IEnumerable<SchedulableField> elligibleFields = sched.Definition.GetSchedulableFields();

                //add built in parameter fields
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.BuiltInParameter))
                {
                    String builtInName = field.FieldValue;
                    Boolean locatedParam = false;
                    foreach (SchedulableField sf in elligibleFields)
                    {
                        if (sf.GetName(dbDoc) == builtInName)
                        {
                            locatedParam = true;
                            ScheduleField f = sched.Definition.AddField(sf);
                            f.ColumnHeading = String.Format("{0} ({1})", field.Label, field.Units);
                            break;
                        }
                    }

                    if (locatedParam == false)
                    {
                        errors.Add(sched.Name + ": built in parameter " + field + " wasn't elligible to be added to schedule");
                    }
                }

                //add project parameter fields
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.ProjectParameter))
                {

                }

                //add calculated fields
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.Calculation))
                {

                }

                t.Commit();
            }

            return errors;
        }
    }
}