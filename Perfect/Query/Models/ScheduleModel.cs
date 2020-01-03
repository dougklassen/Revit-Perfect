using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Models
{
    public class ScheduleModel
    {
        public String name;
        public Int32 id;
        public List<Int32> parameters;

        public ScheduleModel(ViewSchedule sched)
        {
            name = sched.Name;
            id = sched.Id.IntegerValue;

            parameters = new List<Int32>();
            IList<ScheduleFieldId> fieldIds = sched.Definition.GetFieldOrder();
            foreach (ScheduleFieldId id in fieldIds)
            {
                ScheduleField field = sched.Definition.GetField(id);

                parameters.Add(field.ParameterId.IntegerValue);
            }
        }
    }
}
