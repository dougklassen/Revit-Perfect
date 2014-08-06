using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Perfect.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DougKlassen.Revit.Perfect.Commands
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	public class PurgeViewsCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Document dbDoc = commandData.Application.ActiveUIDocument.Document;

			IEnumerable<View> docViews = new FilteredElementCollector(dbDoc)
			.OfClass(typeof(View))
			.AsEnumerable()
			.Cast<View>();

			PurgeElementsWindow purgeWindow = new PurgeElementsWindow(dbDoc, typeof(View));
			purgeWindow.PurgeRegExString 
				= @"(^Elevation ([1-9][0-9]|[1-9]) - [a-d]$)|(^Section ([1-9][0-9]|[1-9])$)|(^Drafting [1-9]?[0-9]$)|(^Callout of .*)|(^Copy of .*)";

			purgeWindow.ShowDialog();
			if (false == purgeWindow.DialogResult)
			{
				return Result.Cancelled;
			}

			ICollection<ElementId> viewsToDelete = new List<ElementId>();
			ElementId match;
			foreach (String viewName in purgeWindow.MatchingElementsListBox.Items)
			{
				match = docViews
					.Where(v => viewName == v.Name)
					.Select(v => v.Id)
					.FirstOrDefault();
				if (null != match)
				{
					viewsToDelete.Add(match);
				}
			}

			using (Transaction t = new Transaction(dbDoc, "Purge Views"))
			{
				t.Start();
				dbDoc.Delete(viewsToDelete);
				t.Commit();
			}

			//todo: take care of empty elevation hosts

			return Result.Succeeded;
		}
	}
}
