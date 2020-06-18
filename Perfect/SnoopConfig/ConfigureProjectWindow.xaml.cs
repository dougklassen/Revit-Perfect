using DougKlassen.Revit.Snoop.Models;
using Microsoft.Win32;
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

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for ConfigureProjectWindow.xaml
    /// </summary>
    public partial class ConfigureProjectWindow : Window
    {
        private List<string> revitVersions;

        public ConfigureProjectWindow(SnoopProject project, IEnumerable<string> availableVersions)
        {
            Project = project;
            InitializeComponent();
            revitVersions = new List<string>(availableVersions);

            int versionSelection;
            if (!string.IsNullOrWhiteSpace(Project.RevitVersion))
            {
                if (!revitVersions.Contains(Project.RevitVersion))
                {
                    revitVersions.Add(Project.RevitVersion);
                }
            }
            revitVersionComboBox.ItemsSource = availableVersions;
            versionSelection = revitVersionComboBox.Items.IndexOf(Project.RevitVersion);
            revitVersionComboBox.SelectedIndex = versionSelection;
        }

        public SnoopProject Project
        {
            get; set;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void revitVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project.RevitVersion = revitVersionComboBox.SelectedItem as String;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selectDialog = new OpenFileDialog();
            selectDialog.Filter = "Revit Files|*.rvt";
            Boolean result = (Boolean) selectDialog.ShowDialog();
            if (result)
            {
                Project.FilePath = selectDialog.FileName;
            }
        }
    }
}
