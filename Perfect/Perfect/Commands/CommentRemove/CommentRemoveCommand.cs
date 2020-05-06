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
            //use this string to delimit separate values in a comment string
            String delimiter = " ";

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

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
                String commentString = commentParams.First().AsString() ?? String.Empty;

                //add the comments of the current element to the list
                HashSet<String> currentComments = new HashSet<String>(splitRegex.Split(commentString));
                activeComments.UnionWith(currentComments);
            }

            //String msg = "Found Comments: ";
            //foreach (var str in activeComments)
            //{
            //    msg += str + delimiter;
            //}
            //TaskDialog.Show("Comments Present", msg);

            SelectObjectsWindow window = new SelectObjectsWindow(
                activeComments,
                false,
                "Select Comments",
                "Select comments to be removed from elements");

            Boolean result = (Boolean) window.ShowDialog();

            if (!result)
            {
                return Result.Cancelled;
            }
            else
            {
                return Result.Succeeded;
            }
        }
    }
}
