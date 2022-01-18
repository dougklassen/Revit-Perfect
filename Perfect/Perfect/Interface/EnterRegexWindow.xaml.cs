using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for EnterRegexWindow.xaml
    /// </summary>
    public partial class EnterRegexWindow : Window
    {
        public Regex enteredRegex
        {
            get;
            set;
        }

        public String RegexValue
        {
            get
            {
                return (String)GetValue(RegexValueProperty);
            }
            set
            {
                String oldValue = RegexValue;
                SetValue(RegexValueProperty, value);
                OnPropertyChanged(
                    new DependencyPropertyChangedEventArgs(RegexValueProperty, oldValue, value));
            }
        }
        public static readonly DependencyProperty RegexValueProperty = DependencyProperty.Register(
            "RegexValue",
            typeof(String),
            typeof(Window),
            new UIPropertyMetadata(null));

        public EnterRegexWindow()
        {
            InitializeComponent();
        }

        private void regexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Check if the value is a valid regular expression
            try
            {
                Regex.Match(String.Empty, regexTextBox.Text);
                okButton.IsEnabled = true;
                statusLabel.Content = "Valid regular expression";
            }
            catch (ArgumentException)
            {
                okButton.IsEnabled = false;
                statusLabel.Content = "Invalid regular expression";
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
