using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DougKlassen.Revit.Snoop;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Snoop.Models
{
    public class ParameterModel
    {
        /// <summary>
        /// The display name of the parameter
        /// </summary>
        public String name;
        /// <summary>
        /// The internal label Revit uses for this parameter in the BuiltInParameter enum 
        /// </summary>
        public String builtInName;
        /// <summary>
        /// The group the parameter belongs to. This will be the collapsible subsection it is listed under
        /// in the properties pallete.
        /// </summary>
        public String group;
        /// <summary>
        /// The user friendly laber for the parameter type
        /// formerly type, the value corresponding to LabelUtils.GetLabelFor(def.ParameterType)
        /// Had value such as "Text", "Yes/No", "Length"
        /// </summary>
        public String units;
        /// <summary>
        /// The id of the parameter definition. This corresponds to the Enum value for builtin parameters
        /// </summary>
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
        /// <param name="builtInParam">A BuiltInParameter enum</param>
        public ParameterModel(BuiltInParameter builtInParam)
        {
            //GetLabelFor() throws an exception for some enumeration members
            try
            {
                name = LabelUtils.GetLabelFor(builtInParam);
            }
            catch (Exception)
            {
                name = null;
            }
            builtInName = Enum.GetName(typeof(BuiltInParameter), builtInParam);
            id = (Int32)builtInParam;
            builtIn = true;
        }

        /// <summary>
        /// Construct a ParameterModel based on a parameter Definition
        /// </summary>
        /// <param name="def"></param>
        /// <param name="map"></param>
        public ParameterModel(Definition def, BindingMap map)
        {
            name = def.Name;
            group = LabelUtils.GetLabelFor(def.ParameterGroup);

#if UNITTYPE //versions prior to 2021
            try
            {
                units = LabelUtils.GetLabelFor(def.ParameterType);
            }
            catch (Exception)
            {
                units = Enum.GetName(typeof(ParameterType), def.ParameterType);
            }
#endif
#if FORGETYPE2021 //version 2021 only
            ForgeTypeId forgeId = def.GetSpecTypeId();
            if (UnitUtils.IsMeasurableSpec(forgeId))
            {
                units = UnitUtils.GetTypeCatalogStringForSpec(forgeId);
            }
#endif
#if FORGETYPE //versions 2022 and higher
            ForgeTypeId forgeId = def.GetDataType();
            if (UnitUtils.IsMeasurableSpec(forgeId))
            {
                units = UnitUtils.GetTypeCatalogStringForSpec(forgeId);
            }
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
        /// <param name="builtInParam">The built in parameter to export data for</param>
        /// <param name="map">A binding map from the Document</param>
        /// <param name="allElements">All elements in the project</param>
        /// <returns>Encapsulated data for the parameter</returns>
        public static ParameterModel GetBuiltInParameter(BuiltInParameter builtInParam, BindingMap map, IEnumerable<Element> allElements)
        {
            //TODO: Is there another way to do this? I haven't found a way to access a parameter definition directly using the BuiltInParam enum value
            //attempt to find an element that has this parameter
            foreach (Element e in allElements)
            {
                Parameter p = e.get_Parameter(builtInParam);
                if (p != null)
                {
                    ParameterModel pm = new ParameterModel(p.Definition, map);
                    pm.builtInName = Enum.GetName(typeof(BuiltInParameter), builtInParam);
                    return pm;
                }
            }
            //fallback
            return new ParameterModel(builtInParam);
        }
    }
}
