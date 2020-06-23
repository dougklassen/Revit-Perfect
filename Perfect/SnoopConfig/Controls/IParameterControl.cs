using DougKlassen.Revit.Snoop.Models;
using System;
using System.Windows.Controls;

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
