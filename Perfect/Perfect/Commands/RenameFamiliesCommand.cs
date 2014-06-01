using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RenameFamiliesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            List<Element> elementsToRename = new List<Element>();
            IEnumerable<Family> familiesInDoc = new FilteredElementCollector(dbDoc).OfClass(typeof(Family)).AsEnumerable().Cast<Family>();

            //select Grid types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(GridType)).AsEnumerable());
            //select Level types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(LevelType)).AsEnumerable());
            //select Wall types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(WallType)).AsEnumerable());
            //select Floor Types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(FloorType)).AsEnumerable());
            //select Window families
            //TODO: if type name matches family name, rename type as well
            elementsToRename.AddRange(familiesInDoc
                .Where(f => "Windows" == f.FamilyCategory.Name)
                .AsEnumerable());
            //select Door families
            //TODO: if type name matches family name, rename type as well
            elementsToRename.AddRange(familiesInDoc
                .Where(f => "Doors" == f.FamilyCategory.Name)
                .AsEnumerable());

            String bcraPrefixPattern = @"^(?:b\.|BCRA_)*(.*)";
            String replacement = "b.$1";

            using (Transaction t = new Transaction(dbDoc, "Standardize Family Names"))
            {
                t.Start();

                foreach (Element e in elementsToRename)
                {
                    e.Name = Regex.Replace(e.Name, bcraPrefixPattern, replacement);
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
