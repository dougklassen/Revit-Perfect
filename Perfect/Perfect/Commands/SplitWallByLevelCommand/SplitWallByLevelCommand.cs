using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SplitWallByLevelCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var dbDoc = uiDoc.Document;

            //cancel the command if the selection isn't of a single wall
            var selection = uiDoc.Selection.GetElementIds();
            if (1 != selection.Count() || !(dbDoc.GetElement(selection.First()) is Wall))
            {
                TaskDialog.Show("Select Wall", "Select a single wall to be split");
                return Result.Failed;
            }

            Wall wall = dbDoc.GetElement(selection.First()) as Wall; //the wall to be split
            Double bottomElevation; //the bottom elevation of the wall
            Double topElevation; //the top elevation of the wall
            Double tolerance = 0.0001; //tolerance to use when choosing host levels

            //get the elevation at the bottom of the wall
            Level bottomLevel = dbDoc.GetElement(wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId()) as Level;
            Double bottomOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
            bottomElevation = bottomLevel.Elevation + bottomOffset;

            //get the elevation at the top of the wall
            //get the top level constraint of the wall. It will be null if the wall is unconnected
            Level topLevel = null;
            var wallTopLevelID = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
            if(null != wallTopLevelID) //if the wall is constrained to a top level
            {
                topLevel = dbDoc.GetElement(wallTopLevelID) as Level;
                Double topOffset = wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();
                topElevation = topLevel.Elevation + topOffset;
            }
            else //if the wall is unconnected, the topElevation is the bottomElevation + the unconnected height
            {
                Double unconnectedHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                topElevation = bottomElevation + unconnectedHeight;
            }

            //get all levels in the document, sorted by elevation.
            //Some of the elements of the category OST_Levels aren't of class Level so they're filtered out.
            //TODO: use .OfClass(Level)?
            var levels = new FilteredElementCollector(dbDoc)
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .Select(e => dbDoc.GetElement(e.Id) as Level)
                    .Where(l => null != l)
                    .OrderBy(l => l.Elevation);

            //generate a list of walls between the top and bottom of the wall
            List<Level> levelsBetween = new List<Level>();
            foreach (var level in levels)
            {
                if (level.Elevation >= (bottomElevation - tolerance) &&
                    level.Elevation <= (topElevation + tolerance))
                {
                    levelsBetween.Add(level);
                }
            }

            //check if the lowest section of the wall can be hosted on the level below, which is preferred
            Boolean useLevelBelow = true;
            if (levels.First().Elevation > bottomElevation)
            {
                useLevelBelow = false;
            }

            using (Transaction t = new Transaction(dbDoc, "Split wall by level"))
            {
                t.Start();

                String msg = String.Empty;
                msg += String.Format("Bottom: {0}\nTop: {1}\n\n", bottomElevation, topElevation);
                foreach (var l in levelsBetween)
                {
                    msg += String.Format("{0}: {1}\n", l.Name, l.Elevation);
                }
                TaskDialog.Show("Wall Split by Level", msg);

                t.Commit();
            }

            //TODO: let user select which levels will be used for splitting

            return Result.Succeeded;
        }
    }
}
