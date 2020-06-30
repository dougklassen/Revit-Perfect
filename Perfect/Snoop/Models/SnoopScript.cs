using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Models
{
    /// <summary>
    /// A script that is loaded and run by the task engine
    /// </summary>
    class SnoopScript
    {
        List<Tuple<String, List<SnoopTask>>> TaskList
        {
            get;
            set;
        }
    }
}
