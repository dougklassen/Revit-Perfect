using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using DougKlassen.Revit.Perfect.Interface;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Add user specified values to the comment parameter of selected elements.
    /// Values are considered to be individual white space delimited words. A value is not
    /// added to a comment if it already exists. This command has the side effect of stripping leading
    /// and trailing white space off comments. It is case sensitive.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CommentAddCommand : IExternalCommand
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

            //Cancel the command if no elements are selected
            if (selectedElements.Count < 1)
            {
                TaskDialog.Show("Remove Comments", "You must select at least one element");
                return Result.Failed;
            }

            //Show a window to request the user to enter comments to add
            AddCommentWindow commentWindow = new AddCommentWindow();
            Boolean result = commentWindow.ShowDialog().Value;
            if (!result)
            {
                return Result.Cancelled;
            }

            //Take the input string and extract separate values to add
            IEnumerable<String> inputValues = splitRegex.Split(commentWindow.CommentsToAdd);

            //Step through each selected element and add values that don't already exist
            using (Transaction t = new Transaction(dbDoc, "add comments"))
            {
                t.Start();
                foreach (ElementId id in selectedElements)
                {
                    Element elem = dbDoc.GetElement(id);
                    IList<Parameter> commentParams = elem.GetParameters("Comments");

                    //only process elements that have comments
                    if (commentParams.Count < 1)
                    {
                        continue;
                    }

                    //add values to the comment if they don't already exist
                    Parameter commentParam = commentParams.First();
                    String commentValue = commentParam.AsString() ?? String.Empty;
                    List<String> commentValues = new List<String>();
                    if (!String.IsNullOrWhiteSpace(commentValue))
                    {
                        commentValues = splitRegex.Split(commentValue).ToList();
                    }
                    foreach (String newValue in inputValues)
                    {
                        if (!commentValues.Contains(newValue))
                        {
                            commentValue = commentValue.Trim() + delimiter + newValue;
                        }
                    }

                    //assign the new value of the comment string back to the comment parameter
                    commentParam.Set(commentValue);
                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
