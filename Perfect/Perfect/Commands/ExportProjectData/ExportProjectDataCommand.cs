﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using System;
using System.Windows.Forms;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportProjectDataCommand : IExternalCommand
    {        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            ProjectDataModel projectData = new ProjectDataModel(dbDoc);

            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = dbDoc.Title + "-project data-" + Helpers.GetTimeStamp() + ".json",
                Filter = "JSON file|*.json",
                Title = "Save Project Data Catalog"
            };
            DialogResult result = saveDialog.ShowDialog();
            if (saveDialog.FileName == String.Empty || result == DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            IProjectDataCatalogRepo catRepo = new ProjectDataCatalogJsonRepo(saveDialog.FileName);
            catRepo.WriteProjectDataCatalog(projectData);

            return Result.Succeeded;
        }
    }
}
