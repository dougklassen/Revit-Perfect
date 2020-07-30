using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class FilterCalloutsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //TODO: filter bugs according to selection criteria

            List<View> views = new List<View>();

            FilterCalloutsWindow window = new FilterCalloutsWindow(views);

            Boolean result = (Boolean) window.ShowDialog();

            if (result)
            {
                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }
        }
    }
}
