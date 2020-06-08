using System;
using System.IO;
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
        private FileLocations fileLocations = FileLocations.Instance;

        public MainWindow()
        {
            Config = new SnoopConfig();
            Config.SetDefaultValues();
            LoadConfig();

            ConfigFilePath = fileLocations.ConfigFilePath;

            InitializeComponent();
        }

        /// <summary>
        /// The path to the configuration file
        /// </summary>
        public String ConfigFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Load status of the config file
        /// </summary>
        public String ConfigFileStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Does the config file have unsaved changes
        /// </summary>
        public Boolean HasUnsavedChanges
        {
            get;
            set;
        }

        /// <summary>
        /// The text contents of the configuration file
        /// </summary>
        public String ConfigFileContents
        {
            get;
            set;
        }

        /// <summary>
        /// A user friendly description of the current configuration
        /// </summary>
        public String ConfigFileDescription
        {
            get
            {
                return Config.GetDescription();
            }
        }

        /// <summary>
        /// The active configuration
        /// </summary>
        public SnoopConfig Config
        {
            get;
            set;
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            Config = new SnoopConfig();
            Config.SetDefaultValues();



            HasUnsavedChanges = true;
        }

        private void writeButton_Click(object sender, RoutedEventArgs e)
        {
            HasUnsavedChanges = false;
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        /// <summary>
        /// Read the config file and parse the contents into a new SnoopConfig object
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                ConfigFileContents = File.ReadAllText(fileLocations.ConfigFilePath);

                try
                {
                    SnoopConfigJsonRepo repo = new SnoopConfigJsonRepo(fileLocations.ConfigFilePath);
                    Config = repo.LoadConfig();
                }
                catch (Exception)
                {
                    ConfigFileStatus = "Couldn't parse config file";
                }
            }
            catch (Exception)
            {
                ConfigFileStatus = "Couldn't find config file";
            }

            HasUnsavedChanges = false;
        }
    }
}
