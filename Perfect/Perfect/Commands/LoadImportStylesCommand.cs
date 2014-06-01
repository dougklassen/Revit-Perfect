using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Perfect.Models;
using DougKlassen.Revit.Perfect.Repositories;
using DougKlassen.Revit.Perfect.StartUp;

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
                IObjectStylesRepo stylesRepo = new JsonFileObjectStyleRepo(dlg.FileName);
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
                            sc.SetLineWeight(matchingStyle.ProjectionLineweight.Value, GraphicsStyleType.Projection);
                            sc.LineColor = matchingStyle.LineColor;
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
