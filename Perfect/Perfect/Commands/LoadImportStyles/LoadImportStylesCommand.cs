using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Query.Models;
using DougKlassen.Revit.Query.Repositories;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class LoadImportStylesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            IEnumerable<ObjectStylesModel> importedStyles = null;

            String msg = String.Empty;

            OpenFileDialog dlg = new OpenFileDialog()
            {
                InitialDirectory = FileLocations.AddInDirectory,
                DefaultExt = ".json",
                Filter = "JSON files|*.json|Text Files|*.txt|All Files|*.*"
            };
            Boolean? dlgResult = dlg.ShowDialog();

            if (true != dlgResult.Value)
            {
                return Result.Cancelled;
            }

            try
            {
                IObjectStylesRepo stylesRepo = new ObjectStylesJsonRepo(dlg.FileName);
                importedStyles = stylesRepo.LoadObjectStyles();
            }
            catch (Exception)
            {
                TaskDialog.Show("Error", "Could not load styles from " + dlg.FileName);
                return Result.Failed;
            }

            using (Transaction t = new Transaction(dbDoc, "Update Imported Styles"))
            {
                t.Start();
                Categories allCats = dbDoc.Settings.Categories;
                foreach (Category c in allCats)
                {
                    foreach (Category sc in c.SubCategories)
                    {
                        ObjectStylesModel matchingStyle = importedStyles.Where(s => sc.Name == s.Name).FirstOrDefault();

                        if (null != matchingStyle)
                        {
                            if (matchingStyle.Delete)
                            {
                                //Delete will remove the category but not any linework belonging to it
                                //All elements that were on the corresponding layer will display per the parent category for the import
                                dbDoc.Delete(sc.Id);
                            }
                            else
                            {
                                sc.SetLineWeight(matchingStyle.ProjectionLineweight.Value, GraphicsStyleType.Projection);
                                sc.LineColor = matchingStyle.LineColor.GetColor();
                            }
                        }
                    }
                }
                t.Commit();
            }

            if (String.Empty != msg)
            {
                TaskDialog.Show("Import Result", msg);
            }

            return Result.Succeeded;
        }
    }
}
