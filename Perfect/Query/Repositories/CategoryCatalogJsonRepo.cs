using DougKlassen.Revit.Query.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Query.Repositories
{
    public class CategoryCatalogJsonRepo : ICategoryCatalogRepo
    {
        private String jsonRepoFilePath;

        public CategoryCatalogJsonRepo(String filePath)
        {
            jsonRepoFilePath = filePath;
        }

        public IEnumerable<CategoryModel> LoadCategoryCatalog()
        {
            throw new NotImplementedException();
        }

        public void WriteScheduleCatalog(IEnumerable<CategoryModel> catalog)
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
                throw new Exception("Couldn't write Schedule catalog", e);
            }
        }
    }
}
