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

        Regex splitRegex = new Regex("_");
        Regex numberedDetailRegex = new Regex(@"^[A-Z]{1,2}[1]?\d.\d\d[a-z]?-\d?\d"); //valid format for sheet/detail number on placed views
        Regex seg0UnPlacedViewRegex = new Regex(@"COORD|DIM|EXPORT|PARENT|PRES|WK"); //valid seg 0 values for unplaced views
        Regex seg1ViewPlanRegex = new Regex(@"EFP|EQP|FP|RP|SP(\(\w+\))?"); //valid seg 1 values for plans
        Regex seg1AreaPlanRegex = new Regex(@"AP(\(\w+\))?");
        Regex seg1rcPlanRegex = new Regex(@"RCP(\(\w+\))?");
        Regex seg1SectionRegex = new Regex(@"BS|WS(\(\w+\))?");
        Regex seg1ElevationRegex = new Regex(@"EV|IE(\(\w+\))?");
        Regex seg2LevelRegex = new Regex(@"[A-Z]?\d{1,2}");
        Regex seg2ElevationRegex = new Regex(@"N(orth)?|E(ast)?|S(outh)?|W(est)?");
        Regex seg2SectionRegex = new Regex(@"NS|SN|EW|WE");
        Regex levelNumberRegex = new Regex(@"(\S* )?([A-B]?\d{1,3})$", RegexOptions.IgnoreCase);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            dbDoc = commandData.Application.ActiveUIDocument.Document;
            projectViewports = new FilteredElementCollector(dbDoc)
                .OfCategory(BuiltInCategory.OST_Viewports)
                .AsEnumerable()
                .Cast<Viewport>();
            IEnumerable<ViewPlan> docViewPlans = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewPlan))
                .AsEnumerable()
                .Cast<ViewPlan>()
                .Where(v => !v.IsTemplate);
            IEnumerable<ViewSection> docViewSections = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewSection))
                .AsEnumerable()
                .Cast<ViewSection>()
                .Where(v => !v.IsTemplate);
            IEnumerable<View3D> docView3Ds = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(View3D))
                .AsEnumerable()
                .Cast<View3D>()
                .Where(v => !v.IsTemplate);
            IEnumerable<ViewDrafting> docViewDraftings = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewDrafting))
                .AsEnumerable()
                .Cast<ViewDrafting>()
                .Where(v => !v.IsTemplate);

            String cmdResultMsg = String.Empty;
            List<String> oldName, newName;

            using (Transaction t = new Transaction(dbDoc, "Standardize View Names"))
            {
                t.Start();

                #region Evaluation of ViewPlans
                foreach (ViewPlan docViewPlan in docViewPlans)
                {
                    oldName = splitRegex.Split(docViewPlan.Name).ToList();
                    newName = new List<String>();

                    if (oldName.Count() < 3 || oldName.Count() > 4) //plan names must have three segments
                    {
                        nonConformingViews.Add(docViewPlan);
                        continue;
                    }

                    #region Evaluation of Segment 0
                    String newSeg0 = GetSeg0(docViewPlan, oldName[0]);
                    if (null != newSeg0)
                    {
                        newName.Add(newSeg0);
                    }
                    else
                    {
                        nonConformingViews.Add(docViewPlan);
                        continue;
                    }
                    #endregion Evaluation of Segment 0

                    #region Evaluation of Segment 1
                    if (ViewType.FloorPlan == docViewPlan.ViewType)
                    {
                        if (seg1ViewPlanRegex.IsMatch(oldName[1])) //if conforms to a proper view type designation
                        {
                            newName.Add(oldName[1]);
                        }
                        else
                        {
                            nonConformingViews.Add(docViewPlan);
                            continue;
                        }
                    }
                    else if (ViewType.CeilingPlan == docViewPlan.ViewType)
                    {
                        if (seg1rcPlanRegex.IsMatch(oldName[1]))
                        {
                            newName.Add(oldName[1]);
                        }
                        else
                        {
                            nonConformingViews.Add(docViewPlan);
                            continue;
                        }
                    }
                    else if (ViewType.AreaPlan == docViewPlan.ViewType)
                    {
                        if (seg1AreaPlanRegex.IsMatch(oldName[1]))
                        {
                            newName.Add(oldName[1]);
                        }
                        else
                        {
                            nonConformingViews.Add(docViewPlan);
                            continue;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(docViewPlan.Name += " is of an unrecognized ViewType");
                    }
                    #endregion Evaluation of Segment 1

                    #region Evaluation of segment 2
                    if (seg2LevelRegex.IsMatch(oldName[2]))
                    {
                        newName.Add(oldName[2]);
                    }
                    else
                    {
                        nonConformingViews.Add(docViewPlan);
                        continue;
                    }
                    #endregion Evaluation of Segment 2

                    #region Evaluation of Segment 3
                    if (oldName.Count() > 3)
                    {
                        newName.Add(oldName[3]);
                    }
                    #endregion Evaluation of Segment 3

                    String nameUpdate = String.Empty;
                    for (int i = 0; i < (newName.Count() - 1); i++)
                    {
                        nameUpdate += newName[i] + '_';
                    }
                    nameUpdate += newName.Last();

                    if (docViewPlan.Name != nameUpdate)
                    {
                        cmdResultMsg += docViewPlan.Name + " => " + nameUpdate + "\n"; 
                    }

                    //docViewPlan.Name = nameUpdate;
                }
                #endregion Evaluation of ViewPlans

                #region Evaluation of ViewSections
                foreach (ViewSection docViewSection in docViewSections)
                {
                    oldName = splitRegex.Split(docViewSection.Name).ToList();
                    newName = new List<String>();

                    if(oldName.Count() < 3 || oldName.Count() > 4) //Elevation and Section views must have three or four segments
                    {
                        nonConformingViews.Add(docViewSection);
                        continue;
                    }

                    #region Evaluation of Segment 0
                    String newSeg0 = GetSeg0(docViewSection, oldName[0]);
                    if (null != newSeg0)
                    {
                        newName.Add(newSeg0);
                    }
                    else
                    {
                        nonConformingViews.Add(docViewSection);
                        continue;
                    }
                    #endregion Evaluation of Segment 0

                    #region Evaluation of Segment 1
                    if (ViewType.Elevation == docViewSection.ViewType &&  seg1ElevationRegex.IsMatch(oldName[1]))
                    {
                        newName.Add(oldName[1]);
                    }
                    else if (ViewType.Section == docViewSection.ViewType && seg1SectionRegex.IsMatch(oldName[1]))
                    {
                        newName.Add(oldName[1]);
                    }
                    else if (ViewType.Section != docViewSection.ViewType && ViewType.Elevation != docViewSection.ViewType)
                    {
                        throw new InvalidOperationException("ViewType of " + docViewSection.Name + " not recognized");
                    }
                    else
                    {
                        nonConformingViews.Add(docViewSection);
                        continue;
                    }
                    #endregion Evaluation of Segment 1

                    #region Evaluation of Segment 2
                    if ((ViewType.Elevation == docViewSection.ViewType && seg2ElevationRegex.IsMatch(oldName[2])) ||
                        (ViewType.Section == docViewSection.ViewType && seg2SectionRegex.IsMatch(oldName[2])))
                    {
                        newName.Add(oldName[2]);
                    }
                    else
                    {
                        nonConformingViews.Add(docViewSection);
                        continue;
                    }
                    #endregion Evaluation of Segment 2

                    #region Evaluation of Segment 3
                    if (oldName.Count() > 3)
                    {
                        newName.Add(oldName[3]);
                    }
                    #endregion Evaluation of Segement 3

                    String nameUpdate = String.Empty;
                    for (int i = 0; i < (newName.Count() - 1); i++)
                    {
                        nameUpdate += newName[i] + '_';
                    }
                    nameUpdate += newName.Last();

                    if (nameUpdate != docViewSection.Name)
                    {
                        cmdResultMsg += docViewSection.Name + " => " + nameUpdate + "\n"; 
                    }

                    //docViewSection.Name = nameUpdate;
                }
                #endregion Evaluation of ViewSections

                cmdResultMsg += "\nNon-Conforming:\n";
                foreach (View v in nonConformingViews)
                {
                    cmdResultMsg += v.Name + '\n';
                }

                t.Commit();
            }

            TaskDialog.Show("View names", cmdResultMsg);
            return Result.Succeeded;
        }

        /// <summary>
        /// Returns Segment 0 for a view based on whether it is placed and whether its name indicates it should be placed
        /// </summary>
        /// <param name="view">A view in the document</param>
        /// <param name="oldName">The current name of the view</param>
        /// <returns>The new name of the view, which may be the same as the old name, or null if the view name doesn't match project standards</returns>
        String GetSeg0(View view, String oldName)
        {
            if (IsPlaced(view))
            {
                if (numberedDetailRegex.IsMatch(oldName) || "DOC" == oldName)
                {
                    return GetPlacedViewPrefix(view);
                }
                else if ("EXPORT" == oldName || "PRES" == oldName) //EXPORT and PRES are valid prefixes for a placed view
                {
                    return oldName;
                }
                else
                {
                    return null;
                }
            }
            else if (numberedDetailRegex.IsMatch(oldName))
            {
                return "DOC"; //rename unplaced but numbered DOC view to DOC
            }
            else if (seg0UnPlacedViewRegex.IsMatch(oldName))
            {
                return oldName;
            }
            else
            {
                return null;
            }
        }

        String GetPlacedViewPrefix(View v)
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
                throw new InvalidOperationException(v.Name + " is not placed on a sheet");
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
        
        //************* View Name utility methods
        List<String> GetStandardName(View v)
        {
            List<String> name = new List<String>();
            name.Add(GetPlacedViewPrefix(v));

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
