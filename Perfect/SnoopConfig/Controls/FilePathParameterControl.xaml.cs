using DougKlassen.Revit.Snoop.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for FilePathParameterControl.xaml
    /// </summary>
        public partial class FileOutputPathParameterControl : UserControl, IParameterControl
    {
        public FileOutputPathParameterControl(SnoopTaskParameter taskParameter)
        {
            TaskParameter = taskParameter;
            InitializeComponent();
        }

        public SnoopTaskParameter TaskParameter { get; set; }
    }
}
