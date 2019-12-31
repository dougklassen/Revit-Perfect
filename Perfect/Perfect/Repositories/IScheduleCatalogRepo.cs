using DougKlassen.Revit.Perfect.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Repositories
{
    interface IScheduleCatalogRepo
    {
        IEnumerable<ScheduleModel> LoadScheduleCatalog();
        void WriteScheduleCatalog(IEnumerable<ScheduleModel> catalog);
    }
}
