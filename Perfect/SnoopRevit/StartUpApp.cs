using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Snoop;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;

namespace DougKlassen.Revit.Snoop
{
    public class StartUpApp : IExternalApplication
    {
        FileLocations fileLocations = FileLocations.Instance;
        SnoopConfig config;

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            if (File.Exists(fileLocations.ConfigFilePath))
            {
                SnoopConfigJsonRepo configRepo = new SnoopConfigJsonRepo(fileLocations.ConfigFilePath);
                try
                {
                    config = configRepo.LoadConfig();
                }
                catch (Exception)
                {
                    TaskDialog.Show("Snoop", "Couldn't parse config file");
                    return Result.Failed;
                }
                TaskDialog.Show("Snoop", "Config file " + config.ConfigFilePath + " loaded");
            }
            else
            {
                TaskDialog.Show("Snoop", "No config file found");
            }

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
