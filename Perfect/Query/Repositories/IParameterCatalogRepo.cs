using DougKlassen.Revit.Query.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface IParameterCatalogRepo
    {
        IEnumerable<ParameterModel> LoadParameterCatalog();
        void WriteParameterCatalog(IEnumerable<ParameterModel> catalog);
    }
}
