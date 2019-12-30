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
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for ViewCalloutVisibilityWindow.xaml
    /// </summary>
    public partial class ViewCalloutVisibilityWindow : Window
    {
        public ViewCalloutVisibilityWindow(List<List<View>> views, Int32 charCount)
        {
            ViewsToProcess = views;
            CharsToMatch = charCount;

            InitializeComponent();
        }

        public List<List<View>> ViewsToProcess
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
            this.DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
