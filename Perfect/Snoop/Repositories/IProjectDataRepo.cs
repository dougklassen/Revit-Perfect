using DougKlassen.Revit.Snoop.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface IProjectDataCatalogRepo
    {
        ProjectDataModel LoadProjectDataCatalog();
        void WriteProjectDataCatalog(ProjectDataModel catalog);
    }
}