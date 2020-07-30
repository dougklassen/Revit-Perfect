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

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FilterCalloutsWindow : Window
    {
        public FilterCalloutsWindow(List<View> views)
        {
            ViewsToProcess = views;

            InitializeComponent();
        }

        public List<View> ViewsToProcess
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
