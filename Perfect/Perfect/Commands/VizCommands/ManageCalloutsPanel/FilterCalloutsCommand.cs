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
            String debug = String.Empty;
            //process each selected sheet
            foreach (ViewSheet sheet in sheets)
            {
                using (Transaction t = new Transaction(dbDoc))
                {
                    t.Start("Set callout visibilty for " + sheet.SheetNumber);
                    //find the sheet number substring to be matched
                    String filterString = String.Empty;
                    if (sheet.SheetNumber.Length >= charsToMatch)
                    {
                        filterString = sheet.SheetNumber.Substring(0, charsToMatch);
                    }
                    else
                    {
                        filterString = sheet.SheetNumber;
                    }

                    //process all views placed on the sheet
                    var placedViews = sheet.GetAllPlacedViews();
                    foreach (ElementId viewId in placedViews)
                    {
                        View view = dbDoc.GetElement(viewId) as View;
                        //temporarily enter reveal hidden mode so that hidden callouts are included
                        Boolean alreadyInRevealHidden = view.IsInTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);
                        if(!alreadyInRevealHidden)
                        {
                            view.EnableRevealHiddenMode();
                        }

#region process viewers 
                        List<ElementId> bugsToHide = new List<ElementId>();
                        List<ElementId> bugsToShow = new List<ElementId>();
                        FilteredElementCollector viewers = new FilteredElementCollector(dbDoc, viewId)
                            .OfCategory(BuiltInCategory.OST_Viewers);

                        foreach (Element viewer in viewers)
                        {
                            debug += "\n\nView " + view.Name;
                            debug += String.Format("\nBug sheet num: {0} Filter: {1}",
                                viewer.get_Parameter(BuiltInParameter.VIEWER_SHEET_NUMBER).AsString(),
                                filterString);
                            if ( viewer.get_Parameter(BuiltInParameter.VIEWER_SHEET_NUMBER).AsString().StartsWith(filterString))
                            {
                                if (viewer.IsHidden(view))
                                {
                                    bugsToShow.Add(viewer.Id);
                                    debug += "\nis hidden, will be shown";
                                }
                                else
                                {
                                    debug += "\nis already visible";
                                }
                            }
                            else if(!viewer.IsHidden(view))
                            {
                                bugsToHide.Add(viewer.Id);
                                debug += "\nis visible, will be hidden";
                            }
                            else
                            {
                                debug += "\nis already hidden";
                            }
                        }

                        debug += "\n\n" + bugsToShow.Count + " callouts revealed in " + view.Name;
                        debug += "\n" + bugsToHide.Count + " callouts hidden in " + view.Name;
                        if (bugsToShow.Count > 0)
                        {
                            view.UnhideElements(bugsToShow);
                        }
                        if (bugsToHide.Count > 0)
                        {
                            view.HideElements(bugsToHide);
                        }
#endregion process viewers 

#region process markers
                        //process elevation markers to hide all markers that have all viewers hidden
                        List<ElementId> markersToShow = new List<ElementId>();
                        List<ElementId> markersToHide = new List<ElementId>();
                        var markers = new FilteredElementCollector(dbDoc, viewId).OfCategory(BuiltInCategory.OST_Elev);
                        foreach (ElevationMarker marker in markers)
                        {
                            Boolean hasVisiblePointer = false;
                            for (int i = 0; i < marker.MaximumViewCount; i++)
                            {
                                ElementId indexedViewId = marker.GetViewId(i);
                                View indexedView = dbDoc.GetElement(indexedViewId) as View;
                                if (indexedView == null)
                                {
                                    debug += "\n???indexed view is null";
                                    continue;
                                }
                                //look for a visible viewer matching the view reference found
                                foreach (Element viewer in viewers)
                                {
                                    //check if there is a visible elevation viewer with a matching name 
                                    if (
                                        !viewer.IsHidden(view) &&
                                        viewer.get_Parameter(BuiltInParameter.VIEW_FAMILY).AsString().Equals("Elevations") &&
                                        viewer.get_Parameter(BuiltInParameter.VIEW_NAME).AsString().Equals(indexedView.Name))
                                    {
                                        hasVisiblePointer = true;
                                    }
                                }
                            }

                            if (hasVisiblePointer)
                            {
                                if (marker.IsHidden(view))
                                {
                                    markersToShow.Add(marker.Id);
                                }
                            }
                            else if (!marker.IsHidden(view))
                            {
                                markersToHide.Add(marker.Id);
                            }
                        }

                        if (markersToShow.Count > 0)
                        {
                            view.UnhideElements(markersToShow);
                        }
                        if (markersToHide.Count > 0)
                        {
                            view.HideElements(markersToHide);
                        }
#endregion process markers

                        //reset reveal hidden mode if necessary
                        if (!alreadyInRevealHidden)
                        {
                            view.DisableTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);
                        }
                    }
                    t.Commit();
                }
            }

            //TODO: debug
            debug += "\nSheets selected: " + sheets.Count;
            TaskDialog.Show("debug", debug);

            return Result.Succeeded;
        }
    }
}
