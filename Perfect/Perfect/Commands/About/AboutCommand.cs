using Autodesk.Revit.UI;
using System.Reflection;

namespace DougKlassen.Revit.Perfect.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class AboutCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            Assembly perfectAsm = Assembly.GetExecutingAssembly();
            TaskDialog.Show(
                perfectAsm.GetName().Version.ToString(),
                perfectAsm.GetName().Name + '\n' +
                perfectAsm.Location + '\n' +
                perfectAsm.GetName().Version);

            return Result.Succeeded;
        }
    }
}