using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Standardize view names by prefixing views placed on a sheet with the sheet number
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class AuditViewNamesCommand : IExternalCommand
    {
        IEnumerable<Viewport> projectViewports;
        List<View> projectViews;
        StringBuilder cmdResultMsg = new StringBuilder();

        String separator = " ";
        //TODO: load from config file
        String prefixRegexString = @"^(?:SK)?\d\d\d[a-z]?$";
        
        Int16 viewCount = 0;
        Int16 placedViewCount = 0;
        Int16 unplacedViewCount = 0;
        Int16 renamedViewCount = 0;
        Int16 failedRenameCount = 0;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            EnterRegexWindow window = new EnterRegexWindow();
            window.RegexValue = prefixRegexString;

            window.ShowDialog();

            if (window.DialogResult == false)
            {
                return Result.Cancelled;
            }

            Regex prefixRegex = new Regex(window.RegexValue);

            cmdResultMsg.AppendLine("View Name Audit");
            cmdResultMsg.Append("=====================\n");

            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            //Collect all plan, section, elevation, 3d, and drafting views
            var viewCollector = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewPlan));
            viewCollector = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewSection))
                .UnionWith(viewCollector);
            viewCollector = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(View3D))
                .UnionWith(viewCollector);
            viewCollector = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewDrafting))
                .UnionWith(viewCollector);
            projectViews = viewCollector
                .Cast<View>()
                .Where(v => !v.IsTemplate)
                .ToList();

            projectViewports = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(Viewport))
                .Cast<Viewport>();

            var sheetNumbers = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Select(s => s.SheetNumber);

            Boolean sheetNumbersAreValid = true;
            foreach (String sheetNumber in sheetNumbers)
            {
                if (!prefixRegex.IsMatch(sheetNumber))
                {
                    sheetNumbersAreValid = false;
                    cmdResultMsg.AppendFormat("Invalid sheet number: {0}\n", sheetNumber);
                }
            }
            if (!sheetNumbersAreValid)
            {
                cmdResultMsg.AppendFormat("\nInvalid sheet numbers found, cannot audit view names\n");
                TaskDialog.Show("View Name Audit", cmdResultMsg.ToString());
                return Result.Failed;
            }

            Regex splitRegex = new Regex(separator);

            using (Transaction t = new Transaction(dbDoc, "Standardize View Names"))
            {
                t.Start();

                foreach (View view in projectViews)
                {
                    viewCount++;

                    String existingName = view.Name; 
                    String[] segments = splitRegex.Split(existingName);
                    Boolean hasPrefix = segments.Count() > 0 && prefixRegex.IsMatch(segments[0]);
                    String existingPrefix = segments[0];
                    String body = hasPrefix ? existingName.Substring(existingPrefix.Length).Trim() : existingName.Trim();
                    
                    if (IsPlaced(view))
                    {
                        String sheetNumber = view.LookupParameter("Sheet Number").AsString();

                        placedViewCount++;
                        if (hasPrefix)
                        {
                            if(existingPrefix != sheetNumber)
                            {
                                String newName = String.Format("{0}{1}{2}", sheetNumber, separator, body);
                                TryRenameView(view, newName);
                            }
                        }
                        else
                        {
                            String newName = String.Format("{0}{1}{2}", sheetNumber, separator, body);
                            TryRenameView(view, newName);
                        }
                    }
                    else
                    {
                        if (hasPrefix)
                        {
                            TryRenameView(view, body);
                        }
                        unplacedViewCount++;
                    }
                }

                t.Commit();
            }

            cmdResultMsg.AppendFormat("\nViews in project: {0}\n", viewCount);
            cmdResultMsg.AppendFormat("Views placed on sheets: {0}\n", placedViewCount);
            cmdResultMsg.AppendFormat("Unplaced views: {0}\n", unplacedViewCount);
            cmdResultMsg.AppendFormat("Renamed views: {0}\n", renamedViewCount);
            cmdResultMsg.AppendFormat("Failed rename attempts: {0}\n", failedRenameCount);

            TaskDialog.Show("View Name Audit", cmdResultMsg.ToString());
            return Result.Succeeded;
        }

         private Boolean IsPlaced(View view)
        {
            if (projectViewports.Where(vp => view.Id == vp.ViewId).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TryRenameView(View view, String newName)
        {
            IEnumerable<String> projectViewNames = projectViews.Select(v => v.Name);
            //skip if there is a View name conflict
            if (!projectViewNames.Contains(newName))
            {
                renamedViewCount++;
                cmdResultMsg.AppendFormat("{0} => {1}\n", view.Name, newName);
                view.Name = newName;
            }
            else
            {
                failedRenameCount++;
                cmdResultMsg.AppendFormat("{0}: Name already in use\n", newName);
            }
        }
    }
}
