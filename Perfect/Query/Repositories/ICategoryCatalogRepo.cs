using DougKlassen.Revit.Query.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface ICategoryCatalogRepo
    {
        IEnumerable<CategoryModel> LoadCategoryCatalog();
        void WriteScheduleCatalog(IEnumerable<CategoryModel> catalog);
    }
}
