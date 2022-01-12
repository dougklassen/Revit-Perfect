﻿using DougKlassen.Revit.Snoop.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public class ProjectDataCatalogJsonRepo : IProjectDataCatalogRepo
    {
        private String filePath;

        public ProjectDataCatalogJsonRepo(String jsonRepoFilePath)
        {
            filePath = jsonRepoFilePath;
        }

        public ProjectDataModel LoadProjectDataCatalog()
        {
            throw new NotImplementedException();
        }

        public void WriteProjectDataCatalog(ProjectDataModel catalog)
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
                throw new Exception("Couldn't write Schedule catalog", e);
            }
        }
    }
}
