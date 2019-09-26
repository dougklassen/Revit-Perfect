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
            //retrieve currently selected sheets
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            var selectedSheetIds = uiDoc.Selection.GetElementIds().
                Where(id => dbDoc.GetElement(id).Category.Name == "Sheets");

            //warn the user if no sheets are selected
            if (selectedSheetIds.Count() < 1)
            {
                TaskDialog.Show(
                    "Export Detail Text Error",
                    "Select one or more sheets before running command");

                return Result.Failed;
            }

            //retrieve and log all text notes on views on all selected sheets
            Int32 textNoteCount = 0;
            FilteredElementCollector textCollector;
            ElementId textNotesCategoryId = new ElementId(BuiltInCategory.OST_TextNotes);
            StringBuilder output = new StringBuilder();
            //step through sheeets
            foreach (var sheetId in selectedSheetIds)
            {
                var sheet = (ViewSheet)dbDoc.GetElement(sheetId);
                var viewsOnSheet = sheet.GetAllPlacedViews();
                //step through views on each sheet
                foreach (var viewId in viewsOnSheet)
                {
                    textCollector = new FilteredElementCollector(dbDoc, viewId)
                        .OfCategoryId(textNotesCategoryId);
                    //step through text notes in each view
                    foreach (TextNote textNote in textCollector)
                    {
                        var text = textNote.Text;
                        //Revit TextNote.Text is terminated by a CR which needs to be stripped
                        Char[] charsToTrim = { '\x000A', '\x000D', '\x0009', '\x0020' }; //LF, CR, tab, space
                        text = text.Trim(charsToTrim);
                        //TODO: skip common text such as "TYP" or "SIM"
                        output.AppendFormat("{0}\t{1}\t{2}\r\n",
                            text,
                            sheet.SheetNumber,
                            dbDoc.GetElement(viewId).Name);
                        textNoteCount++;
                    }
                }
            }

            //prompt the user for an output location
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
            //write the log to the specified file
            try
            {
                File.WriteAllText(outputFilePath, output.ToString());
                TaskDialog.Show("Text Notes Exported", textNoteCount + " text notes exported");
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
