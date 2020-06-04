using Newtonsoft.Json;
using System;
using System.IO;

namespace DougKlassen.Revit.Perfect
{
    public class VizSettingsJsonRepo : IVizSettingsRepo
    {
        private static String configFileName = "VizSettings.json";
        private static String configFilePath = FileLocations.AddInDirectory + configFileName;

        public VizSettings LoadSettings()
        {
            VizSettings settings;

            if (!File.Exists(configFilePath))
            {
                WriteSettings(new VizSettings());
            }

            try
            {
                String jsonText = File.ReadAllText(configFilePath);
                settings = JsonConvert.DeserializeObject<VizSettings>(jsonText);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't read config file", e);
            }

            return settings;
        }

        public void WriteSettings(VizSettings settings)
        {
            if (!Directory.Exists(FileLocations.AddInDirectory))
            {
                Directory.CreateDirectory(FileLocations.AddInDirectory);
            }

            try
            {
                String jsonText = JsonConvert.SerializeObject(settings);
                File.WriteAllText(configFilePath, jsonText);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write config file", e);
            }
        }
    }
}
