using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Autodesk.Revit.DB;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SelectElementsWindow : Window
    {
        public SelectElementsWindow(Document dbDoc, List<Element> elements)
        {
            ElementsToChoose = elements;
            InitializeComponent();
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
