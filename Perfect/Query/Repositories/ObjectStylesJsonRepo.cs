using DougKlassen.Revit.Query.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Query.Repositories
{
    public class ObjectStylesJsonRepo : IObjectStylesRepo
    {
        private String filePath;

        public ObjectStylesJsonRepo(String jsonRepoFilePath)
        {
            filePath = jsonRepoFilePath;
        }

        public IEnumerable<ObjectStylesModel> LoadObjectStyles()
        {
            String jsonData = File.ReadAllText(filePath);
            try
            {
                List<ObjectStylesModel> objectStyles = (List<ObjectStylesModel>)JsonConvert.DeserializeObject(jsonData, typeof(List<ObjectStylesModel>));
                return objectStyles;
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't read Object Styles catalog", e);
            }
        }

        public void WriteObjectStyles(IEnumerable<ObjectStylesModel> catalog)
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
                throw new Exception("Couldn't write Object Styles catalog", e);
            }
        }
    }
}
