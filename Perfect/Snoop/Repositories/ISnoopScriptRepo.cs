using DougKlassen.Revit.Snoop.Models;

namespace DougKlassen.Revit.Snoop.Repositories
{
    public interface ISnoopScriptRepo
    {
        SnoopScript LoadScript();

        void WriteScript(SnoopScript script);
    }
}
