using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Perfect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommentRemoveCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //use this regex to split comment strings into separate values
            Regex splitRegex = new Regex(@"\s+");
            //use this as a delimeter when adding a separation between tags
            String delimeter = " ";

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

#region Gather comments
            var selectedElements = uiDoc.Selection.GetElementIds();

            if (selectedElements.Count < 1)
            {
                TaskDialog.Show("Remove Comments", "You must select at least one element");
                return Result.Failed;
            }

            HashSet<String> activeComments = new HashSet<String>();

            //step through all selected elements and build a list of all comments present
            foreach (ElementId id in selectedElements)
            {
                Element elem = dbDoc.GetElement(id);
                IList<Parameter> commentParams = elem.GetParameters("Comments");

                //skip elements without a Comments parameter
                if (commentParams.Count < 1)
                {
                    continue;
                }

                Parameter commentParam = commentParams.First();
                String commentString = commentParams.First().AsString();

                //skip elements with an empty Comments parameter
                if (String.IsNullOrWhiteSpace(commentString))
                {
                    continue;
                }

                //add the comments of the current element to the list
                HashSet<String> currentComments = new HashSet<String>(splitRegex.Split(commentString));
                activeComments.UnionWith(currentComments);
            }
#endregion Gather comments

#region Select comments to delete
            SelectObjectsWindow window = new SelectObjectsWindow(
                activeComments,
                false,
                "Select Comments",
                "Select comments to be removed from elements");

            Boolean result = (Boolean) window.ShowDialog();
#endregion Select commments to delete

#region Delete selected comments
            if (!result)
            {
                return Result.Cancelled;
            }
            else
            {
                using (Transaction t = new Transaction(dbDoc, "Remove comments"))
                {
                    t.Start();

                    IEnumerable<String> commentsToRemove = window.SelectedObjects.Cast<String>();

                    //step through all selected elements and remove marked comments
                    foreach (ElementId id in selectedElements)
                    {
                        Element elem = dbDoc.GetElement(id);
                        IList<Parameter> commentParams = elem.GetParameters("Comments");

                        //skip elements without a Comments parameter
                        if (commentParams.Count < 1)
                        {
                            continue;
                        }

                        Parameter commentParam = commentParams.First();
                        String commentString = commentParams.First().AsString();

                        //skip elements with an empty Comments parameter
                        if (String.IsNullOrWhiteSpace(commentString))
                        {
                            continue;
                        }

                        //replace all instances of the specified comments and surrounding whitespace with the delimeter
                        foreach (String removedComment in commentsToRemove)
                        {
                            Regex removalRegEx = new Regex(@"(?:^|\s+)" + removedComment + @"\s*(?:$|\s)");
                            commentString = removalRegEx.Replace(commentString, delimeter);
                        }

                        commentString = commentString.Trim();
                        commentParam.Set(commentString);
                    }

                    t.Commit();
                }

                return Result.Succeeded;
            }
#endregion Delete selected comments

        }
    }
}
