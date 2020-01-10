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

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            String configFilePath = Directory.GetCurrentDirectory() + SnoopConfig.configFileName;

            try
            {
                SnoopConfigJsonRepo repo = new SnoopConfigJsonRepo(configFilePath);
                Config = repo.LoadConfig();

            }
            catch (Exception)
            {
                Config = new SnoopConfig();
                Config.SetDefaultValues(Directory.GetCurrentDirectory());
            }
            InitializeComponent();
        }

        SnoopConfig Config { get; set; }
    }
}
