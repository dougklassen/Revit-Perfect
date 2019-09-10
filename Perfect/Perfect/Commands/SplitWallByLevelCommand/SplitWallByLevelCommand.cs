using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Perfect.Interface;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SplitWallByLevelCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var dbDoc = uiDoc.Document;
            var create = dbDoc.Create;

            /* get all levels in the document, sorted by elevation.
            Some of the elements of the category OST_Levels aren't of class Level so they're filtered out. */
            // TODO: use .OfClass(Level)?
            var levels = new FilteredElementCollector(dbDoc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .Select(e => dbDoc.GetElement(e.Id) as Level)
                .Where(l => null != l)
                .OrderBy(l => l.Elevation);

            /* cancel the command if the selection isn't of a single wall */
            var selection = uiDoc.Selection.GetElementIds();
            if (1 != selection.Count() || !(dbDoc.GetElement(selection.First()) is Wall))
            {
                TaskDialog.Show("Select Wall", "Select a single wall to be split");
                return Result.Failed;
            }

            /* The parameters that will drive the creation of the new walls */
            Wall sourceWall = dbDoc.GetElement(selection.First()) as Wall; //the wall to be split
            Double overallBottomElevation; //the bottom elevation of the wall
            Double overallTopElevation; //the top elevation of the wall
            Level overallLevelAbove = null; //the level immediatly above the highest host level.
                //This will be the top constraint of the wall unless no level is found above the highest host level, in which case it will be set to null
            Double tolerance = 0.0001; //tolerance to use when choosing host levels
            List<Level> hostLevels = new List<Level>(); //The levels that will host the new individual walls
            String msg = String.Empty; //diagnostic string

            /* get the elevation at the bottom of the wall */
            overallBottomElevation =
                (dbDoc.GetElement(sourceWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId()) as Level).Elevation + //the elevation of the host level
                + sourceWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble(); //plus the bottom offset of the wall

            /* get the elevation at the top of the wall */
            Level topLevel = null; //get the top level constraint of the wall. It will be null if the wall is unconnected
            var wallTopLevelID = sourceWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
            if(null != wallTopLevelID) //if the wall is constrained to a top level
            {
                topLevel = dbDoc.GetElement(wallTopLevelID) as Level;
                Double topOffset = sourceWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();
                overallTopElevation = topLevel.Elevation + topOffset;
            }
            else //if the wall is unconnected, the topElevation is the bottomElevation + the unconnected height
            {
                Double unconnectedHeight = sourceWall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
                overallTopElevation = overallBottomElevation + unconnectedHeight;
            }

            /* find the host levels */
            /* find the bottom level */
            Level bottomLevel = levels.First(); //start with the lowest level in the project, even if it's higher than the bottom of the wall
            foreach (var level in levels) //work up through all levels lower than the bottom of the wall
            {
                if (level.Elevation <= overallBottomElevation + tolerance) //update the bottom level if another level is found below the bottom of the wall
                {
                    bottomLevel = level;
                }
                else //stop looking once the bottom level is found
                {
                    break;
                }
            }
            hostLevels.Add(bottomLevel); //set the first host level to the bottom level that was found
            /* find the remaining host levels */
            foreach (var level in levels)
            {
                if (level.Id == bottomLevel.Id) //exclude the bottom level if it has already been added
                {
                    continue;
                }
                else if (level.Elevation >= (bottomLevel.Elevation - tolerance) && //level must at or higher than the bottom level already established
                    level.Elevation <= (overallTopElevation + tolerance)) //level must be at or lower than the top of the wall
                {
                    hostLevels.Add(level);
                }
                else if (level.Elevation > (overallTopElevation + tolerance)) //if the level is above the top of the wall, stop adding more levels
                {
                    overallLevelAbove = level;
                    break;
                }
            }

            if ( //if the top host level found coincides with the top of the wall, remove it from the list of host levels. It will be a top constraint instead
                hostLevels.Last().Elevation >= (overallTopElevation - tolerance) &&
                hostLevels.Last().Elevation <= (overallTopElevation + tolerance))
            {
                overallLevelAbove = hostLevels.Last(); //overwrite the value of the levelAbove with this level, which exactly matches the top of the wall
                hostLevels.Remove(hostLevels.Last());
            }

            /* let user select which levels will be used for splitting. pre-select only levels designated as stories */
            SelectElementsWindow levelPickerWindow = new SelectElementsWindow(dbDoc, hostLevels.Cast<Element>().ToList());
            Boolean? result = levelPickerWindow.ShowDialog();

            using (Transaction t = new Transaction(dbDoc, "Split wall by level"))
            {
                t.Start();

                msg += String.Format("Wall Bottom: {0}\nWall Top: {1}\n", overallBottomElevation, overallTopElevation);
                msg += String.Format("Levels found: {0}\n\n", hostLevels.Count());
                /*iterate through the host levels*/
                for (int i = 0; i < hostLevels.Count(); i++)
                {
                    Level newHostLvl = hostLevels[i];
                    Level newTopLvl = null; //the top constraint, null indicates the top will be unconnected
                    Double newBottomOffset = 0;
                    Double newTopOffset = 0;
                    /* determine the top constraint of the wall */
                    if (i < (hostLevels.Count() - 1)) //if there is a host level above, use that as top constraint
                    {
                        newTopLvl = hostLevels[i + 1];
                    }
                    else if (null != overallLevelAbove) //if there is a level above, use that as top constraint
                    {
                        newTopLvl = overallLevelAbove;
                    }

                    /* determine if this is a single level, the bottom level, the top level, or a middle level */
                    /* single level wall */
                    if (1 == hostLevels.Count) //if there is only one level
                    {
                        msg += String.Format("Single Level-{0}: {1}\n", newHostLvl.Name, newHostLvl.Elevation);
                        /* set the bottom of the wall */
                        //confirm whether the wall bottom is outside of tolerance and requires an offset
                        if (!((newHostLvl.Elevation >= overallBottomElevation - tolerance) && (newHostLvl.Elevation <= overallBottomElevation + tolerance)))
                        {
                            newBottomOffset = overallBottomElevation - newHostLvl.Elevation;
                        }
                        msg += String.Format("+Bottom offset: {0}\n", newBottomOffset);
                        /* set the top of the wall */
                        if (newTopLvl != null) //if a valid top constraint was found
                        {
                            //confirm whether the wall top is outside of tolerance and requires an offset
                            if (!((newTopLvl.Elevation >= overallTopElevation - tolerance) && (newTopLvl.Elevation <= overallTopElevation + tolerance)))
                            {
                                newTopOffset = overallTopElevation - newTopLvl.Elevation;
                            }
                            msg += String.Format("+Top constraint: {0}\n+Top offset: {1}\n", newTopLvl.Name, newTopOffset);
                        }
                        else //the wall will be unconnected and newTopOffset will be the unconnected height
                        {
                            newTopOffset = overallTopElevation - newHostLvl.Elevation;
                            msg += String.Format("+Unconnected height: {0}", newTopOffset);
                        }
                    }
                    /* bottom level wall */
                    else if (newHostLvl == hostLevels.First()) //if this is the bottom level of the wall
                    {
                        //confirm whether the wall bottom is outside of tolerance and requires an offset
                        if (!((newHostLvl.Elevation >= overallBottomElevation - tolerance) && (newHostLvl.Elevation <= overallBottomElevation + tolerance)))
                        {
                            newBottomOffset = overallBottomElevation - newHostLvl.Elevation;
                        }
                        msg += String.Format("Bottom Level-{0}: {1}\n", newHostLvl.Name, newHostLvl.Elevation);
                        msg += String.Format("+Bottom offset: {0}\n", newBottomOffset);
                    }
                    /* top level wall */
                    else if (hostLevels[i].Id == hostLevels.Last().Id) //if this is the top level of the wall
                    {
                        msg += String.Format("Top Level-{0}: {1}\n", newHostLvl.Name, newHostLvl.Elevation);
                        /* set the top of the wall */
                        if (newTopLvl != null) //if a valid top constraint was found
                        {
                            //confirm whether the wall top is outside of tolerance and requires an offset
                            if (!((newTopLvl.Elevation >= overallTopElevation - tolerance) && (newTopLvl.Elevation <= overallTopElevation + tolerance)))
                            {
                                newTopOffset = overallTopElevation - newTopLvl.Elevation;
                            }
                            msg += String.Format("+Top constraint: {0}\n+Top offset: {1}\n", newTopLvl.Name, newTopOffset);
                        }
                        else //the wall will be unconnected and newTopOffset will be the unconnected height
                        {
                            newTopOffset = overallTopElevation - newHostLvl.Elevation;
                            msg += String.Format("+Unconnected height: {0}", newTopOffset);
                        }
                    }
                    /* middle level wall */
                    else //if this is a level between the top and bottom of the wall
                    {
                        msg += String.Format("Middle Level-{0}: {1}\n", newHostLvl.Name, newHostLvl.Elevation);
                    }

                    /* create the new wall */
                    var curve = (sourceWall.Location as LocationCurve).Curve;
                    var w = Wall.Create(dbDoc, curve, sourceWall.WallType.Id, newHostLvl.Id, 10, newBottomOffset, sourceWall.Flipped, true);
                    if (!(newTopLvl == null)) //if the wall is constrained to a top level
                    {
                        w.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(newTopLvl.Id); //set the top constraint of the wall
                        w.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(newTopOffset); //set the top offset
                    }
                    else //if the wall is unconnected
                    {
                        w.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).SetValueString("Unconnected"); //set the top constraint of the wall
                        w.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(newTopOffset); //set the unconnected height of the wall
                    }
                    w.StructuralUsage = sourceWall.StructuralUsage; //copy structural usage
                    //TODO: room bounding
                    /* step through and copy parameter values to the new wall */
                    List<BuiltInParameter> paramsToMatch = new List<BuiltInParameter> { //list of parameter values to copy to new wall
                        BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS, //comments
                        BuiltInParameter.WALL_KEY_REF_PARAM //location line
                    };
                    foreach (var p in paramsToMatch) //step through each parameter and copy its value to the new wall
                    {
                        var val = sourceWall.get_Parameter(p).AsValueString();
                        w.get_Parameter(p).SetValueString(val);
                    }
                }
                TaskDialog.Show("Wall Split by Level", msg);
                dbDoc.Delete(sourceWall.Id); //delete the original wall

                t.Commit();

            }

            return Result.Succeeded;
        }
    }
}
