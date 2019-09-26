using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class UnflipCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIDocument uiDoc = commandData.Application.ActiveUIDocument;
			Document dbDoc = commandData.Application.ActiveUIDocument.Document;

			ICollection<ElementId> sel = uiDoc.Selection.GetElementIds();

			Int16 counter = 0;

			using (Transaction t = new Transaction(dbDoc, "Unflip Elements"))
			{
				t.Start();
				foreach (var id in sel)
				{
					FamilyInstance inst = dbDoc.GetElement(id) as FamilyInstance;

                    //skip if the element isn't a family instance (ie a built in family)
                    if (null == inst) continue;

					//Some FamilyInstances are flipped even though CanFlipHand is false. CanFlip hand can't be ignored to modify these instances
					//In this case the attempt to flip will silently fail
					if (inst.CanFlipHand)
					{
						//Facing should be maintained, but this requires that the hand also be flipped for the FamilyInstance.
						//A window hosted in a flipped wall must have it's facing flipped to locate correctly.
						//However, this means that a window with unflipped hand will look like a flipped window.
						//Keeping FacingFlipped and HandFlipped synced ensures that the window is oriented correctly.
						if ((inst.HandFlipped && !inst.FacingFlipped) || (!inst.HandFlipped && inst.FacingFlipped))
						{
							inst.flipHand();
							counter++;
						}
					}
				}
				t.Commit();

				TaskDialog.Show("Unflip Command",
						counter + " elements flipped\nElements in groups and elements without a flipped handle will not be flipped");
			}

			return Result.Succeeded;
		}
	}

	class UnflipCommandAvailability : IExternalCommandAvailability
	{
		public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
		{
			UIDocument uiDoc = applicationData.ActiveUIDocument;

			if (null != uiDoc)
			{
				if (0 != uiDoc.Selection.GetElementIds().Count)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}

}
