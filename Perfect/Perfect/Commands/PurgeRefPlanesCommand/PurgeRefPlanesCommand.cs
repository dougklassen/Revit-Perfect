using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class PurgeRefPlanesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<ReferencePlane> refPlanes = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ReferencePlane))
                .Where(p => String.IsNullOrWhiteSpace(p.Name) || "Reference Plane" == p.Name)
                .Cast<ReferencePlane>()
                //projecting onto a new list is necessary because deleting from the original FilteredElement during iteration will throw an exception
                .ToList();

            List<ElementId> refPlaneIds = refPlanes.Select(e => e.Id).ToList();

            Int16 refPlanesDeletedCtr = 0,
                elementsDeletedCtr = 0,
                groupedRefPlanesCtr = 0,
                undeletableRefPlanesCtr = 0;


            using (Transaction t = new Transaction(dbDoc, "Purge Reference Planes"))
            {
                t.Start();

                foreach (ReferencePlane rp in refPlanes)
                {
                    if (ElementId.InvalidElementId == rp.GroupId)
                    {
                        try
                        {
                            dbDoc.Delete(rp);
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
                //foreach (ElementId id in refPlaneIds)
                //{
                //    if (ElementId.InvalidElementId == dbDoc.GetElement(id).GroupId)
                //    {
                //        try
                //        {
                //            dbDoc.Delete(id);
                //            refPlanesDeletedCtr++;
                //        }
                //        catch (Exception)
                //        {
                //            undeletableRefPlanesCtr++;
                //            continue;
                //        }
                //    }
                //    else
                //    {
                //        groupedRefPlanesCtr++;
                //    }
                //}

                t.Commit();
            }

            TaskDialog.Show("Result",
                refPlaneIds.Count() + " unnamed reference planes found\n"
                + groupedRefPlanesCtr + " grouped reference planes not deleted\n"
                + refPlanesDeletedCtr + " reference planes deleted\n");

            return Result.Succeeded;
        }
    }
}
