using DougKlassen.Revit.Snoop.Models;

namespace DougKlassen.Revit.SnoopConfigurator
{
    interface IParameterControl
    {
        SnoopTaskParameter TaskParameter
        {
            get;
            set;
        }
    }
}
