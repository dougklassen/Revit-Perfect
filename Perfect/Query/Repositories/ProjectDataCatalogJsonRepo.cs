﻿using DougKlassen.Revit.Query.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DougKlassen.Revit.Query.Repositories
{
    public class ProjectDataCatalogJsonRepo : IProjectDataCatalogRepo
    {
        private String jsonRepoFilePath;

        public ProjectDataCatalogJsonRepo(String filePath)
        {
            jsonRepoFilePath = filePath;
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
                File.WriteAllText(jsonRepoFilePath, jsonData);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't write Schedule catalog", e);
            }
        }
    }
}
