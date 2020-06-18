﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
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
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using DougKlassen.Revit.Snoop;

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            FileLocations fileLocations = FileLocations.Instance;
            ConfigFilePath = fileLocations.ConfigFilePath;

            Config = new SnoopConfig();
            Config.SetDefaultValues();
            LoadConfig();

            ConfigFilePath = fileLocations.ConfigFilePath;

            InitializeComponent();
        }

        /// <summary>
        /// The active configuration
        /// </summary>
        public SnoopConfig Config
        {
            get;
            set;
        }

        /// <summary>
        /// The path to the configuration file
        /// </summary>
        public static readonly DependencyProperty ConfigFilePathProperty =
            DependencyProperty.Register("ConfigFilePath", typeof(String), typeof(MainWindow));
        public String ConfigFilePath
        {
            get
            {
                return (String)GetValue(ConfigFilePathProperty);
            }
            set
            {
                SetValue(ConfigFilePathProperty, value);
            }
        }

        /// <summary>
        /// Load status of the config file
        /// </summary>
        public static readonly DependencyProperty ConfigFileStatusProperty =
            DependencyProperty.Register("ConfigFileStatus", typeof(String), typeof(MainWindow));
        public String ConfigFileStatus
        {
            get
            {
                return (String)GetValue(ConfigFileStatusProperty);
            }
            set
            {
                SetValue(ConfigFileStatusProperty, value);
            }
        }

        /// <summary>
        /// Does the config file have unsaved changes
        /// </summary>
        public static readonly DependencyProperty HasUnsavedChangesProperty =
            DependencyProperty.Register("HasUnsavedChanges", typeof(Boolean), typeof(MainWindow));
        public Boolean HasUnsavedChanges
        {
            get
            {
                return (Boolean)GetValue(HasUnsavedChangesProperty);
            }
            set
            {
                SetValue(HasUnsavedChangesProperty, value);
            }
        }

        /// <summary>
        /// The text contents of the configuration file
        /// </summary>
        public static readonly DependencyProperty ConfigFileContentsProperty =
            DependencyProperty.Register("ConfigFileContents", typeof(String), typeof(MainWindow));
        public String ConfigFileContents
        {
            get
            {
                return (String)GetValue(ConfigFileContentsProperty);
            }
            set
            {
                SetValue(ConfigFileContentsProperty, value);
            }
        }

        /// <summary>
        /// A user friendly description of the current configuration
        /// </summary>
        public static readonly DependencyProperty ConfigFileDescriptionProperty =
            DependencyProperty.Register("ConfigFileDescription", typeof(String), typeof(MainWindow));
        public String ConfigFileDescription
        {
            get
            {
                return (String)GetValue(ConfigFileDescriptionProperty);
            }
            set
            {
                SetValue(ConfigFileDescriptionProperty, value);
            }
        }

        /// <summary>
        /// The project that is currently selected in the project pane
        /// </summary>
        public static readonly DependencyProperty SelectedProjectProperty =
            DependencyProperty.Register("SelectedProject", typeof(SnoopProject), typeof(MainWindow));
        public SnoopProject SelectedProject
        {
            get
            {
                return (SnoopProject)GetValue(SelectedProjectProperty);
            }
            set
            {
                SetValue(SelectedProjectProperty, value);
            }
        }

        /// <summary>
        /// The task that is currently selected in the task pane
        /// </summary>
        public static readonly DependencyProperty SelectedTaskProperty =
            DependencyProperty.Register("SelectedTask", typeof(SnoopTask), typeof(MainWindow));
        public SnoopTask SelectedTask
        {
            get
            {
                return (SnoopTask)GetValue(SelectedTaskProperty);
            }
            set
            {
                SetValue(SelectedTaskProperty, value);
            }
        }

        /// <summary>
        /// Read the config file and parse the contents into a new SnoopConfig object
        /// </summary>
        private void LoadConfig()
        {

            try
            {
                ConfigFileContents = File.ReadAllText(ConfigFilePath);
                ConfigFileDescription = Config.GetDescription();

                try
                {
                    ISnoopConfigRepo repo = new SnoopConfigJsonRepo(ConfigFilePath);
                    Config = repo.LoadConfig();
                }
                catch (Exception)
                {
                    ConfigFileStatus = "Couldn't parse configuration file";
                }

                ConfigFileStatus = "Sucessfully loaded configuration file";
                HasUnsavedChanges = false;
            }
            catch (Exception)
            {
                ConfigFileStatus = "Couldn't find configuration file";
            }
        }

        /// <summary>
        /// Write current configuration to config file
        /// </summary>
        private void WriteConfig()
        {
            try
            {
                ISnoopConfigRepo repo = new SnoopConfigJsonRepo(ConfigFilePath);
                repo.WriteConfig(Config);

                ConfigFileStatus = "Sucessfully saved configuration file";
                ConfigFileContents = File.ReadAllText(ConfigFilePath);
                HasUnsavedChanges = false;
            }
            catch (Exception)
            {
                ConfigFileStatus = "Couldn't write configuration file";
            }
        }

//TODO: add warning dialogs about overwriting config files
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            Config = new SnoopConfig();
            Config.SetDefaultValues();
            ConfigFileDescription = Config.GetDescription();

            HasUnsavedChanges = true;
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            WriteConfig();
            HasUnsavedChanges = false;
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo pi = new ProcessStartInfo(ConfigFilePath);
            pi.UseShellExecute = true;
            pi.WorkingDirectory = Path.GetDirectoryName(ConfigFilePath);
            pi.Verb = "OPEN";
            try
            {
                Process.Start(pi);
            }
            catch
            {
                MessageBox.Show("Couldn't open configuration file for editing");
            }
        }

        private void projectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProject = projectsListBox.SelectedItem as SnoopProject;
        }

        private void editProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SnoopProject editProject = SelectedProject.Clone() as SnoopProject;
            ConfigureProjectWindow configureWindow
                = new ConfigureProjectWindow(editProject, Config.RevitFilePaths.Keys);
            Boolean result = (Boolean)configureWindow.ShowDialog();
            if (result)
            {
                SelectedProject.SetValues(editProject);
                HasUnsavedChanges = true;
            }
        }

        private void addProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SnoopProject newProject = new SnoopProject();
            newProject.ProjectName = "Project " + (Config.ActiveProjects.Count + 1).ToString();
            ConfigureProjectWindow configureWindow = new ConfigureProjectWindow(newProject, Config.RevitFilePaths.Keys);
            Boolean result = (Boolean) configureWindow.ShowDialog();
            if (result)
            {
                Config.ActiveProjects.Add(newProject);
                HasUnsavedChanges = true;
            }
        }

        private void removeProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Config.ActiveProjects.Remove(SelectedProject);
            HasUnsavedChanges = true;
        }

        private void editTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void removeTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
