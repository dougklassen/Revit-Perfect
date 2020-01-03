using DougKlassen.Revit.Query.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface IObjectStylesRepo
    {
        IEnumerable<ObjectStylesModel> LoadObjectStyles();
        void WriteObjectStyles(IEnumerable<ObjectStylesModel> catalog);
    }
}
