using DougKlassen.Revit.Perfect.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Repositories
{
    class ParameterCatalogJsonRepo : IParameterCatalogRepo
    {
        private String jsonRepoFilePath;

        public ParameterCatalogJsonRepo(String filePath)
        {
            jsonRepoFilePath = filePath;
        }

        public IEnumerable<ParameterModel> GetParameterCatalog()
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
                File.WriteAllText(jsonRepoFilePath, jsonData);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write Parameters catalog", e);
            }
        }
    }
}
