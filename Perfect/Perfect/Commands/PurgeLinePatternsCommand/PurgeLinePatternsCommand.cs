using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PurgeLinePatternsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<LinePatternElement> docLinePatterns = new FilteredElementCollector(dbDoc).OfClass(typeof(LinePatternElement)).AsEnumerable().Cast<LinePatternElement>();

            Regex importPatternRegEx = new Regex(@"^IMPORT-.*");
            var importLinePatterns = docLinePatterns.Where(p => importPatternRegEx.IsMatch(p.Name));

            String msg = String.Empty;

            foreach (LinePatternElement pattern in importLinePatterns)
            {
                msg += pattern.Name + '\n';
            }

            TaskDialog.Show("Purge Line Patterns", msg,);

            return Result.Succeeded;
        }
    }
}
