using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DougKlassen.Revit.Snoop.Models
{
    public class SnoopConfig : INotifyPropertyChanged
    {
        #region Fields
        FileLocations fileLocations = FileLocations.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        private String homeDirectoryPath;
        private String configFileName;
        private String taskFileName;
        private ObservableCollection<SnoopProject> activeProjects;
        private Dictionary<String, String> revitFilePaths;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Create a new SnoopConfig object. Values will not be set;
        /// </summary>
        public SnoopConfig()
        {
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The directory containing all config and task files
        /// </summary>
        public String HomeDirectoryPath {
            get
            {
                return homeDirectoryPath;
            }
            set
            {
                homeDirectoryPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The file name of the configuration file
        /// </summary>
        public String ConfigFileName {
            get
            {
                return configFileName;
            }
            set
            {
                configFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The file name used for task files
        /// </summary>
        public String TaskFileName {
            get
            {
                return taskFileName;
            }
            set
            {
                taskFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// All projects being processed
        /// </summary>
        public ObservableCollection<SnoopProject> ActiveProjects {
            get
            {
                return activeProjects;
            }
            set
            {
                activeProjects = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// All Revit installations available. The key is the version year. The value is the file path.
        /// </summary>
        public Dictionary<String, String> RevitFilePaths {
            get
            {
                return revitFilePaths;
            }
            set
            {
                revitFilePaths = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Set the config settings to default values
        /// </summary>
        public void SetDefaultValues()
        {
            HomeDirectoryPath = fileLocations.HomeDirectoryPath;
            ConfigFileName = fileLocations.ConfigFileName;
            TaskFileName = fileLocations.TaskFileName;
            ActiveProjects = new ObservableCollection<SnoopProject>();
            RevitFilePaths = fileLocations.RevitFilePaths;
        }

        /// <summary>
        /// Get a user friendly text description of the current config file
        /// </summary>
        /// <returns></returns>
        public String GetDescription()
        {
            String msg = String.Empty;
            msg += "Home Directory Path: " + HomeDirectoryPath;
            msg += "\nConfiguration File: " + ConfigFileName;
            msg += "\nCurrent Task File: " + TaskFileName;
            msg += "\nRevit Installations:";
            foreach (String install in RevitFilePaths.Keys)
            {
                msg += String.Format("\n\t{0}: {1}", install, RevitFilePaths[install]);
            }

            return msg;
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
        #endregion Methods
    }
}