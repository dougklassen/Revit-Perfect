using DougKlassen.Revit.Snoop.Models;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface IScheduleCatalogRepo
    {
        IEnumerable<ScheduleModel> LoadScheduleCatalog();
        void WriteScheduleCatalog(IEnumerable<ScheduleModel> catalog);
    }
}
