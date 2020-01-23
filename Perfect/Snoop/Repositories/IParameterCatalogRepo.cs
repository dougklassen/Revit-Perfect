using DougKlassen.Revit.Snoop.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface IParameterCatalogRepo
    {
        IEnumerable<ParameterModel> LoadParameterCatalog();
        void WriteParameterCatalog(IEnumerable<ParameterModel> catalog);
    }
}
