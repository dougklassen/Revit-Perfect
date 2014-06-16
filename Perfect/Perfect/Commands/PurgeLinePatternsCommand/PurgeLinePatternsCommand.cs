using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Perfect.Interface;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PurgeLinePatternsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<LinePatternElement> docLinePatterns = new FilteredElementCollector(dbDoc).OfClass(typeof(LinePatternElement)).AsEnumerable().Cast<LinePatternElement>();

            PurgeElementsWindow purgeWindow = new PurgeElementsWindow(dbDoc, typeof(LinePatternElement));
            purgeWindow.PurgeRegExString = @"^IMPORT-.*$";

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
