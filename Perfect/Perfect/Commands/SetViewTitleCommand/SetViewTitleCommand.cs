using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
	/// <summary>
	/// For all views in the project where the "Title on Sheet" parameter is empty, sets the parameter the name of the view. This prevents the view name from being changed when a user edits the view title.
	/// </summary>
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	class SetViewTitleCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Document dbDoc = commandData.Application.ActiveUIDocument.Document;

			IEnumerable<View> docViews = new FilteredElementCollector(dbDoc)
					.OfCategory(BuiltInCategory.OST_Views)
					.Cast<View>()
					.Where(v => !v.IsTemplate);

			Int32 setViewTitlesCounter = 0;

			using (Transaction t = new Transaction(dbDoc, "Set Title on Sheet value"))
			{
				t.Start();

				foreach (View v in docViews)
				{
					Parameter p = v.LookupParameter("Title on Sheet");
					if (null != p
							&& !p.IsReadOnly
							&& String.IsNullOrWhiteSpace(p.AsString()))
					{
						p.Set(v.ViewName);
						setViewTitlesCounter++;
					}
				}

				t.Commit();
			}

			TaskDialog.Show("Result", setViewTitlesCounter + " view titles set");

			return Result.Succeeded;
		}
	}
}
