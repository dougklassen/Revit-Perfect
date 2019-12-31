using DougKlassen.Revit.Perfect.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Repositories
{
    interface ICategoryCatalogRepo
    {
        IEnumerable<CategoryModel> LoadCategoryCatalog();
        void WriteScheduleCatalog(IEnumerable<CategoryModel> catalog);
    }
}
