using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DougKlassen.Revit.Snoop.Models
{
    public class SnoopProject : ICloneable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String projectName;
        private String filePath;
        private String revitVersion;
        private ObservableCollection<SnoopTask> taskList;

        public SnoopProject()
        {
            taskList = new ObservableCollection<SnoopTask>();
        }

        /// <summary>
        /// The title of the project
        /// </summary>
        public String ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The path to the project file
        /// </summary>
        public String FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The version of Revit the project is using, as a string containing only the year, e.g. "2020"
        /// </summary>
        public String RevitVersion
        {
            get
            {
                return revitVersion;
            }
            set
            {
                revitVersion = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// All tasks scheduled to be run on the project
        /// </summary>
        public ObservableCollection<SnoopTask> TaskList
        {
            get
            {
                return taskList;
            }
            set
            {
                taskList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets all properties to match the source SnoopProject, using a deep copy
        /// </summary>
        /// <param name="source">The source SnoopProject</param>
        public void SetValues(SnoopProject source)
        {
            this.ProjectName = source.ProjectName;
            this.FilePath = source.FilePath;
            this.RevitVersion = source.RevitVersion;
            this.TaskList = new ObservableCollection<SnoopTask>();
            foreach (SnoopTask task in source.TaskList)
            {
                this.TaskList.Add((SnoopTask)task.Clone());
            }
        }

        /// <summary>
        /// Returns a clone of the SnoopProject
        /// </summary>
        /// <returns>A cloned copy of the SnoopProject</returns>
        public Object Clone()
        {
            SnoopProject clone = new SnoopProject();
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
