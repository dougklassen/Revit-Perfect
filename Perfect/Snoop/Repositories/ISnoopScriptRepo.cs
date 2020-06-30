using DougKlassen.Revit.Snoop.Models;

namespace DougKlassen.Revit.Snoop.Repositories
{
    interface ISnoopScriptRepo
    {
        SnoopScript LoadScript();

        void WriteScript(SnoopScript script);
    }
}
