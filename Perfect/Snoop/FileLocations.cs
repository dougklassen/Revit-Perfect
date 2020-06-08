using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Snoop
{
    /// <summary>
    /// A class used to retrieve file locations and names. The class is written as a singleton so it
    /// can use run time information to generate file paths
    /// </summary>
    public sealed class FileLocations
    {
        private FileLocations()
        {
        }

        private static readonly Lazy<FileLocations> lazy
            = new Lazy<FileLocations>(() => new FileLocations());

        public static FileLocations Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        /// <summary>
        /// The name of the configuration file
        /// </summary>
        public String ConfigFileName
        {
            get
            {
                return "SnoopConfig.json";
            }
        }

        /// <summary>
        /// The path of the home directory. Currently set to the same directory as the assembly
        /// </summary>
        public String HomeDirectoryPath
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }

        /// <summary>
        /// The full path of the config file
        /// </summary>
        public String ConfigFilePath
        {
            get
            {
                return HomeDirectoryPath + ConfigFileName;
            }
        }

        /// <summary>
        /// The name of the tasks file that will be used to run Revit Tasks
        /// </summary>
        public String TaskFileName
        {
            get
            {
                return "SnoopTask.json";
            }
        }

        /// <summary>
        /// The full path of the tasks file
        /// </summary>
        public String TaskFilePath
        {
            get
            {
                return HomeDirectoryPath + TaskFileName;
            }
        }

        /// <summary>
        /// Search the machine for all currently installed versions of Revit
        /// </summary>
        public Dictionary<String, String> RevitFilePaths
        {
            get
            {
                Dictionary<String, String> paths = new Dictionary<String, String>();
                for (int i = 2010; i <= 2030; i++)
                {
                    String revitPath = String.Format(@"C:\Program Files\Autodesk\Revit {0}\Revit.exe", i);
                    if (File.Exists(revitPath))
                    {
                        paths.Add(i.ToString(), revitPath);
                    }
                }
                return paths;
            }
        }
    }
}
