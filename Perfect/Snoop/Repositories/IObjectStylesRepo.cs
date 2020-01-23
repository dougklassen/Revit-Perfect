using DougKlassen.Revit.Snoop.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface IObjectStylesRepo
    {
        IEnumerable<ObjectStylesModel> LoadObjectStyles();
        void WriteObjectStyles(IEnumerable<ObjectStylesModel> catalog);
    }
}
