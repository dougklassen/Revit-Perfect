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
		private String cmdResultMsg = String.Empty;
		IEnumerable<Viewport> projectViewports; //all viewports in the project, representing all placed views
		IEnumerable<String> projectViewNames; //all the view names in the project
		List<View> nonConformingViews = new List<View>(); //all views with a name not matching standards
		Document dbDoc;

		Regex splitRegex = new Regex("_");
		//todo: this is passing numbering of the form A2.91_DOC
		Regex numberedDetailRegex = new Regex(@"^([A-Z][A-Z-][\d]{1,3}(-[A-Z]{1,3})?-\w{1,4})|([A-Z]{1,3}[\d]{1,2}(.[\d]{1,2})?[A-Za-z]?-\w{1,4})$");
			//valid format for sheet/detail number on placed views, old BCRA standard or NCS compatible
		Regex seg0UnPlacedViewRegex = new Regex(@"^(COORD|DIM|DOC|EXPORT|PARENT|PRES|WK)$");    //valid seg 0 values for unplaced views
		Regex seg1ViewPlanRegex = new Regex(@"^(EFP|EQP|FP|RP|SP)(\(\w+\))?$");     //valid seg 1 values for plans
		Regex seg1AreaPlanRegex = new Regex(@"^AP(\(\w+\))?$");
		Regex seg1rcPlanRegex = new Regex(@"^RCP(\(\w+\))?$");
		Regex seg1SectionRegex = new Regex(@"^(BS|WS)(\(\w+\))?$");
		Regex seg1ElevationRegex = new Regex(@"^(EV|IE)(\(\w+\))?$");
		Regex seg1ThreeDRegex = new Regex(@"^(3D|PV)(\(\w+\))?$");
		Regex seg2LevelRegex = new Regex(@"^[A-Z]?\d{1,2}(.\d{1,2})?$");	//valid seg 2 values denoting a level
		Regex seg2ElevationRegex = new Regex(@"^(N(orth)?|E(ast)?|S(outh)?|W(est)?)$");	//valid seg 2 values denoting an elevation direction
		Regex seg2SectionRegex = new Regex(@"^(NS|SN|EW|WE)$");
		Regex default3DRegex = new Regex(@"^{3D( - [\w ]{2,20})?}$");
		Regex levelNumberRegex = new Regex(@"^(\S* )?([A-B]?\d{1,3})$", RegexOptions.IgnoreCase);

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			dbDoc = commandData.Application.ActiveUIDocument.Document;
			projectViewNames = new FilteredElementCollector(dbDoc)
				.OfCategory(BuiltInCategory.OST_Views)
				.AsEnumerable()
				.Select(v => v.Name);
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

			cmdResultMsg = String.Empty;
			List<String> oldName, newName;

			using (Transaction t = new Transaction(dbDoc, "Standardize View Names"))
			{
				t.Start();

				#region Evaluation of ViewPlans
				foreach (ViewPlan docViewPlan in docViewPlans)
				{
					oldName = splitRegex.Split(docViewPlan.Name).ToList();
					newName = new List<String>();

					if (oldName.Count() < 2 || oldName.Count() > 4) //plan names must have two to four segments
					{
						nonConformingViews.Add(docViewPlan);
						continue;
					}

					#region Evaluation of Segment 0
					String newSeg0 = GetSeg0(docViewPlan, oldName[0]);
					if (newSeg0 != null)
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
					//ViewPlans are permitted to have only two segments where they are large scale details
					if (oldName.Count == 2)
					{
						if (
							((ViewType.FloorPlan == docViewPlan.ViewType) ||
								(ViewType.CeilingPlan == docViewPlan.ViewType) ||
								(ViewType.Detail == docViewPlan.ViewType)) &&
							docViewPlan.get_Parameter(BuiltInParameter.VIEW_SCALE).AsDouble() <= 12)
						{
							newName.Add(oldName[1]);
							TryRenameView(docViewPlan, newName);
							continue;
						}
						else
						{
							nonConformingViews.Add(docViewPlan);
							continue;
						}
					}
					else if (ViewType.FloorPlan == docViewPlan.ViewType)
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
					else if (ViewType.Detail != docViewPlan.ViewType) //details are only permitted at larger scales as checked above
					{
						throw new InvalidOperationException(docViewPlan.Name += " is of an unrecognized ViewType");
					}
					else
					{
						nonConformingViews.Add(docViewPlan);
					}
					#endregion Evaluation of Segment 1

					#region Evaluation of segment 2
					//todo: add level verification
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

					TryRenameView(docViewPlan, newName);
				}
				#endregion Evaluation of ViewPlans

				#region Evaluation of ViewSections
				foreach (ViewSection docViewSection in docViewSections)
				{
					oldName = splitRegex.Split(docViewSection.Name).ToList();
					newName = new List<String>();
					if (oldName.Count() < 2 || oldName.Count() > 4) //Section and elevation views must have between two and four segments
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
					if (2 == oldName.Count)
					{
						if (((ViewType.Section == docViewSection.ViewType) || (ViewType.Detail == docViewSection.ViewType))
							&& docViewSection.get_Parameter(BuiltInParameter.VIEW_SCALE).AsDouble() <= 12)
						{
							newName.Add(oldName[1]);
							TryRenameView(docViewSection, newName);
							continue;
						}
						else
						{
							nonConformingViews.Add(docViewSection);
							continue;
						}
					}
					else if (ViewType.Elevation == docViewSection.ViewType && seg1ElevationRegex.IsMatch(oldName[1]))
					{
						newName.Add(oldName[1]);
					}
					else if (ViewType.Section == docViewSection.ViewType && seg1SectionRegex.IsMatch(oldName[1]))
					{
						newName.Add(oldName[1]);
					}
					else if (
						ViewType.Section != docViewSection.ViewType &&
						ViewType.Elevation != docViewSection.ViewType &&
						ViewType.Detail != docViewSection.ViewType)
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
					else if ((ViewType.Elevation == docViewSection.ViewType) &&
						("IE" == oldName[1])) //interior elevations don't need orientation specified in seg 2
					{
						newName.Add(oldName[2]);
					}
					else if ((ViewType.Section == docViewSection.ViewType) &&
						("WS" == oldName[1]))	//wall types don't need orientation specified in seg 2
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

					TryRenameView(docViewSection, newName);
				}
				#endregion Evaluation of ViewSections

				#region Evaluation of ViewDraftings
				foreach (ViewDrafting docViewDrafting in docViewDraftings)
				{
					oldName = splitRegex.Split(docViewDrafting.Name).ToList();
					newName = new List<string>();

					if (oldName.Count() != 2) //drafting view names may only have 2 segments
					{
						nonConformingViews.Add(docViewDrafting);
						continue;
					}

					#region Evaluate Segment 0
					String newSeg0 = GetSeg0(docViewDrafting, oldName[0]);
					if (null != newSeg0)
					{
						newName.Add(newSeg0);
					}
					else
					{
						nonConformingViews.Add(docViewDrafting);
						continue;
					}
					#endregion Evaluate Segment 0

					#region Audit Segment 1
					newName.Add(oldName[1]);
					#endregion Audit Segment 1

					TryRenameView(docViewDrafting, newName);
				}
				#endregion Evaluation of ViewDraftings

				#region Evaluation of View3Ds
				foreach (View3D docView3D in docView3Ds)
				{
					oldName = splitRegex.Split(docView3D.Name).ToList();
					newName = new List<String>();

					if (default3DRegex.IsMatch(docView3D.Name)) //ignore View3Ds with the default name
					{
						continue;
					}

					if (oldName.Count() < 2 || oldName.Count() > 4)
					{
						nonConformingViews.Add(docView3D);
						continue;
					}

					#region Audit Segment 0
					String newSeg0 = GetSeg0(docView3D, oldName[0]);
					if (null != newSeg0)
					{
						newName.Add(oldName[0]);
					}
					else
					{
						nonConformingViews.Add(docView3D);
						continue;
					}
					#endregion Audit Segment 0

					#region Audit Segment 1
					if (seg1ThreeDRegex.IsMatch(oldName[1]))
					{
						newName.Add(oldName[1]);
					}
					else
					{
						nonConformingViews.Add(docView3D);
						continue;
					}
					#endregion Audit Segment 1

					#region Audit Segment 2
					if (oldName.Count() > 2)
					{
						newName.Add(oldName[2]);
					}
					#endregion Audit Segment 2

					#region Audit Segment 3
					if (4 == oldName.Count())
					{
						newName.Add(oldName[3]);
					}
					#endregion Audit Segment 3

					TryRenameView(docView3D, newName);
				}
				#endregion Evaluation of View3Ds

				if (String.IsNullOrEmpty(cmdResultMsg) && 0 == nonConformingViews.Count())
				{
					cmdResultMsg += "Perfect!";
				}
				else if (nonConformingViews.Count != 0)
				{
					cmdResultMsg += "\nNon-Conforming:\n";
					foreach (View v in nonConformingViews)
					{
						cmdResultMsg += v.Name + '\n';
					}
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
		/// <param name="oldSeg0">The current name of the view</param>
		/// <returns>The new name of the view, which may be the same as the old name,
		/// or null if the view name doesn't match project standards</returns>
		private String GetSeg0(View view, String oldSeg0)
		{
			if (IsPlaced(view))
			{
				if (numberedDetailRegex.IsMatch(oldSeg0) || "DOC" == oldSeg0)
				{
					return GetPlacedViewPrefix(view);
				}
				else if ("EXPORT" == oldSeg0 || "PRES" == oldSeg0) //EXPORT and PRES are valid prefixes for a placed view
				{
					return oldSeg0;
				}
				else
				{
					return null;
				}
			}
			else if (numberedDetailRegex.IsMatch(oldSeg0))
			{
				return "DOC"; //rename unplaced but numbered DOC view to DOC
			}
			else if (seg0UnPlacedViewRegex.IsMatch(oldSeg0))
			{
				return oldSeg0;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Generate a string of the format SHEET NUMBER-VIEW NUMBER for a specified view
		/// </summary>
		/// <param name="view">A View in the document</param>
		/// <returns>A string to be used as a prefix to the View Name</returns>
		private String GetPlacedViewPrefix(View view)
		{
			Viewport viewport = projectViewports.Where(vp => view.Id == vp.ViewId).FirstOrDefault();

			if (null != viewport)
			{
				String sheetNumber = viewport.LookupParameter("Sheet Number").AsString();
				String detailNumber = viewport.LookupParameter("Detail Number").AsString();
				return String.Format("{0}-{1}", sheetNumber, detailNumber);
			}
			else
			{
				throw new InvalidOperationException(view.Name + " is not placed on a sheet");
			}
		}

		/// <summary>
		/// Determine whether a View is placed onto a sheet
		/// </summary>
		/// <param name="view">A View in the document</param>
		/// <returns>Whether the View is placed</returns>
		private Boolean IsPlaced(View view)
		{
			if (projectViewports.Where(vp => view.Id == vp.ViewId).Count() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Generates a new List representing the components of the new View name
		/// </summary>
		/// <param name="view">A View in the document</param>
		/// <returns>A List to represent the components of a new View name, with the first segment entered</returns>
		private List<String> GetStandardName(View view)
		{
			List<String> nameComponents = new List<String>();
			nameComponents.Add(GetPlacedViewPrefix(view));

			return nameComponents;
		}

		/// <summary>
		/// Determine whether a view should be renamed, and if so, set new name. Can only be called within an active transaction.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="newName"></param>
		private void TryRenameView(View view, List<String> newName)
		{
			String nameUpdate = String.Empty;
			for (int i = 0; i < (newName.Count() - 1); i++)
			{
				nameUpdate += newName[i] + '_';
			}
			nameUpdate += newName.Last();

			//skip if the name is already correct or if there is a View name conflict
			if ((view.Name != nameUpdate) && !projectViewNames.Contains(nameUpdate))
			{
				cmdResultMsg += view.Name + " => " + nameUpdate + "\n";
				view.Name = nameUpdate;
			}
		}
	}
}
