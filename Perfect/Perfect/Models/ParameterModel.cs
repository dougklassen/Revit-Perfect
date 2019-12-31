using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Models
{
    public class ParameterModel
    {
        public String name;
        public String group;
        public String type;
        public String unitType;
        public Int32 id;
        public String guid;
        public String builtInParam;
        public Boolean? variesAcrossGroups;
        public Boolean? visible;
        public String projectSharedGlobal;
        public String internalOrExternal;
        public String instanceOrType;
        public String sourceFamily;
        public List<Int32> categories;

        public ParameterModel(Definition def)
        {
            name = def.Name;
            group = LabelUtils.GetLabelFor(def.ParameterGroup);
            type = LabelUtils.GetLabelFor(def.ParameterType);
            unitType = LabelUtils.GetLabelFor(def.UnitType);

            if (def is InternalDefinition)
            {
                internalOrExternal = "internal";
                InternalDefinition id = def as InternalDefinition;
                if (id.BuiltInParameter == BuiltInParameter.INVALID)
                {
                    builtInParam = null;
                }
                else
                {
                    builtInParam = LabelUtils.GetLabelFor(id.BuiltInParameter);
                }
                variesAcrossGroups = id.VariesAcrossGroups;
                visible = id.Visible;
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
        }

        public ParameterModel(ParameterElement param, BindingMap map)
            : this(param.GetDefinition())
        {
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

            Definition def = param.GetDefinition();
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
    }
}
