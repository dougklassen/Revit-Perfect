using DougKlassen.Revit.Snoop;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

            CanEditProject = false; //set to false until a project is selected
            CanEditTask = false; //set to false until a task is selected

            InitializeComponent();
        }

        /// <summary>
        /// The active configuration
        /// </summary>
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("ConfigProperty", typeof(SnoopConfig), typeof(MainWindow));
        public SnoopConfig Config
        {
            get
            {
                return (SnoopConfig)GetValue(ConfigProperty);
            }
            set
            {
                SetValue(ConfigProperty, value);
            }
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
        /// Whether an editable project is selected as the SelectedProject
        /// </summary>
        public static readonly DependencyProperty CanEditProjectProperty =
            DependencyProperty.Register("CanEditProject", typeof(Boolean), typeof(MainWindow));
        public Boolean CanEditProject
        {
            get
            {
                return (Boolean)GetValue(CanEditProjectProperty);
            }
            set
            {
                SetValue(CanEditProjectProperty, value);
            }
        }

        /// <summary>
        /// Whether an editable task is selected as the SelectedTask
        /// </summary>
        public static readonly DependencyProperty CanEditTaskProperty =
            DependencyProperty.Register("CanEditTask", typeof(Boolean), typeof(MainWindow));
        public Boolean CanEditTask
        {
            get
            {
                return (Boolean)GetValue(CanEditTaskProperty);
            }
            set
            {
                SetValue(CanEditTaskProperty, value);
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

        /// <summary>
        /// Reset all selections and reset the project list
        /// </summary>
        private void RefreshAll()
        {
            SelectedTask = null;
            SelectedProject = null;
            //ItemsSource property needs to be manually updated because the binding still points to the property of theold config object
            projectsListBox.ItemsSource = Config.ActiveProjects; 
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Overwrite configuration with default settings?",
                "Overwrite Settings",                
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Config = new SnoopConfig();
                Config.SetDefaultValues();
                ConfigFileDescription = Config.GetDescription();

                RefreshAll();

                HasUnsavedChanges = true;
            }
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ConfigFilePath))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Overwrite configuration file?",
                    "Save Configuration",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    WriteConfig();
                    HasUnsavedChanges = false;
                } 
            }
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Load settings from configuration file?",
                "Load Configuration",
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                LoadConfig();

                RefreshAll();
            }
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
            CanEditProject = true;
        }

        private void editProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SnoopProject editProject = SelectedProject.Clone() as SnoopProject;
            ConfigureProjectWindow configureWindow =
                new ConfigureProjectWindow(editProject, Config.RevitFilePaths.Keys);
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
            SnoopTask editTask = SelectedTask.Clone() as SnoopTask;
            ConfigureTaskWindow configureWindow =
                new ConfigureTaskWindow(editTask);
            Boolean result = (Boolean)configureWindow.ShowDialog();
            if (result)
            {
                SelectedTask.SetValues(editTask);
                HasUnsavedChanges = true;
            }
        }

        private void addTaskButton_Click(object sender, RoutedEventArgs e)
        {
            SnoopTask task = new SnoopTask();
            ConfigureTaskWindow window = new ConfigureTaskWindow(task);
            Boolean result = (Boolean)window.ShowDialog();
            if (result)
            {
                SelectedProject.TaskList.Add(task);
                HasUnsavedChanges = true;
            }
        }

        private void removeTaskButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedProject.TaskList.Remove(SelectedTask);
            HasUnsavedChanges = true;
        }

        private void tasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTask = tasksListBox.SelectedItem as SnoopTask;
            CanEditTask = true;
        }
    }
}
