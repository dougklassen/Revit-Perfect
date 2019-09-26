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
            IEnumerable<Family> familiesInDoc = new FilteredElementCollector(dbDoc)
                .OfClass(typeof(Family))
                .AsEnumerable().Cast<Family>();

            /*
            //TODO: if type name matches family name, rename type as well
            elementsToRename.AddRange(familiesInDoc.Where(f => "Windows" == f.FamilyCategory.Name)); //select Window families
            //TODO: if type name matches family name, rename type as well
            elementsToRename.AddRange(familiesInDoc.Where(f => "Doors" == f.FamilyCategory.Name)); //select Door families
            //select Grid types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(GridType)).AsEnumerable());
            //select Level types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(LevelType)).AsEnumerable());
            */
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(WallType))); //select Wall types
            elementsToRename.AddRange(new FilteredElementCollector(dbDoc).OfClass(typeof(FloorType))); //select Floor Types

            Regex prefixRegex = new Regex(@"^(\d\d\.\d\d\d)_(.+)"); //the regex for the family name. Group 1 is the prefix, Group 2 is the body
            String prefixParameter = "TPhase"; //the parameter to pull to use as the family type prefix

            using (Transaction t = new Transaction(dbDoc, "Standardize Family Type Names"))
            {
                t.Start();

                foreach (Element e in elementsToRename)
                {
                    String prefix = null;
                    if (e.GetParameters(prefixParameter).Count() >= 1)
                    {
                        prefix = e.GetParameters(prefixParameter).First().AsString();
                    }
                    Match m = prefixRegex.Match(e.Name);
                    if (null != prefix)
                    {
                        if (m.Success)
                        {
                            e.Name = String.Format("{0}_{1}", prefix, m.Groups[2]);
                        }
                        else
                        {
                            e.Name = String.Format("{0}_{1}", prefix, e.Name);
                        }
                    }
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
