using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FilterCalloutsWindow : Window
    {
        public FilterCalloutsWindow(List<ViewSheet> sheets)
        {
            SheetsToProccess = sheets;

            InitializeComponent();

            MessageTextBox.Text = String.Format("{0} sheets selected", sheets.Count);
        }

        public List<ViewSheet> SheetsToProccess
        {
            get;
            set;
        }

        public Int32 CharsToMatch
        {
            get;
            set;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
