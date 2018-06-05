using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Export callout text from all views on selected sheets
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class ExportDetailTextCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            var selectedSheetIds = uiDoc.Selection.GetElementIds().
                Where(id => dbDoc.GetElement(id).Category.Name == "Sheets");

            if (selectedSheetIds.Count() < 1)
            {
                TaskDialog.Show(
                    "Export Detail Text Error",
                    "Select one or more sheets before running command");

                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
