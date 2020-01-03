using DougKlassen.Revit.Query.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface IProjectDataCatalogRepo
    {
        ProjectDataModel LoadProjectDataCatalog();
        void WriteProjectDataCatalog(ProjectDataModel catalog);
    }
}