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
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            var selectedElements = uiDoc.Selection.GetElementIds();

            List<ViewSheet> sheets = new List<ViewSheet>();

            foreach (ElementId id in selectedElements)
            {
                ViewSheet selectedSheet = dbDoc.GetElement(id) as ViewSheet;
                if (null != selectedSheet)
                {
                    sheets.Add(selectedSheet);
                }
            }

            //Check whether at least one sheet is selected. If not, add the current view if it is a sheet.
            if (sheets.Count == 0)
            {
                ViewSheet currentSheet = uiDoc.ActiveView as ViewSheet;

                if (null != currentSheet)
                {
                    sheets.Add(currentSheet);
                }
                else
                {
                    String msg = "You must select at least one sheet";
                    TaskDialog.Show("Select a sheet", msg);

                    return Result.Failed;
                }
            }

            //Show window
            FilterCalloutsWindow window = new FilterCalloutsWindow(sheets);
            Boolean result = (Boolean) window.ShowDialog();
            if (!result)
            {
                return Result.Cancelled;
            }
            


            Int32 charsToMatch = window.CharsToMatch;

            //TODO: debug
            List<ElementId> refs = new List<ElementId>();
            String debug = String.Empty;
            Int32 viewCount = 0;

            //process each selected sheet
            foreach (ViewSheet sheet in sheets)
            {
                //process all views placed on the sheet
                var placedViews = sheet.GetAllPlacedViews();
                //TODO: debug
                viewCount += placedViews.Count;

                foreach (ElementId viewId in placedViews)
                {
                    View view = dbDoc.GetElement(viewId) as View;
                    //find view callouts visible in this viewwo
                    List<ElementId> referenceIds = new List<ElementId>();
                    //TODO: this isn't working, no view references found
                    referenceIds.AddRange(view.GetReferenceCallouts());
                    referenceIds.AddRange(view.GetReferenceElevations());
                    referenceIds.AddRange(view.GetReferenceElevations());
                    
                    //TODO: debug
                    refs.AddRange(referenceIds);
                }
            }

            //TODO: debug
            debug += "\nSheets selected: " + sheets.Count;
            debug += "\nPlaced views found: " + viewCount;
            debug += "\nView callouts found: " + refs.Count;
            TaskDialog.Show("debug", debug);

            return Result.Succeeded;
        }
    }
}
