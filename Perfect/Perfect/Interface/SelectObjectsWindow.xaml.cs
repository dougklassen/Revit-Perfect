using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Wrapper class for an object that tracks whether it is selected by the user and provides
    /// properties to be bound to the controls of the SelectObjectsWindow
    /// </summary>
    public class ObjectSelection : IComparable
    {
        /// <summary>
        /// Create an instance of ObjectSelection
        /// </summary>
        /// <param name="obj">The object to be encapsulated</param>
        /// <param name="objectSelected">Whether the object has been selected</param>
        public ObjectSelection(Object obj, Boolean objectSelected)
        {
            Value = obj;
            IsSelected = objectSelected;
        }

        /// <summary>
        /// Whether the object is selected, to be bound to the IsChecked property of the Checkbox
        /// </summary>
        public Boolean IsSelected { get; set; }

        /// <summary>
        /// The object encapsulated in the ObjectSelection
        /// </summary>
        public Object Value { get; set; }

        /// <summary>
        /// An encapsulation of Object.ToString() for use in binding as the text of the Checkbox
        /// </summary>
        public String Description
        {
            get
            {
                return Value.ToString();
            }
        }

        /// <summary>
        /// Use ToString() as the basis of CompareTo()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The value of the ToString() comparison</returns>
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

        /// <summary>
        /// The object that the user has been prompted to select from
        /// </summary>
        public List<ObjectSelection> ObjectList
        {
            get;
            set;
        }

        /// <summary>
        /// All the tags that are currently selected in the window by the user by checking their CheckBox
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

        private void allButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in tagListBox.Items)
            {
                CheckBox checkBox = GetCheckboxControl(item);
                checkBox.IsChecked = true;
            }
        }

        private void noneButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Object item in tagListBox.Items)
            {
                CheckBox checkBox = GetCheckboxControl(item);
                checkBox.IsChecked = false;
            }
        }

        /// <summary>
        /// Access the CheckBox defined by the DataTemplate of the list Item
        /// </summary>
        /// <param name="sourceObject">A list Item contraining a CheckBox created by a DataTemplate</param>
        /// <returns>The CheckBox control</returns>
        private CheckBox GetCheckboxControl(Object sourceObject)
        {
            CheckBox checkBox;
            DependencyObject listItem = tagListBox.ItemContainerGenerator.ContainerFromItem(sourceObject);
            ContentPresenter presenter = FindVisualChild<ContentPresenter>(listItem);
            DataTemplate template = presenter.ContentTemplate;
            checkBox = template.FindName("selectCheckBox", presenter) as CheckBox;
            return checkBox;
        }

        /// <summary>
        /// Find a child of an element of the specified type
        /// </summary>
        /// <typeparam name="childItem">The type of the child to look for</typeparam>
        /// <param name="parent">The parent element to search</param>
        /// <returns>The specified child of the parent element</returns>
        private childItem FindVisualChild<childItem>(DependencyObject parent)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is childItem)
                {
                    return child as childItem;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}
