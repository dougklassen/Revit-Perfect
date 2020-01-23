using DougKlassen.Revit.Snoop.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface ICategoryCatalogRepo
    {
        IEnumerable<CategoryModel> LoadCategoryCatalog();
        void WriteScheduleCatalog(IEnumerable<CategoryModel> catalog);
    }
}
