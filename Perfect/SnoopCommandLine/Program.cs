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
            SnoopConfig config;

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    #region bootstrap
                    case "bootstrap":
                        Console.WriteLine("\nBootstrapping config file");
                        Console.WriteLine("-----");

                        config = new SnoopConfig();
                        String workingDir = Directory.GetCurrentDirectory();
                        config.SetDefaultValues(workingDir);
                        if (File.Exists(config.ConfigFilePath))
                        {
                            Console.WriteLine("\nConfig file already exists at " + config.ConfigFilePath);
                            return;
                        }
                        ISnoopConfigRepo configRepo = new SnoopConfigJsonRepo(config.ConfigFilePath);
                        try
                        {
                            configRepo.WriteConfig(config);
                            Console.WriteLine("\nCreated new config file " + config.ConfigFilePath);
                            return;
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    #endregion bootstrap

                    #region check
                    case "check":
                        Console.WriteLine("\nConfig File Validation");
                        Console.WriteLine("-----");

                        String configFilePath = String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), SnoopConfig.configFileName);
                        String msg = String.Empty;

                        Console.WriteLine(String.Format("Looking for {0}\n", configFilePath));

                        //check the working directory
                        if (File.Exists(SnoopConfig.configFileName))
                        {
                            try
                            {
                                SnoopConfigJsonRepo repo = new SnoopConfigJsonRepo(configFilePath);
                                config = repo.LoadConfig();
                                ShowConfigInfo(config);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Couldn't load config file", e);
                            }
                        }
                        else
                        {
                            msg += "No config file found";
                        }
                        Console.WriteLine(msg);
                        return;
                    #endregion check

                    #region help
                    case "help":
                        break;
                    #endregion help

                    default:
                        break;
                }
            }

            //default to displaying the help message
            Console.WriteLine();
            ShowHelpMessage();
            return;
        }

        static void ShowHelpMessage()
        {
            String msg = String.Empty;
            msg += "Usage\n";
            msg += "-----\n";
            msg += "bootstrap: create a new configuration file\n";
            msg += "check: validate the configuration file\n";
            msg += "help: display this message\n";
            Console.Write(msg);
        }

        static void ShowConfigInfo(SnoopConfig config)
        {
            Console.Write(config.GetDescription());
        }
    }
}
