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
        String configFilePath = FileLocations.Instance.ConfigFilePath;
        SnoopConfig config;

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            if (File.Exists(configFilePath))
            {
                SnoopConfigJsonRepo configRepo = new SnoopConfigJsonRepo(configFilePath);
                try
                {
                    config = configRepo.LoadConfig();
                }
                catch (Exception)
                {
                    TaskDialog.Show("Snoop", "Couldn't parse config file");
                    return Result.Failed;
                }
            }
            else
            {
                TaskDialog.Show("Snoop", "No config file found at " + configFilePath);
            }

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
