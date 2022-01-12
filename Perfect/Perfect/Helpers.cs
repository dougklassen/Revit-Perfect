using Autodesk.Revit.DB;
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
                String schedName = template.FilterParameterValue + " - " + template.FilterParameterValueLabel + " (" + cat.Name + ")";

                Int32 countMatch;
                Int32 renameTries = 0;
                do
                {
                    if (renameTries > 20)
                    {
                        throw new Exception("Couldn't generate schedule name");
                    }

                    countMatch = new FilteredElementCollector(dbDoc)
                        .OfClass(typeof(ViewSchedule))
                        .Cast<ViewSchedule>()
                        .Where(s => s.Name == schedName)
                        .Count();

                    if (countMatch > 0)
                    {
                        schedName = Helpers.GetIncrementedName(schedName);
                    }

                    renameTries++;
                } while (countMatch > 0);

                sched.Name = schedName;

                IEnumerable<SchedulableField> elligibleFields = sched.Definition.GetSchedulableFields();

                //add the parameter that will be used to filter the schedule
                //and set up filter
                #region filter parameter
                Boolean filterParamFound = false;
                foreach (SchedulableField sf in elligibleFields)
                {
                    if (sf.GetName(dbDoc) == template.FilterParameterName)
                    {
                        filterParamFound = true;
                        ScheduleField f = sched.Definition.AddField(sf);
                        f.IsHidden = true;

                        ScheduleFilter schedFilter = new ScheduleFilter(f.FieldId, ScheduleFilterType.Equal, template.FilterParameterValue);
                        sched.Definition.AddFilter(schedFilter);

                        break;
                    }
                }

                if (!filterParamFound)
                {
                    errors.Add(sched.Name + ": filter parameter " + template.FilterParameterName + " couldn't be used to filter the schedule");
                }
                #endregion filter parameter

                //add built in parameter fields
                #region builtIn params
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.BuiltInParameter))
                {
                    String builtInName = field.FieldValue;
                    Boolean paramFound = false;
                    foreach (SchedulableField sf in elligibleFields)
                    {
                        if (sf.GetName(dbDoc) == builtInName)
                        {
                            paramFound = true;
                            ScheduleField f = sched.Definition.AddField(sf);
                            if (String.IsNullOrEmpty(field.Label) || String.IsNullOrEmpty(field.Units))
                            {
                                f.IsHidden = true;
                            }
                            else
                            {
                                f.ColumnHeading = String.Format("{0} ({1})", field.Label, field.Units);
                            }
                            break;
                        }
                    }

                    if (!paramFound)
                    {
                        errors.Add(sched.Name + ": built in parameter " + field.FieldValue + " wasn't elligible to be added to schedule");
                    }
                }
                #endregion builtIn params

                //add project parameter fields
                #region project params
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.ProjectParameter))
                {
                    String projParamName = field.FieldValue;
                    Boolean paramFound = false;
                    foreach (SchedulableField sf in elligibleFields)
                    {
                        if (sf.GetName(dbDoc) == projParamName)
                        {
                            paramFound = true;
                            ScheduleField f = sched.Definition.AddField(sf);
                            if (String.IsNullOrEmpty(field.Label) || String.IsNullOrEmpty(field.Units))
                            {
                                f.IsHidden = true;
                            }
                            else
                            {
                                f.ColumnHeading = String.Format("{0} ({1})", field.Label, field.Units);
                            }
                            break;
                        }
                    }

                    if (!paramFound)
                    {
                        errors.Add(sched.Name + ": project parameter " + field.FieldValue + " wasn't elligible to be added to schedule");
                    }
                }
                #endregion project params

                //TODO: add calculated fields if API support is added
                #region calculated fields
                foreach (QuantityScheduleField field
                    in template.Fields.Where(f => f.Type == FieldType.Calculation))
                {
                }
                #endregion calculated fields

                t.Commit();
            }

            return errors;
        }

        #region Viz Helpers

        /// <summary>
        /// Generate a string description of an OverrideGraphicsSettings object
        /// </summary>
        /// <param name="settings">The OverrideGraphicsSettings object to be represented</param>
        /// <returns>A description of the OverrideGraphicsSettings</returns>
        public static String GetVizDescription(this OverrideGraphicSettings settings)
        {
            String desc = String.Empty;

            desc +=
                "Detail Level: " + settings.DetailLevel.ToString() + '\n' +
                "Halftone: " + settings.Halftone.ToString() + '\n' +
                "Projection Lines:\n" +
                "- Pattern Id: " + settings.ProjectionLinePatternId.ToString() + '\n' +
                "- Color: " + settings.ProjectionLineColor.GetVizDescription() + '\n' +
                "- Weight: " + settings.ProjectionLineWeight.ToString() + '\n' +
                "Surface Patterns:\n" +
                "- Foreground Visible: " + settings.IsSurfaceForegroundPatternVisible.ToString() + '\n' +
                "- Foreground Pattern Id: " + settings.SurfaceForegroundPatternId.ToString() + '\n' +
                "- Foreground Color: " + settings.SurfaceForegroundPatternColor.GetVizDescription() + '\n' +
                "- Background Visible: " + settings.IsSurfaceBackgroundPatternVisible.ToString() + '\n' +
                "- Background Pattern Id: " + settings.SurfaceBackgroundPatternId.ToString() + '\n' +
                "- Background Color: " + settings.SurfaceBackgroundPatternColor.GetVizDescription() + '\n' +
                "Surface Transparency: " + settings.Transparency.ToString() + '\n' +
                "Cut Lines:\n" +
                "- Pattern Id: " + settings.CutLinePatternId.ToString() + '\n' +
                "- Color: " + settings.CutLineColor.GetVizDescription() + '\n' +
                "- Weight: " + settings.CutLineWeight.ToString() + '\n' +
                "Cut Patterns:\n" +
                "- Foreground Pattern Visibility: " + settings.IsCutForegroundPatternVisible.ToString() + '\n' +
                "- Foreground Pattern Id: " + settings.CutForegroundPatternId.ToString() + '\n' +
                "- Foreground Color: " + settings.CutForegroundPatternColor.GetVizDescription() + '\n' +
                "- Background Pattern Visibility: " + settings.IsCutBackgroundPatternVisible.ToString() + '\n' +
                "- Background Pattern Id: " + settings.CutBackgroundPatternId.ToString() + '\n' +
                "- Background Color: " + settings.CutBackgroundPatternColor.GetVizDescription() + '\n';

            return desc;
        }

        /// <summary>
        /// Generate a string description of a Revit Color object
        /// </summary>
        /// <param name="color">A Revit Color</param>
        /// <returns>A description of the Color</returns>
        public static string GetVizDescription(this Autodesk.Revit.DB.Color color)
        {
            String desc = String.Empty;
            if (color.IsValid)
            {
                desc += String.Format("Red: {0} Green: {1} Blue: {2}", color.Red, color.Green, color.Blue);
            }
            else
            {
                desc += "Invalid";
            }
            return desc;
        }

        /// <summary>
        /// Get a VizColor object that represents a Revit Color object
        /// </summary>
        /// <param name="color">A Revit Color</param>
        /// <returns>A VizColor representation of the Revit Color</returns>
        public static VizColor GetVizModel(this Color color)
        {
            if (color.IsValid)
            {
                return new VizColor(color);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get a VizOverrides object that represents an OverrideGraphicSettings object
        /// </summary>
        /// <param name="settings">An OverrideGraphicSettings object</param>
        /// <returns>A VizOverrides representation of the OverrideGraphicsSettings object</returns>
        public static VizOverrides GetVizModel(this OverrideGraphicSettings settings)
        {
            return new VizOverrides(settings);
        }

        #endregion Viz Helpers

    }
}