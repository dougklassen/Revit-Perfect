using DougKlassen.Revit.Snoop;
using DougKlassen.Revit.Snoop.Models;
using DougKlassen.Revit.Snoop.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The heading used for all message boxes displayed by this window
        /// </summary>
        private String messageBoxTitle = "Configurator";
        private FileLocations fileLocations;
        private StringBuilder sessionLog = new StringBuilder();

        public MainWindow()
        {
            fileLocations = FileLocations.Instance;
            ConfigFilePath = fileLocations.ConfigFilePath;

            Config = new SnoopConfig();
            Config.SetDefaultValues();
            LoadConfig();

            ConfigFilePath = fileLocations.ConfigFilePath;

            CanEditProject = false; //set to false until a project is selected
            CanEditTask = false; //set to false until a task is selected

            LogMessage("Snoop Configurator started");

            RefreshActiveScripts();

            InitializeComponent();
        }


        /// <summary>
        /// The active configuration
        /// </summary>
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(SnoopConfig), typeof(MainWindow));
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
        /// A log of all activity during the session
        /// </summary>
        public static readonly DependencyProperty SessionLogProperty =
            DependencyProperty.Register("SessionLog", typeof(String), typeof(MainWindow));
        public String SessionLog
        {
            get
            {
                return (String)GetValue(SessionLogProperty);
            }
            set
            {
                SetValue(SessionLogProperty, value);
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
        /// All scripts that exist as script files ready to be run
        /// </summary>
        public static readonly DependencyProperty ActiveScriptsProperty =
            DependencyProperty.Register("ActiveScripts", typeof(List<String>), typeof(MainWindow));
        public List<String> ActiveScripts
        {
            get
            {
                return (List<String>)GetValue(ActiveScriptsProperty);
            }
            set
            {
                SetValue(ActiveScriptsProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedScriptTextProperty =
            DependencyProperty.Register("SelectedScriptText", typeof(String), typeof(MainWindow));
        public String SelectedScriptText
        {
            get
            {
                return (String)GetValue(SelectedScriptTextProperty);
            }
            set
            {
                SetValue(SelectedScriptTextProperty, value);
            }
        }

        /// <summary>
        /// Add a new timestamped entry to the session log
        /// </summary>
        /// <param name="message">The message to add</param>
        private void LogMessage(String message)
        {
            sessionLog.AppendFormat("{0}: {1}\n\n", Helpers.GetTimeStamp(), message);
            SessionLog = sessionLog.ToString();
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
                    LogMessage("Couldn't parse configuration file");
                }

                LogMessage("Sucessfully loaded configuration file");
                HasUnsavedChanges = false;
            }
            catch (Exception)
            {
                LogMessage("Couldn't find configuration file");
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

                LogMessage("Sucessfully saved configuration file");
                ConfigFileContents = File.ReadAllText(ConfigFilePath);
                HasUnsavedChanges = false;
            }
            catch (Exception)
            {
                LogMessage("Couldn't write configuration file");
            }
        }

        /// <summary>
        /// Generate task script files for all versions of Revit for all active projects
        /// </summary>
        private void GenerateScripts()
        {
            LogMessage("Generating scripts for all active projects");

            Dictionary<String, SnoopScript> currentScripts = Config.GenerateScripts();
            if (currentScripts.Count == 0)
            {
                LogMessage("No tasks found for any active projects");
            }

            foreach (String version in currentScripts.Keys)
            {
                String scriptFilePath = fileLocations.GetScriptFilePathForVersion(version);
                try
                {
                    ISnoopScriptRepo repo = new SnoopScriptJsonRepo(scriptFilePath);
                    repo.WriteScript(currentScripts[version]);

                    LogMessage(String.Format("Successfully wrote script file \"{0}\"", scriptFilePath));
                }
                catch (Exception)
                {
                    LogMessage(String.Format("Couldn't write script file \"{0}\"", scriptFilePath));
                }
            }
        }

        /// <summary>
        /// Delete all active scripts
        /// </summary>
        private void ClearScripts()
        {
            foreach (String version in Helpers.GetRevitVersions())
            {
                String scriptFile = fileLocations.GetScriptFilePathForVersion(version);
                if (File.Exists(scriptFile))
                {
                    File.Delete(scriptFile);
                }
            }
        }

        /// <summary>
        /// Update the list of active script files
        /// </summary>
        private void RefreshActiveScripts()
        {
            List<String> foundScripts = new List<String>();
            foreach (String version in Helpers.GetRevitVersions())
            {
                String scriptPath = fileLocations.GetScriptFilePathForVersion(version);
                if (File.Exists(scriptPath))
                {
                    foundScripts.Add(scriptPath);
                }
            }
            ActiveScripts = foundScripts;
        }

        /// <summary>
        /// Reset all selections
        /// </summary>
        private void ResetAll()
        {
            SelectedTask = null;
            SelectedProject = null;
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Overwrite configuration with default settings?",
                messageBoxTitle,                
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                LogMessage("Overwriting with default settings");

                Config = new SnoopConfig();
                Config.SetDefaultValues();
                ConfigFileDescription = Config.GetDescription();

                ResetAll();

                HasUnsavedChanges = true;
            }
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ConfigFilePath))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Overwrite configuration file?",
                    messageBoxTitle,
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
                messageBoxTitle,
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                LoadConfig();
                ResetAll();
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
            if (projectsListBox.SelectedIndex != -1)
            {
                SelectedProject = projectsListBox.SelectedItem as SnoopProject;
                CanEditProject = true;
            }
            else
            {
                SelectedProject = null;
                CanEditProject = false;
            }
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
            if (tasksListBox.SelectedIndex != -1)
            {
                SelectedTask = tasksListBox.SelectedItem as SnoopTask;
                CanEditTask = true;
            }
            else
            {
                SelectedTask = null;
                CanEditTask = false;
            }
        }

        private void snoopConfiguratorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (HasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Save unsaved changes to configuration?", "Configurator", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    WriteConfig();
                }
            }
        }

        private void generateScriptsButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateScripts();
            RefreshActiveScripts();
            scriptsListBox.SelectedIndex = -1;
        }

        private void refreshScriptsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshActiveScripts();
        }

        private void clearScriptsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearScripts();
            RefreshActiveScripts();
        }

        private void scriptsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scriptsListBox.SelectedIndex != -1)
            {
                String scriptPath = scriptsListBox.SelectedItem as String;
                SelectedScriptText = File.ReadAllText(scriptPath);
            }
            else
            {
                SelectedScriptText = String.Empty;
            }
        }
    }
}
