using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Snoop;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Export data about schedules in the current project to a json file
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    class ExportSchedulesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<ViewSchedule> allSchedules =
                new FilteredElementCollector(dbDoc)
                .OfClass(typeof(ViewSchedule))
                .ToElements()
                .Cast<ViewSchedule>();

            List<ScheduleModel> scheduleData = new List<ScheduleModel>();
            foreach (ViewSchedule sched in allSchedules)
            {
                scheduleData.Add(new ScheduleModel(sched));
            }

            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                FileName = dbDoc.Title + "-schedules-" + SnoopHelpers.GetTimeStamp() + ".json",
                Filter = "JSON file|*.json",
                Title = "Save Schedule Catalog"
            };
            DialogResult result = saveDialog.ShowDialog();
            if (saveDialog.FileName == String.Empty || result == DialogResult.Cancel)
            {
                return Result.Cancelled;
            }
            IScheduleCatalogRepo schedRepo = new ScheduleCatalogJsonRepo(saveDialog.FileName);
            schedRepo.WriteScheduleCatalog(scheduleData);

            return Result.Succeeded;
        }
    }
}
