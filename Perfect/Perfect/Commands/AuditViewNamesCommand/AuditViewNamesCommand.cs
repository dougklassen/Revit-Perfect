using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class AuditViewNamesCommand : IExternalCommand
    {
        IEnumerable<Viewport> projectViewports; //all viewports in the project, representing all placed views
        Document dbDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            dbDoc = commandData.Application.ActiveUIDocument.Document;

            projectViewports = new FilteredElementCollector(dbDoc).OfCategory(BuiltInCategory.OST_Viewports).AsEnumerable().Cast<Viewport>();
            IEnumerable<ViewPlan> planViews = new FilteredElementCollector(dbDoc).OfClass(typeof(ViewPlan)).AsEnumerable().Cast<ViewPlan>();
            IEnumerable<ViewSection> sectionViews = new FilteredElementCollector(dbDoc).OfClass(typeof(ViewSection)).AsEnumerable().Cast<ViewSection>();
            IEnumerable<View3D> threeDViews = new FilteredElementCollector(dbDoc).OfClass(typeof(View3D)).AsEnumerable().Cast<View3D>();
            IEnumerable<ViewDrafting> draftingViews = new FilteredElementCollector(dbDoc).OfClass(typeof(ViewDrafting)).AsEnumerable().Cast<ViewDrafting>();

            String msg = String.Empty;
            msg += projectViewports.Count() + " viewports found\n";
            msg += planViews.Count() + " plan views found\n\n";

            Regex splitRegex = new Regex("_");
            List<String> oldName, newName;

            using (Transaction t = new Transaction(dbDoc, "Standardize View Names"))
            {
                t.Start();

                foreach (ViewPlan plan in planViews)
                {
                    oldName = splitRegex.Split(plan.Name).ToList();
                    newName = new List<String>();

                    if (HasDefaultName(plan))
                    {
                        newName = GetStandardName(plan);
                    }

                    msg += oldName.Count() + " Segments: ";
                    String segs = String.Empty;
                    foreach (String s in oldName)
                    {
                        segs += s + " - ";
                    }
                    msg += segs;

                    msg += GetTypeSeg(plan) + '\n';

                    String nameUpdate = String.Empty;
                    for (int i = 0; i < (newName.Count() - 1); i++)
                    {
                        nameUpdate += newName[i] + '_';
                    }
                    nameUpdate += newName.Last();

                    plan.Name = nameUpdate;
                }

                t.Commit();
            }

            TaskDialog.Show("View names", msg);
            return Result.Succeeded;
        }

        String GetTypeSeg(View v)
        {
            Viewport viewport = projectViewports.Where(vp => v.Id == vp.ViewId).FirstOrDefault();

            if (null != viewport)
	        {
                String sheetNumber = viewport.get_Parameter("Sheet Number").AsString();
                String detailNumber = viewport.get_Parameter("Detail Number").AsString();
                return String.Format("{0}-{1}", sheetNumber, detailNumber);
	        }
            else
            {
                return "WK";
            }
        }

        List<String> GetStandardName(View v)
        {
            List<String> name = new List<String>();
            name.Add(GetTypeSeg(v));

            return name;
        }

        Boolean HasDefaultName(View v)
        {
            Regex elevationNameRegex = new Regex(@"Elevation \d - \s");
            if (elevationNameRegex.IsMatch(v.Name))
            {
                return true;
            }

            return false;
        }
    }
}
