using DougKlassen.Revit.Snoop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public class ParameterCatalogJsonRepo : IParameterCatalogRepo
    {
        private String filePath;

        public ParameterCatalogJsonRepo(String jsonRepoFilePath)
        {
            filePath = jsonRepoFilePath;
        }

        public IEnumerable<ParameterModel> LoadParameterCatalog()
        {
            throw new NotImplementedException();
        }

        public void WriteParameterCatalog(IEnumerable<ParameterModel> catalog)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            String jsonData = JsonConvert.SerializeObject(catalog, settings);
            try
            {
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write Parameters catalog", e);
            }
        }
    }
}
