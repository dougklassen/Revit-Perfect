using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Purge all Line Patterns whose names match the specified regex. The default regex selects all
    /// line patterns generated from imported CAD files 
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PurgeLinePatternsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<LinePatternElement> docLinePatterns = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(LinePatternElement))
                .AsEnumerable()
                .Cast<LinePatternElement>();

            RegexSelectElementsWindow purgeWindow = new RegexSelectElementsWindow(dbDoc, typeof(LinePatternElement));
            purgeWindow.SelectRegExString = @"^IMPORT-.*$";

            purgeWindow.ShowDialog();
            if (false == purgeWindow.DialogResult)
            {
                return Result.Cancelled;
            }

            ICollection<ElementId> patternsToDelete = new List<ElementId>();
            ElementId match;
            foreach (String patName in purgeWindow.MatchingElementsListBox.Items)
            {
                match = docLinePatterns.Where(p => patName == p.Name).FirstOrDefault().Id;
                if (null != match)
                {
                    patternsToDelete.Add(match);
                }
            }

            using (Transaction t = new Transaction(dbDoc, "Purge Line Patterns"))
            {
                t.Start();
                dbDoc.Delete(patternsToDelete);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
