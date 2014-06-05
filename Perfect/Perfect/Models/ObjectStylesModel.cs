using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Autodesk.Revit.DB;

namespace DougKlassen.Revit.Perfect.Models
{
    [DataContract]
    public class ObjectStylesModel
    {
        [DataMember(Order = 1)]
        public String Name
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public Int32? ProjectionLineweight
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public Color LineColor
        {
            get
            {
                return new Color(Red, Green, Blue);
            }
            set
            {
                Red = value.Red;
                Green = value.Green;
                Blue = value.Blue;
            }
        }

        [DataMember(Order = 3)]
        public Byte Red
        {
            get;
            set;
        }

        [DataMember(Order = 4)]
        public Byte Green
        {
            get;
            set;
        }

        [DataMember(Order = 5)]
        public Byte Blue
        {
            get;
            set;
        }

        [DataMember(Order = 6)]
        public Boolean Delete
        {
            get;
            set;
        }
    }
}
