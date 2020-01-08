using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using System;
using System.IO;

namespace DougKlassen.Revit.Snoop
{
    class Program
    {
        static void Main(String[] args)
        {
            SnoopConfig config = new SnoopConfig();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "bootstrap":
                        String workingDir = Directory.GetCurrentDirectory();
                        config.SetDefaultValues(workingDir);
                        if (File.Exists(config.ConfigFilePath))
                        {
                            Console.WriteLine("Config file already exists at " + config.ConfigFilePath);
                            return;
                        }
                        ISnoopConfigRepo configRepo = new SnoopConfigJsonRepo(config.ConfigFilePath);
                        try
                        {
                            configRepo.WriteConfig(config);
                            Console.WriteLine("Created new config file " + config.ConfigFilePath);
                            return;
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    case "help":
                        break;
                    default:
                        break;
                } 
            }
            //default to displaying the help message
            Console.WriteLine(HelpMessage);
            return;
        }

        static String HelpMessage
        {
            get
            {
                String msg = String.Empty;
                msg += "\n\tUsage:\n";
                msg += "\t-----\n";
                msg += "\tbootstrap: create a new configuration file\n";
                msg += "\thelp: display this message\n";
                return msg;
            }
        }
    }
}
