using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

using DougKlassen.Revit.Perfect.Models;

namespace DougKlassen.Revit.Perfect.Repositories
{
    internal class JsonFileObjectStyleRepo : IObjectStylesRepo
    {
        private String jsonRepoFilePath;

        private JsonFileObjectStyleRepo() { }

        public JsonFileObjectStyleRepo(String fileName)
            : this()
        {
            jsonRepoFilePath = fileName;
        }

        public IEnumerable<ObjectStylesModel> LoadObjectStyles()
        {
            List<ObjectStylesModel> objectStyles = null;
            using (FileStream fs = File.Open(jsonRepoFilePath, FileMode.Open))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ObjectStylesModel>));
                objectStyles = (List<ObjectStylesModel>)s.ReadObject(fs);
            }

            return objectStyles;
        }

        public void WriteObjectStyles(IEnumerable<ObjectStylesModel> objectStyles)
        {
            using (FileStream fs = File.Create(jsonRepoFilePath))
            {

                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<ObjectStylesModel>));
                s.WriteObject(fs, objectStyles);
            }
        }
    }
}
