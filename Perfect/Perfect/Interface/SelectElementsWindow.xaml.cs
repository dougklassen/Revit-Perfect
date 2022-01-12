using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SelectElementsWindow : Window
    {
        public SelectElementsWindow(List<Element> elements, Boolean selectAll)
        {
            ElementsToChoose = elements;

            InitializeComponent();

            if (selectAll)
            {
                elementsListBox.SelectAll();
            }
        }

        public List<Element> ElementsToChoose
        {
            get;
            set;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ElementsToChoose = elementsListBox.SelectedItems.Cast<Element>().ToList();

            this.DialogResult = true;
            this.Close();
        }
    }
}
