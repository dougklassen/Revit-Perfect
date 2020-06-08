using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Snoop.Models
{
    public class SnoopConfig
    {
        FileLocations fileLocations = FileLocations.Instance;

        public String HomeDirectoryPath { get; set; }
        public String ConfigFilePath { get; set; }
        public String CurrentTaskFilePath { get; set; }
        public IEnumerable<SnoopTask> TaskList { get; set; }
        public IEnumerable<String> ActiveProjects { get; set; }
        public Dictionary<String, String> RevitFilePaths { get; set; }

        /// <summary>
        /// Create a new SnoopConfig object. Values will not be set;
        /// </summary>
        public SnoopConfig()
        {
        }

        /// <summary>
        /// Set the config settings to default values
        /// </summary>
        public void SetDefaultValues()
        {
            HomeDirectoryPath = fileLocations.HomeDirectoryPath;
            ConfigFilePath = fileLocations.ConfigFilePath;
            CurrentTaskFilePath = fileLocations.TaskFileName;
            TaskList = new List<SnoopTask>();
            ActiveProjects = new List<String>();
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
            msg += "\nFile Path: " + ConfigFilePath;
            msg += "\nCurrent Task Path: " + CurrentTaskFilePath;
            msg += "\nRevit Installations:";
            foreach (String install in RevitFilePaths.Keys)
            {
                msg += String.Format("\n\t{0}: {1}", install, RevitFilePaths[install]);
            }

            return msg;
        }
    }
}
