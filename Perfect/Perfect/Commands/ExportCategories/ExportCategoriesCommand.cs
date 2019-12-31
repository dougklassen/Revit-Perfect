using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Models;
using DougKlassen.Revit.Perfect.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportCategoriesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            List<CategoryModel> categoryData = new List<CategoryModel>();
            Categories categories = dbDoc.Settings.Categories;
            foreach (Category cat in categories)
            {
                categoryData.Add(new CategoryModel(cat));
            }

            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = dbDoc.Title + " categories.json",
                Filter = "JSON file|*.json",
                Title = "Save Categories Catalog"
            };
            DialogResult result = saveDialog.ShowDialog();
            if (saveDialog.FileName == String.Empty || result == DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            ICategoryCatalogRepo catRepo = new CategoryCatalogJsonRepo(saveDialog.FileName);
            catRepo.WriteScheduleCatalog(categoryData);

            return Result.Succeeded;
        }
    }
}
