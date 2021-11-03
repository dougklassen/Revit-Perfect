using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DougKlassen.Revit.Snoop.Models
{
    /// <summary>
    /// Complete configuration information for Snoop including all active projects 
    /// </summary>
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
        /// The file name used for task script files
        /// </summary>
        public String ScriptFileName {
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
        /// All projects being managed by Snoop 
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
            ScriptFileName = fileLocations.ScriptFileName;
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
            msg += "\nCurrent Task File: " + ScriptFileName;
            msg += "\nRevit Installations:";
            foreach (String install in RevitFilePaths.Keys)
            {
                msg += String.Format("\n\t{0}: {1}", install, RevitFilePaths[install]);
            }

            return msg;
        }

        /// <summary>
        /// Generate a dictionary of scripts keyed by Revit version for all active project tasks.
        /// </summary>
        /// <returns>A dictionary where the key is the version of Revit targeted and the value is a script to run on that version</returns>
        public Dictionary<String, SnoopScript> GenerateScripts()
        {
            Dictionary<String, SnoopScript> scripts = new Dictionary<String, SnoopScript>();

            foreach (SnoopProject project in ActiveProjects)
            {
                if (String.IsNullOrWhiteSpace(project.RevitVersion))
                {
                    continue;
                }
                else if (project.TaskList.Count > 0)
                {
                    //Create a script for the project's Revit version if it doesn't exist already
                    if (!scripts.ContainsKey(project.RevitVersion))
                    {
                        scripts.Add(project.RevitVersion, new SnoopScript());
                    }
                    scripts[project.RevitVersion].AddProject(project);
                }
            }

            return scripts;
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