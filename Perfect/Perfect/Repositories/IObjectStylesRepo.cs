using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DougKlassen.Revit.Perfect.Models;

namespace DougKlassen.Revit.Perfect.Repositories
{
    public interface IObjectStylesRepo
    {
        IEnumerable<ObjectStylesModel> LoadObjectStyles();
        void WriteObjectStyles(IEnumerable<ObjectStylesModel> objectStyles);
    }
}
