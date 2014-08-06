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
    class FlagUnitElementsCommand : IExternalCommand
    {
        Regex unitGroupRegex = new Regex(@"UNIT [A-Z0-9][A-Z0-9]");

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            ElementMulticategoryFilter categoriesToFlag = new ElementMulticategoryFilter(
                new List<ElementId>
                    {
                        dbDoc.Settings.Categories.get_Item(BuiltInCategory.OST_Doors).Id,
                        dbDoc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls).Id
                    });

            IEnumerable<Element> elementsToFlag = new FilteredElementCollector(dbDoc).WherePasses(categoriesToFlag);

            using(Transaction t = new Transaction(dbDoc, "Flag unit doors"))
        	{
                t.Start();

		        foreach (Element e in elementsToFlag)
                {
                    ElementId elemGroupId = e.GroupId;

                    if (ElementId.InvalidElementId != elemGroupId)
	                {
                        if (unitGroupRegex.IsMatch(dbDoc.GetElement(elemGroupId).Name))
	                    {
		                    Parameter p = e.get_Parameter("UNIT TYPE");
                            if (null != p)
                            {
                                if(String.IsNullOrWhiteSpace(p.AsString()) && !p.IsReadOnly)
	                            {
		                            p.Set("UNIT");
	                            }
                            }
	                    }
	                }
                }

                t.Commit();
        	}

            return Result.Succeeded;
        }
    }
}
