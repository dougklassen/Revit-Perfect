using DougKlassen.Revit.Query.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Query.Repositories
{
    public interface IScheduleCatalogRepo
    {
        IEnumerable<ScheduleModel> LoadScheduleCatalog();
        void WriteScheduleCatalog(IEnumerable<ScheduleModel> catalog);
    }
}
