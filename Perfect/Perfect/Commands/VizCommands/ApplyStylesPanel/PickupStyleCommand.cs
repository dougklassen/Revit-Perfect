using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class PickupStyleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            String ttl = "Style Eyedropper";
            IVizSettingsRepo repo = new VizSettingsJsonRepo();
            VizSettings vizSettings = repo.LoadSettings();

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            View currentView = commandData.Application.ActiveUIDocument.ActiveView;
            ElementId sourceElementId;

            if (uiDoc.Selection.GetElementIds().Count == 1)
            {
                sourceElementId = uiDoc.Selection.GetElementIds().First();
            }
            else
            {
                try
                {
                    sourceElementId = uiDoc.Selection.PickObject(ObjectType.Element).ElementId;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                {
                    return Result.Cancelled;
                }
            }
            OverrideGraphicSettings selectedOverrides = currentView.GetElementOverrides(sourceElementId);

            vizSettings.CurrentOverrideStyle = selectedOverrides.GetVizModel();
            repo.WriteSettings(vizSettings);

            return Result.Succeeded;
        }
    }
}

