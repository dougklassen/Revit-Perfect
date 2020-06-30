using DougKlassen.Revit.Snoop.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DougKlassen.Revit.Snoop.Repositories
{
    class SnoopScriptJsonRepo : ISnoopScriptRepo
    {
        private String filePath;

        public SnoopScriptJsonRepo(String scriptFilePath)
        {
            filePath = scriptFilePath;
        }

        public SnoopScript LoadScript()
        {
            String jsonData = File.ReadAllText(filePath);
            try
            {
                SnoopScript script = (SnoopScript)JsonConvert.DeserializeObject(jsonData, typeof(SnoopScript));
                return script;
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't parse script file", e);
            }
        }

        public void WriteScript(SnoopScript script)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            try
            {
                String jsonData = JsonConvert.SerializeObject(script, settings);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write script file", e);
            }
        }
    }
}
