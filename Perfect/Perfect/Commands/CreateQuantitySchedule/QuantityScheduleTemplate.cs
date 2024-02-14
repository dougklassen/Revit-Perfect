using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// A model of a standard quantity schedule template containing information to generate a new schedule
    /// </summary>
    public class QuantityScheduleTemplate
    {
        /// <summary>
        /// The category of Elements that will be scheduled. This value is taken from the name of the category in the Revit API BuiltInParams enumeration
        /// </summary>
        public String ElementCategory { get; set; }
        /// <summary>
        /// The name of the Parameter that will be used to filter the schedule. A single filter will be added to the schedule, with Filter By set to this parameter
        /// </summary>
        public String FilterParameterName { get; set; }
        /// <summary>
        /// This value will be used in the schedule filter. Only elements who's filter parameter value matches this value
        /// will be included in the schedule
        /// </summary>
        public String FilterParameterValue { get; set; }
        /// <summary>
        /// Further description of the meaning of the value of FilterParameterValue. Combined with FilterParameterValue to name the schedule
        /// </summary>
        public String FilterParameterValueLabel { get; set; }
        /// <summary>
        /// All of the fields included in the schedule
        /// </summary>
        public List<QuantityScheduleField> Fields { get; set; }
        /// <summary>
        /// Initialize a new QuantityScheduleTemplate
        /// </summary>
        public QuantityScheduleTemplate()
        {
            Fields = new List<QuantityScheduleField>();
        }

        /// <summary>
        /// Get description for schedule including its filter parameter and how many fields it contains
        /// </summary>
        /// <returns>A schedule description</returns>
        public String GetDescription()
        {
            String catLabel;
            try
            {
                BuiltInCategory cat = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), ElementCategory);
                catLabel = LabelUtils.GetLabelFor(cat);
            }
            catch (Exception)
            {
                catLabel = ElementCategory;
            }
            String desc = String.Format("{0}: {1} ({2}) - {3} fields", FilterParameterValue, FilterParameterValueLabel, catLabel, Fields.Count);
            return desc;
        }
    }

    /// <summary>
    /// The definition of a field in the schedule. The column heading for the field will be in the format Label (Units)
    /// </summary>
    public class QuantityScheduleField
    {
        /// <summary>
        /// The description label for the field
        /// </summary>
        public String Label { get; set; }
        /// <summary>
        /// The unit label for the field
        /// </summary>
        public String Units { get; set; }
        /// <summary>
        /// The units of value displayed by the field. Either a user defined project parameter, a built in parameter, or a calculation based on the 
        /// </summary>
        public FieldType Type { get; set; }
        /// <summary>
        /// The source for the fields value. This will be either the name of a parameter value or a calculation, as specified by Type
        /// </summary>
        public String FieldValue { get; set; }
    }

    public enum FieldType
    {
        ProjectParameter,
        BuiltInParameter,
        Calculation
    }
}
