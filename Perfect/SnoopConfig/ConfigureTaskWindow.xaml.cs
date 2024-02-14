using DougKlassen.Revit.Snoop;
using DougKlassen.Revit.Snoop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for ConfigureTaskWindow.xaml
    /// </summary>
    public partial class ConfigureTaskWindow : Window
    {

        public ConfigureTaskWindow(SnoopTask task)
        {
            Task = task;
            InitializeComponent();
            taskTypeComboBox.ItemsSource = Helpers.GetFriendlyTaskNames();
            taskTypeComboBox.SelectedIndex = (int)Task.TaskType;
        }

        /// <summary>
        /// The task being configured
        /// </summary>
        public SnoopTask Task
        {
            get; set;
        }

        /// <summary>
        /// Creates fields for entering parameter values based on the current task units
        /// </summary>
        /// <param name="taskType">The specified task units</param>
        private void GenerateParamInterface(SnoopTaskType taskType)
        {
            ClearParamInterface();
            List<SnoopParameterType> paramTypes = SnoopTask.GetParameterTypes(taskType);
            foreach (SnoopParameterType paramType in paramTypes)
            {
                Int32 newControlIndex = -1;
                switch (paramType)
                {
                    case SnoopParameterType.FileOutputDirectory:
                        SnoopTaskParameter taskParameter = new SnoopTaskParameter();
                        taskParameter.ParameterType = paramType;
                        FileOutputPathParameterControl fileControl = new FileOutputPathParameterControl(taskParameter);
                        newControlIndex = taskParametersStackPanel.Children.Add(fileControl);
                        break;
                    default:
                        break;
                }
                //set the value if it was set on the original task
                //NOTE: this will fail if the task has two parameters of the same units
                foreach (SnoopTaskParameter existingParam in Task.TaskParameters)
                {
                    if (existingParam.ParameterType == paramType)
                    {
                        //retrieve the newly created control
                        IParameterControl newControl = taskParametersStackPanel.Children[newControlIndex] as IParameterControl;
                        newControl.TaskParameter.ParameterValue = existingParam.ParameterValue;
                    }
                }
            }
        }

        /// <summary>
        /// Remove all parameter fields from the interface
        /// </summary>
        private void ClearParamInterface()
        {
            while (taskParametersStackPanel.Children.Count > 0)
            {
                taskParametersStackPanel.Children.RemoveAt(taskParametersStackPanel.Children.Count - 1);
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            //read the values from the parameter controls
            ObservableCollection<SnoopTaskParameter> newParameters = new ObservableCollection<SnoopTaskParameter>();
            IEnumerable<IParameterControl> parameterControls = taskParametersStackPanel.Children.Cast<IParameterControl>();
            foreach (IParameterControl item in parameterControls)
            {
                newParameters.Add(item.TaskParameter);
            }
            Task.TaskParameters = newParameters;
            DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void taskTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task.TaskType = (SnoopTaskType)taskTypeComboBox.SelectedIndex;
            GenerateParamInterface(Task.TaskType);
        }
    }
}
