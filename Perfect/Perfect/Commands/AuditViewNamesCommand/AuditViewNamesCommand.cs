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
        List<View> nonConformingViews = new List<View>(); //all views with a name not matching standards
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

            Regex splitRegex = new Regex("_");
            Regex numberedDetailRegex = new Regex(@"^[A-Z]{1,2}[1]?\d.\d\d[a-z]?-\d?\d"); //valid format for sheet/detail number on placed views
            Regex seg0UnPlacedViewRegex = new Regex(@"(COORD)|(EXPORT)|(PARENT)|(PRES)|(PV)|(WK)"); //valid seg 0 values for unplaced views
            Regex seg1PlanViewRegex = new Regex(@"(EFP)|(EQP)|(FP)|(RP)|(SP)"); //valid seg 1 values for plans
            Regex seg2Regex = new Regex(@"[A-Z]?\d{1,2}");
            Regex levelNumberRegex = new Regex(@"(\S* )?([A-B]?\d{1,3})$", RegexOptions.IgnoreCase);
            List<String> oldName, newName;

            using (Transaction t = new Transaction(dbDoc, "Standardize View Names"))
            {
                t.Start();

                //evaluation of ViewPlans
                foreach (ViewPlan plan in planViews)
                {
                    oldName = splitRegex.Split(plan.Name).ToList();
                    newName = new List<String>();

                    //plan names must have three segments
                    if (oldName.Count() < 3 || oldName.Count() > 4)
                    {
                        nonConformingViews.Add(plan);
                        continue;
                    }

                    //evaluation of segment 0
                    if (IsPlaced(plan))
                    {
                        if (numberedDetailRegex.IsMatch(oldName[0]) || "DOC" == oldName[0])
                        {
                            newName.Add(GetSeg0(plan));
                        }
                        else if ("EXPORT" == oldName[0])
                        {
                            newName.Add("EXPORT");
                        }
                        else
                        {
                            nonConformingViews.Add(plan);
                            continue;
                        }
                    }
                    else if (numberedDetailRegex.IsMatch(oldName[0]))
                    {
                        newName.Add("DOC"); //rename unplaced view to DOC
                    }
                    else if (seg0UnPlacedViewRegex.IsMatch(oldName[0]))
                    {
                        newName.Add(oldName[0]);
                    }
                    else
                    {
                        nonConformingViews.Add(plan);
                        continue;
                    }

                    //evaluation of segment 1
                    if (ViewType.FloorPlan == plan.ViewType)
                    {
                        if (seg1PlanViewRegex.IsMatch(oldName[1])) //if conforms to a proper view type designation
                        {
                            newName.Add(oldName[1]);
                        }
                        else
                        {
                            nonConformingViews.Add(plan);
                            continue;
                        }
                    }
                    else if (ViewType.CeilingPlan == plan.ViewType)
                    {
                        if ("RCP" == oldName[1])
                        {
                            newName.Add(oldName[1]);
                        }
                        else
                        {
                            nonConformingViews.Add(plan);
                            continue;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(plan.Name += " is of an unrecognized ViewType");
                    }

                    //evaluation of segment 2
                    if (seg2Regex.IsMatch(oldName[2]))
                    {
                        newName.Add(oldName[2]);
                    }
                    else
                    {
                        nonConformingViews.Add(plan);
                        continue;
                    }

                    //evaluation of segment 3
                    if (oldName.Count() > 3)
                    {
                        newName.Add(oldName[3]);
                    }

                    String nameUpdate = String.Empty;
                    for (int i = 0; i < (newName.Count() - 1); i++)
                    {
                        nameUpdate += newName[i] + '_';
                    }
                    nameUpdate += newName.Last();

                    msg += plan.Name + " => " + nameUpdate + "\n";

                    //plan.Name = nameUpdate;
                }//end evaluation of ViewPlans

                msg += "\nNon-Conforming:\n";
                foreach (View v in nonConformingViews)
                {
                    msg += v.Name + '\n';
                }

                t.Commit();
            }

            TaskDialog.Show("View names", msg);
            return Result.Succeeded;
        }

        String GetSeg0(View v)
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

        Boolean IsPlaced(View v)
        {
            if(projectViewports.Where(vp => v.Id == vp.ViewId).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        List<String> GetStandardName(View v)
        {
            List<String> name = new List<String>();
            name.Add(GetSeg0(v));

            return name;
        }

        Boolean HasDefaultName(View v)
        {
            if (true)
            {
                
            }
            Regex elevationNameRegex = new Regex(@"Elevation \d - \s");
            if (elevationNameRegex.IsMatch(v.Name))
            {
                return true;
            }

            return false;
        }
    }
}
