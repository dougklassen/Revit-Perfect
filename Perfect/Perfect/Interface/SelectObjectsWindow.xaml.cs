using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Wrapper class for a tag that tracks whether it is selected
    /// </summary>
    public class ObjectSelection : IComparable
    {
        public ObjectSelection(Object obj, Boolean objectSelected)
        {
            Value = obj;
            IsSelected = objectSelected;
        }

        /// <summary>
        /// Whether the object is selected
        /// </summary>
        public Boolean IsSelected { get; set; }

        /// <summary>
        /// The object encapsulated in the ObjectSelection
        /// </summary>
        public Object Value { get; set; }

        /// <summary>
        /// An encapsulation of Object.ToString()
        /// </summary>
        public String Description
        {
            get
            {
                return Value.ToString();
            }
        }

        public int CompareTo(object obj)
        {
            return Description.CompareTo(obj.ToString());
        }
    }

    /// <summary>
    /// Interaction logic for SelectTagsWindow.xaml
    /// </summary>
    public partial class SelectObjectsWindow : Window
    {
        /// <summary>
        /// Create a new window representing a collection of objects
        /// </summary>
        /// <param name="objects">The collection of objects</param>
        /// <param name="selectAll">Whether all objects are checked by default</param>
        public SelectObjectsWindow(List<Object> objects, Boolean selectAll)
            : this(objects, selectAll, null, null)
        {
        }

        /// <summary>
        /// Create a new window representing a collection of objects
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="selectAll"></param>
        /// <param name="title"></param>
        public SelectObjectsWindow(
            IEnumerable<Object> objects,
            Boolean selectAll,
            String title,
            String message)
        {
            ObjectList = new List<ObjectSelection>();
            foreach (Object obj in objects)
            {
                ObjectList.Add(new ObjectSelection(obj, selectAll));
            }
            ObjectList.Sort();

            if (title != null)
            {
                Title = title;
            }

            if (String.IsNullOrWhiteSpace(message))
            {
                instructionsTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                Instructions = message;
            }

            InitializeComponent();
        }

        public List<ObjectSelection> ObjectList
        {
            get;
            set;
        }

        /// <summary>
        /// All the tags that are currently selected in the window
        /// </summary>
        public List<Object> SelectedObjects
        {
            get
            {
                List<Object> selectedObjects = new List<Object>();
                foreach (ObjectSelection obj in ObjectList)
                {
                    if (obj.IsSelected)
                    {
                        selectedObjects.Add(obj.Value);
                    }
                }
                return selectedObjects;
            }
        }

        /// <summary>
        /// The instructional message displayed at the top of the dialog
        /// </summary>
        public String Instructions
        {
            get;
            set;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
