using DougKlassen.Revit.Perfect.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Repositories
{
    interface IParameterCatalogRepo
    {
        IEnumerable<ParameterModel> LoadParameterCatalog();
        void WriteParameterCatalog(IEnumerable<ParameterModel> catalog);
    }
}
