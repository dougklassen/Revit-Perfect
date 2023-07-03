using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Models
{
    public class ParameterModel
    {
        public String name;
        public String group;
        public String type;
        public String unitType;
        public Int32 id;
        public String guid;
        public Boolean? builtIn;
        public Boolean? variesAcrossGroups;
        public Boolean? visible;
        public String projectSharedGlobal;
        public String internalOrExternal;
        public String instanceOrType;
        public String sourceFamily;
        public List<Int32> categories;

        /// <summary>
        /// Construct a ParameterModel based on a BuiltInParameter
        /// </summary>
        /// <param name="param">A BuiltInParameter enum</param>
        public ParameterModel(BuiltInParameter param)
        {
            //GetLabelFor() throws an exception for some enumeration members
            try
            {
                name = LabelUtils.GetLabelFor(param);
            }
            catch (Exception)
            {
                name = Enum.GetName(typeof(BuiltInParameter), param);
            }
            id = (Int32)param;
            builtIn = true;
        }

        public ParameterModel(Definition def, BindingMap map)
        {
            name = def.Name;
            group = LabelUtils.GetLabelFor(def.ParameterGroup);

#if UNITTYPE //versions prior to 2021
            try
            {
                type = LabelUtils.GetLabelFor(def.ParameterType);
            }
            catch (Exception)
            {
                type = Enum.GetName(typeof(ParameterType), def.ParameterType);
            }
            try
            {
                unitType = LabelUtils.GetLabelFor(def.UnitType);
            }
            catch (Exception)
            {
                unitType = Enum.GetName(typeof(UnitType), def.UnitType);
            }
#endif
#if FORGETYPE            
            //TODO: code from Forge schema
#endif

            if (def is InternalDefinition)
            {
                internalOrExternal = "internal";
                InternalDefinition intDef = def as InternalDefinition;
                if (intDef.BuiltInParameter == BuiltInParameter.INVALID)
                {
                    builtIn = false;
                }
                else
                {
                    id = (Int32)intDef.BuiltInParameter;
                    builtIn = true;
                }
                variesAcrossGroups = intDef.VariesAcrossGroups;
                visible = intDef.Visible;
            }
            else if (def is ExternalDefinition)
            {
                internalOrExternal = "external";
                ExternalDefinition ed = def as ExternalDefinition;
            }
            else
            {
                internalOrExternal = "undefined";
            }

            Binding b = map.get_Item(def);
            if (b is ElementBinding)
            {
                ElementBinding eb = b as ElementBinding;
                if (eb is InstanceBinding)
                {
                    instanceOrType = "instance";
                }
                else if (eb is TypeBinding)
                {
                    instanceOrType = "type";
                }

                categories = new List<Int32>();
                foreach (Category cat in eb.Categories)
                {
                    categories.Add(cat.Id.IntegerValue);
                }
            }
        }

        public ParameterModel(ParameterElement param, BindingMap map)
            : this(param.GetDefinition(), map)
        {
            Definition def = param.GetDefinition();

            id = param.Id.IntegerValue;

            if (param is SharedParameterElement)
            {
                projectSharedGlobal = "shared";
                guid = ((SharedParameterElement)param).GuidValue.ToString();
            }
            else if (param is GlobalParameter)
            {
                projectSharedGlobal = "global";
            }
            else
            {
                projectSharedGlobal = "project";
            }
        }

        /// <summary>
        /// Retrieve data for a built in parameter. This is a very slow brute force method requiring iterating over
        /// all elements in the project. For much faster but incomplete data retrival,
        /// use the ParameterModel(BuiltInParameter param) constructor. This method will only complete data for parameters
        /// that are in use by at least one element in the model, so larger projects are more likely to return a full result.
        /// </summary>
        /// <param name="param">The built in parameter to export data for</param>
        /// <param name="map">A binding map from the Document</param>
        /// <param name="allElements">All elements in the project</param>
        /// <returns>Encapsulated data for the parameter</returns>
        public static ParameterModel GetBuiltInParameter(BuiltInParameter param, BindingMap map, IEnumerable<Element> allElements)
        {
            //attempt to find an element that has this parameter
            foreach (Element e in allElements)
            {
                Parameter p = e.get_Parameter(param);
                if (p != null)
                {
                    return new ParameterModel(p.Definition, map);
                }
            }
            //fallback
            return new ParameterModel(param);
        }
    }
}
