using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Models
{
    public enum SnoopTasks
    {
        ExportAllData,
        Audit,
        Compact,
        AuditCompact
    }

    public class SnoopTask : ICloneable
    {
        /// <summary>
        /// The user friendly name for the task
        /// </summary>
        public String FriendlyName
        {
            get
            {
                return Enum.GetName(typeof(SnoopTasks), Task);
            }
        }

        /// <summary>
        /// The type of task
        /// </summary>
        public SnoopTasks Task { get; set; }

        /// <summary>
        /// Parameters passed to the engine executing the task
        /// </summary>
        public Dictionary<String, String> Parameters { get; set; }

        /// <summary>
        /// Return a clone of the task
        /// </summary>
        /// <returns>A cloned copy of the task</returns>
        public object Clone()
        {
            SnoopTask clone = new SnoopTask();

            clone.SetValue(this);

            return clone;
        }

        /// <summary>
        /// Sets all values to match the source task using a deep copy
        /// </summary>
        /// <param name="source">The source SnoopTask</param>
        public void SetValue(SnoopTask source)
        {
            this.Task = source.Task;
            this.Parameters = new Dictionary<String, String>();
            foreach (String key in source.Parameters.Keys)
            {
                this.Parameters.Add(key, source.Parameters[key]);
            }
        }
    }
}