using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Query.Models;
using DougKlassen.Revit.Query.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportParametersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<ParameterElement> userParams =
                new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ParameterElement))
                .ToElements()
                .Cast<ParameterElement>();

            List<ParameterModel> paramData = new List<ParameterModel>();
            BindingMap map = dbDoc.ParameterBindings;

            foreach (ParameterElement param in userParams)
            {
                paramData.Add(new ParameterModel(param, map));
            }

            IEnumerable<Element> allElements = Helpers.GetAllElements(dbDoc);
            foreach (BuiltInParameter builtIn in Enum.GetValues(typeof(BuiltInParameter)))
            {
                paramData.Add(ParameterModel.GetBuiltInParameter(builtIn, map, allElements));

            }

            //TODO: autoincrement file name
            //TODO: cross reference with categories

            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = dbDoc.Title + "-parameters-" + Helpers.GetTimeStamp() + ".json",
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
