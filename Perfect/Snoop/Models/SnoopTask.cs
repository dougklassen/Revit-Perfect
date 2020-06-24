using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DougKlassen.Revit.Snoop.Models
{
    /// <summary>
    /// An enumeration of task types
    /// </summary>
    public enum SnoopTaskType
    {
        ExportAllData,
        Audit,
        Compact,
        AuditCompact
    }

    /// <summary>
    /// A task run on a project by the task engine
    /// </summary>
    public class SnoopTask : ICloneable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SnoopTaskType taskType;
        private ObservableCollection<SnoopTaskParameter> taskParameters;

        public SnoopTask()
        {            
            TaskParameters = new ObservableCollection<SnoopTaskParameter>();
        }

        /// <summary>
        /// The user friendly name for the task
        /// </summary>
        public String FriendlyName
        {
            get
            {
                return TaskType.GetFriendlyName();
            }
        }

        /// <summary>
        /// The type of task
        /// </summary>
        public SnoopTaskType TaskType
        {
            get
            {
                return taskType;
            }
            set
            {
                taskType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FriendlyName)); //changing the TaskType also changes FriendlyName
            }
        }

        /// <summary>
        /// Parameters passed to the engine executing the task
        /// </summary>
        public ObservableCollection<SnoopTaskParameter> TaskParameters
        {
            get
            {
                return taskParameters;
            }
            set
            {
                taskParameters = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Return a canonical list of all parameters types used by a specified SnoopTaskType.
        /// Parameter type determines the interface presented when setting the parameter.
        /// </summary>
        /// <param name="taskType">The specified task type</param>
        /// <returns>Parameters used by the task</returns>
        public static List<SnoopParameterType> GetParameterTypes(SnoopTaskType taskType)
        {
            List<SnoopParameterType> paramTypes = new List<SnoopParameterType>();

            switch (taskType)
            {
                case SnoopTaskType.ExportAllData:
                    paramTypes.Add(SnoopParameterType.FileOutputDirectory);
                    break;
                case SnoopTaskType.Audit:
                    break;
                case SnoopTaskType.Compact:
                    break;
                case SnoopTaskType.AuditCompact:
                    break;
            }

            return paramTypes;
        }

        /// <summary>
        /// Sets all values to match the source task using a deep copy
        /// </summary>
        /// <param name="source">The source SnoopTask</param>
        public void SetValues(SnoopTask source)
        {
            this.TaskType = source.TaskType;
            this.TaskParameters = new ObservableCollection<SnoopTaskParameter>();
            foreach (SnoopTaskParameter taskParam in source.TaskParameters)
            {
                this.TaskParameters.Add(taskParam);
            }
        }

        /// <summary>
        /// Return a clone of the task
        /// </summary>
        /// <returns>A cloned copy of the task</returns>
        public object Clone()
        {
            SnoopTask clone = new SnoopTask();

            clone.SetValues(this);

            return clone;
        }

        /// <summary>
        /// Raise the PropertyChanged event
        /// </summary>
        /// <param name="name">The name of the Property that has been updated. This is provided via
        /// the CallerMemberName attribute</param>
        protected void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}