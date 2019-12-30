using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Models;
using DougKlassen.Revit.Perfect.Repositories;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Binding = Autodesk.Revit.DB.Binding;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportParametersCommand : IExternalCommand
    {
        IEnumerable<ParameterModel> modelParams = new List<ParameterModel>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            List<ParameterModel> paramData = new List<ParameterModel>();

            BindingMap map = dbDoc.ParameterBindings;

            foreach (Binding binding in map)
            {
                paramData.Add(new ParameterModel(binding));
            }

            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = "params.json",
                Filter = "JSON file|*.json",
                Title = "Save Parameters Catalog"
            };
            DialogResult result = saveDialog.ShowDialog();
            if (saveDialog.FileName == String.Empty || result == DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            IParameterCatalogRepo paramRepo = new ParameterCatalogJsonRepo(saveDialog.FileName);
            paramRepo.WriteParameterCatalog(paramData);

            return Result.Succeeded;
        }
    }
}
