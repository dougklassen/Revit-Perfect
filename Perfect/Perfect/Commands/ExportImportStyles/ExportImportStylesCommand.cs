using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportImportStylesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            List<ObjectStylesModel> docObjectStyles = new List<ObjectStylesModel>();
            //regex identifying categories associated with imported AutoCAD files
            //Revit names these categories after the filename of the CAD file, so they end with dwg
            Regex importCatRegex = new Regex(@".+\.dwg", RegexOptions.IgnoreCase);

            IEnumerable<Category> importCats = dbDoc.Settings.Categories
                .Cast<Category>()
                .Where(c => importCatRegex.IsMatch(c.Name));
            IEnumerable<Category> layerCats = new List<Category>();

            string msg = string.Empty;
            foreach (Category c in importCats)
            {
                foreach (Category sc in c.SubCategories)
                {
                    //If no category has been found yet with this name
                    if (!docObjectStyles.Any(x => sc.Name == x.Name))
                    {
                        docObjectStyles.Add(new ObjectStylesModel()
                        {
                            Name = sc.Name,
                            ProjectionLineweight = sc.GetLineWeight(GraphicsStyleType.Projection),
                            LineColor = new ColorModel(sc.LineColor),
                            Delete = false
                        });
                    }
                }
            }

            if (String.Empty != msg)
            {
                TaskDialog.Show("Import Categories", msg);
            }

            SaveFileDialog dlg = new SaveFileDialog()
            {
                InitialDirectory = FileLocations.AddInDirectory,
                FileName = ((null == dbDoc.Title) ? "exported" : dbDoc.Title) + "-styles-" + Helpers.GetTimeStamp(),
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
                IObjectStylesRepo destinationRepo = new ObjectStylesJsonRepo(dlg.FileName);
                destinationRepo.WriteObjectStyles(docObjectStyles);
            }
            catch (Exception)
            {
                TaskDialog.Show("Error", "Could not export to " + dlg.FileName);
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
