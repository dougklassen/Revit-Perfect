using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Reset graphic overrides for all elements in the current view.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ResetGraphicsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            String ttl = "Viz-Reset Graphics Overrides";
            String msg = String.Empty;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            View currentView = commandData.Application.ActiveUIDocument.ActiveView;
            msg += "Current view: " + currentView.Name + '\n';
            if (!currentView.AreGraphicsOverridesAllowed())
            {
                msg += "Graphic overrides are not allowed in the current view";
                TaskDialog.Show(ttl, msg);
                return Result.Failed;
            }

            List<Element> elementsToCheck = Helpers.GetAllElements(dbDoc).ToList();
            msg += "All element count: " + elementsToCheck.Count + '\n';

            foreach (Element e in elementsToCheck)
            {
                using (Transaction t = new Transaction(dbDoc))
                {
                    t.Start("Viz-" + ttl);
                    currentView.SetElementOverrides(e.Id, new OverrideGraphicSettings());
                    t.Commit();
                }
            }

            TaskDialog.Show(ttl, msg);
            return Result.Succeeded;
        }
    }
}
