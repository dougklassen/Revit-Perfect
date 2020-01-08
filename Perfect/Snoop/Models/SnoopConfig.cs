﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Snoop.Models
{
    public class SnoopConfig
    {
        private readonly String configFileName = "SnoopConfig.json";
        private readonly String taskFileName = "SnoopTask.json";

        public String HomeDirectoryPath { get; set; }
        public String ConfigFilePath { get; set; }
        public String CurrentTaskFilePath { get; set; }
        public IEnumerable<SnoopTask> ToDoList { get; set; }
        public IEnumerable<String> ActiveProjects { get; set; }
        public Dictionary<String, String> RevitFilePaths { get; set; }

        public SnoopConfig()
        {
        }

        public void SetDefaultValues(String homeDir)
        {
            if (!homeDir.EndsWith("\\"))
            {
                homeDir += "\\";
            }
            HomeDirectoryPath = homeDir;
            ConfigFilePath = homeDir + configFileName;
            CurrentTaskFilePath = homeDir + taskFileName;
            ToDoList = new List<SnoopTask>();
            ActiveProjects = new List<String>();
            RevitFilePaths = new Dictionary<String, String>();
            for (int i = 2010; i <= 2030; i++)
            {
                String revitPath = String.Format(@"C:\Program Files\Autodesk\Revit {0}\Revit.exe",i);
                if (File.Exists(revitPath))
                {
                    RevitFilePaths.Add(i.ToString(), revitPath);
                }
            }
        }
    }
}
