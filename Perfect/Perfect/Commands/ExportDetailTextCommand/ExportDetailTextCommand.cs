using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Export callout text from all views sheets selected by the user. Sheets must be
    /// selected prior to invoking the command.
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

            FilteredElementCollector textCollector;
            ElementId textNotesCategoryId = new ElementId(BuiltInCategory.OST_TextNotes);
            StringBuilder output = new StringBuilder();
            foreach (var sheetId in selectedSheetIds)
            {
                var sheet = (ViewSheet)dbDoc.GetElement(sheetId);
                var viewsOnSheet = sheet.GetAllPlacedViews();
                foreach (var viewId in viewsOnSheet)
                {
                    textCollector = new FilteredElementCollector(dbDoc, viewId)
                        .OfCategoryId(textNotesCategoryId);

                    foreach (TextNote textNote in textCollector)
                    {
                        output.AppendFormat("{0}\t{1}\t{2}\n",
                            textNote.Text,
                            sheet.SheetNumber,
                            dbDoc.GetElement(viewId).Name);
                    }
                }
            }

            String outputFilePath;
            var saveDialog = new SaveFileDialog();
            saveDialog.FileName = "Exported callout text.txt";
            saveDialog.DefaultExt = ".txt";
            saveDialog.Filter = "Text Documents (.txt)|*.txt";
            var result = saveDialog.ShowDialog();
            switch (result)
            {
                case DialogResult.OK:
                    outputFilePath = saveDialog.FileName;
                    break;
                case DialogResult.Cancel:
                    return Result.Cancelled;
                default:
                    return Result.Failed;
            }

            try
            {
                File.WriteAllText(outputFilePath, output.ToString());
                return Result.Succeeded;
            }
            catch
            {
                TaskDialog.Show(
                    "Error Saving Output",
                    "Could not save file " + outputFilePath);
                return Result.Failed;
            }
        }
    }
}
