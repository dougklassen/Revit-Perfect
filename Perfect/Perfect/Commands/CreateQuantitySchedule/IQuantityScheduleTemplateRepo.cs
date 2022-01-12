using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Write and load a collection of templates used in creating standardized quantity schedules
    /// By convention, the key to the dictionary corresponds to the value of FilterParamValue
    /// </summary>
    public interface IQuantityScheduleTemplateRepo
    {
        List<QuantityScheduleTemplate> LoadTemplates();
        void WriteTemplates(List<QuantityScheduleTemplate> templates);
    }
}
