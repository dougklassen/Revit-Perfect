using DougKlassen.Revit.Snoop.Models;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface IProjectDataCatalogRepo
    {
        ProjectDataModel LoadProjectDataCatalog();
        void WriteProjectDataCatalog(ProjectDataModel catalog);
    }
}