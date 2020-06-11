using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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
            String path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            HomeDirectoryPath = StandardizeToTrailingBackslash(path);

            RevitFilePaths = new Dictionary<String, String>();
            for (int i = 2010; i <= 2030; i++)
            {
                String revitPath = String.Format(@"C:\Program Files\Autodesk\Revit {0}\Revit.exe", i);
                if (File.Exists(revitPath))
                {
                    RevitFilePaths.Add(i.ToString(), revitPath);
                }
            }
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
            get;
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
            get;
            private set;
        }

        /// <summary>
        /// Return a version of a directory path that doesn't end with a trailing backslash
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String StandardizeToNoTrailingBackslash(String path)
        {
            //match trailing slashes and whitespace
            Regex trimRegEx = new Regex(@"[\\|\/|\s]+$");
            path = trimRegEx.Replace(path, String.Empty);
            return path;
        }

        /// <summary>
        /// Return a version of a directory path that ends with a trailing backslash
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String StandardizeToTrailingBackslash(String path)
        {
            return StandardizeToNoTrailingBackslash(path) + "\\";
        }
    }
}
