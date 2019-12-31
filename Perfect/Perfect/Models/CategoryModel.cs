using Autodesk.Revit.DB;
using System;

namespace DougKlassen.Revit.Perfect.Models
{
    class CategoryModel
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
