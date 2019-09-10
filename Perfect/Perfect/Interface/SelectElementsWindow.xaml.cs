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
        List<Element> chosenElements;

        private SelectElementsWindow()
        {
            InitializeComponent();
        }

        public SelectElementsWindow(Document dbDoc, List<Element> elementsToChose) : this()
        {
            List<CheckBox> elementCheckBoxes = new List<CheckBox>();

            foreach (var element in elementsToChose)
            {
                CheckBox cb = new CheckBox();
                cb.Content = element;
                cb.IsChecked = true;
                elementCheckBoxes.Add(cb);
            }

            elementsListBox.ItemsSource = elementCheckBoxes;
        }
    }
}
