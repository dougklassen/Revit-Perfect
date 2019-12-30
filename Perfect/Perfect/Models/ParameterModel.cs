using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Perfect.Models
{
    public class ParameterModel
    {
        public String description;
        public String paramType;
        public List<String> categories;

        public ParameterModel(Binding binding)
        {
            description = binding.ToString();

            if(binding is InstanceBinding)
            {
                paramType = "instance";
            }
            else if(binding is TypeBinding)
            {
                paramType = "type";
            }

            ElementBinding b = binding as ElementBinding;        }
    }
}
