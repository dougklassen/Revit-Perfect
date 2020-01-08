using DougKlassen.Revit.Snoop.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public class SnoopConfigJsonRepo : ISnoopConfigRepo
    {
        private String filePath;

        public SnoopConfigJsonRepo(String configFilePath)
        {
            filePath = configFilePath;
        }

        public SnoopConfig LoadConfig()
        {
            String jsonData = File.ReadAllText(filePath);
            try
            {
                SnoopConfig config = (SnoopConfig)JsonConvert.DeserializeObject(jsonData, typeof(SnoopConfig));
                return config;
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't read config file", e);
            }
        }

        public void WriteConfig(SnoopConfig config)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            try
            {
                String jsonData = JsonConvert.SerializeObject(config, settings);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write config file", e);
            }
        }
    }
}
