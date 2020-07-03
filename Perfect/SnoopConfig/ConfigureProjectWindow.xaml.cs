using DougKlassen.Revit.Snoop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            //if the project has an unknown version add it to the list
            if (!string.IsNullOrWhiteSpace(Project.RevitVersion))
            {
                if (!revitVersions.Contains(Project.RevitVersion))
                {
                    revitVersions.Add(Project.RevitVersion);
                }
            }

            revitVersionComboBox.ItemsSource = availableVersions;
            //find the index of the projects current version
            versionSelection = revitVersionComboBox.Items.IndexOf(Project.RevitVersion);
            //if no selection has been found yet, set it to the first item
            if (versionSelection == -1 && revitVersionComboBox.HasItems)
            {
                versionSelection = 0;
            }
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

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog selectDialog = new Microsoft.Win32.OpenFileDialog();
            selectDialog.Filter = "Revit Files|*.rvt";
            Boolean result = (Boolean) selectDialog.ShowDialog();
            if (result)
            {
                Project.FilePath = selectDialog.FileName;
            }
        }
    }
}
