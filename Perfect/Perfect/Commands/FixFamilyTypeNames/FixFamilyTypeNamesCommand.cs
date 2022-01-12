using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Fix family type naming. For families with only one type, set the family type name to match the family name
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FixFamilyTypeNamesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            StringBuilder resultText = new StringBuilder(String.Format("Family Type Naming - {0} {1}\n", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString()));
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<Family> familiesToProcess = new FilteredElementCollector(dbDoc).OfClass(typeof(Family)).ToElements().Cast<Family>();

            int singleTypeFamilyCount = 0;
            int symbolToRenameCount = 0;
            StringBuilder symbolRenameText = new StringBuilder();

            using (Transaction t = new Transaction(dbDoc, "Fix family type names"))
            {
                t.Start();

                foreach (Family family in familiesToProcess)
                {
                    IEnumerable<ElementId> symbolIds = family.GetFamilySymbolIds();
                    if (symbolIds.Count() == 1)
                    {
                        singleTypeFamilyCount++;
                        FamilySymbol symbol = dbDoc.GetElement(symbolIds.Single()) as FamilySymbol;
                        if (!symbol.Name.Equals(family.Name))
                        {
                            symbolToRenameCount++;
                            symbolRenameText.AppendFormat("<Rename> {0}:{1}\n", family.Name, symbol.Name);
                            symbol.Name = family.Name;
                        }
                    }
                }

                resultText.AppendLine("=================");
                resultText.AppendFormat("{0} families found\n", familiesToProcess.Count());
                resultText.AppendFormat("{0} families with single type found\n", singleTypeFamilyCount);
                resultText.AppendFormat("{0} family types to rename\n", symbolToRenameCount);
                resultText.AppendLine("=================");
                resultText.Append(symbolRenameText);
                TaskDialog dlg = new TaskDialog("Fix Family Type Names");
                dlg.MainContent = resultText.ToString();
                //TODO: show only "Ok" if there are no changes to make
                dlg.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                TaskDialogResult result = dlg.Show();

                if (result == TaskDialogResult.Ok)
                {
                    t.Commit();
                    return (Result.Succeeded);
                }
                else
                {
                    t.RollBack();
                    return (Result.Cancelled);
                }
            }
        }
    }
}
