using DougKlassen.Revit.Snoop.Models;
using System;
using System.Windows.Controls;

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

        private void browseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog selectDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = selectDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TaskParameter.ParameterValue = selectDialog.SelectedPath;
            }
        }
    }
}
