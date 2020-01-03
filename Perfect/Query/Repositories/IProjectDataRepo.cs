using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DougKlassen.Revit.Query.Models;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface IProjectDataCatalogRepo
    {
        IEnumerable<ProjectDataModel> LoadProjectDataCatalog();
        void WriteProjectDataCatalog(IEnumerable<ProjectDataModel> catalog);
    }
}