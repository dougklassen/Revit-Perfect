using System;

namespace DougKlassen.Revit.Snoop.Models
{
    public class ObjectStylesModel
    {
        public String Name { get; set; }
        public Int32? ProjectionLineweight { get; set; }
        public ColorModel LineColor { get; set; }
        public Byte Red { get; set; }
        public Byte Green { get; set; }
        public Byte Blue { get; set; }
        public Boolean Delete { get; set; }
    }
}
