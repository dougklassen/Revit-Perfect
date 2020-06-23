using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DougKlassen.Revit.Snoop.Models
{
    /// <summary>
    /// An enumeration of possible task parametertypes
    /// </summary>
    public enum SnoopParameterType
    {
        FileOutputDirectory
    }

    /// <summary>
    /// A parameter for the running of a task
    /// </summary>
    public class SnoopTaskParameter : ICloneable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SnoopParameterType parameterType;
        private String parameterValue;

        /// <summary>
        /// The user friendly name for the parameter
        /// </summary>
        public String FriendlyName
        {
            get
            {
                return ParameterType.GetFriendlyName();
            }
        }

        /// <summary>
        /// The name of the parameter
        /// </summary>
        public SnoopParameterType ParameterType
        {
            get
            {
                return parameterType;
            }
            set
            {
                parameterType = value;
                OnPropertyChanged();
                OnPropertyChanged("FriendlyName"); //changing the ParameterType also changes FriendlyName
            }
        }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        public String ParameterValue
        {
            get
            {
                return parameterValue;
            }
            set
            {
                parameterValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets all values to match the source task using a deep copy
        /// </summary>
        /// <param name="source">The source SnoopTaskParameter</param>
        public void SetValues(SnoopTaskParameter source)
        {
            this.ParameterType = source.ParameterType;
            this.ParameterValue = source.ParameterValue;
        }

        /// <summary>
        /// Return a clone of the task
        /// </summary>
        /// <returns>A cloned copy of the task parameter</returns>
        public object Clone()
        {
            SnoopTaskParameter clone = new SnoopTaskParameter();

            clone.SetValues(this);

            return clone;
        }

        /// <summary>
        /// Raise the PropertyChanged event
        /// </summary>
        /// <param name="name">The name of the Property that has been updated. This is provided via
        /// the CallerMemberName attribute</param>
        protected void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}