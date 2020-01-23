using Autodesk.Revit.DB;
using System;

namespace DougKlassen.Revit.Snoop.Models
{
    public class CategoryModel
    {
        public String name;
        public Int32 id;

        public CategoryModel(Category cat)
        {
            name = cat.Name;
            id = cat.Id.IntegerValue;
        }
    }
}
