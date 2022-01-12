using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Purge all unnamed reference planes from the project
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PurgeRefPlanesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<ElementId> refPlaneIds = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ReferencePlane))
                .Where(p => String.IsNullOrWhiteSpace(p.Name) || "Reference Plane" == p.Name)
                .Select(e => e.Id)
                //projecting onto a new list is necessary because deleting from the original FilteredElement during iteration will throw an exception
                .ToList();

            Int32 refPlanesDeletedCtr = 0,
                elementsDeletedCtr = 0,
                groupedRefPlanesCtr = 0,
                undeletableRefPlanesCtr = 0;


            using (Transaction t = new Transaction(dbDoc, "Purge Reference Planes"))
            {
                t.Start();

                foreach (ElementId id in refPlaneIds)
                {
                    if (ElementId.InvalidElementId == dbDoc.GetElement(id).GroupId)
                    {
                        try
                        {
                            elementsDeletedCtr += dbDoc.Delete(id).Count();
                            refPlanesDeletedCtr++;
                        }
                        catch (Exception)
                        {
                            undeletableRefPlanesCtr++;
                            continue;
                        }
                    }
                    else
                    {
                        groupedRefPlanesCtr++;
                    }
                }

                t.Commit();
            }

            TaskDialog.Show("Result",
                refPlaneIds.Count() + " unnamed reference planes found\n"
                + groupedRefPlanesCtr + " grouped reference planes not deleted\n"
                + refPlanesDeletedCtr + " reference planes deleted\n"
                + (elementsDeletedCtr - refPlanesDeletedCtr) + " other elements deleted");

            return Result.Succeeded;
        }
    }
}
