using DougKlassen.Revit.Snoop.Models;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface ISnoopConfigRepo
    {
        SnoopConfig LoadConfig();
        void WriteConfig(SnoopConfig config);
    }
}
