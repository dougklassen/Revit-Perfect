using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Perfect.Interface;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Automatically hide irrelevant view callouts. For all views placed on a sheet,
    /// hide all view callouts to views placed on other sheets with non-matching sheet numbers.
    /// Sheet number matching is based on a specified number of digits taken from the sheet
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class ViewCalloutVisibilityCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = uiDoc.Document;

            /* differentiate between sheet and other views. If a schedule is active, the command
             * will be cancelled. If a sheet is active, all views on the sheet will be processed.
             * If some other view is active, only the view itself will be processed */
            if(uiDoc.ActiveView is ViewSchedule)
            {
                TaskDialog.Show("Error", "Callout visibility does not apply to schedules");
                return Result.Failed;
            }
            else if (uiDoc.ActiveView is ViewSheet)
            {
                TaskDialog.Show("msg", "it's a sheet");
            }
            else
            {
                TaskDialog.Show("msg", "it's a view");
            }

            ViewCalloutVisibilityWindow window = new ViewCalloutVisibilityWindow();

            Boolean result = (Boolean)window.ShowDialog();

            if(result)
            {
                return Result.Succeeded;
            }
            else
            {
                return Result.Failed;
            }
        }
    }
}
