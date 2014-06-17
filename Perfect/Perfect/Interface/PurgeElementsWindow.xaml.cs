using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

using Autodesk.Revit.DB;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for PurgeLinePatternsWindow.xaml
    /// </summary>
    public partial class PurgeElementsWindow : Window
    {
        private IEnumerable<Element> docElements;

        //whether or not to run a purge on the matching element once the dialog closes
        public Boolean DoPurge = false;

        //TODO: does it really need to be a DependencyProperty?
        //public String PurgeRegExString;
        public String PurgeRegExString
        {
            get { return (String)GetValue(PurgeRegExStringProperty); }
            set
            {
                String oldValue = PurgeRegExString;
                SetValue(PurgeRegExStringProperty, value);
                OnPropertyChanged(
                    new DependencyPropertyChangedEventArgs(PurgeRegExStringProperty, oldValue, value));
            }
        }
        public static readonly DependencyProperty PurgeRegExStringProperty =
            DependencyProperty.Register(
            "PurgeRegExString",
            typeof(String),
            typeof(Window),
            new UIPropertyMetadata(null));

        private PurgeElementsWindow()
        {
            InitializeComponent();
        }

        public PurgeElementsWindow(Document dbDoc, Type elementType) : this()
        {
            docElements = new FilteredElementCollector(dbDoc).OfClass(elementType).AsEnumerable();

            MatchingElementsListBox.ItemsSource = GetMatchingDocLinePatternNames();
        }

        public IEnumerable<String> GetMatchingDocLinePatternNames()
        {
            Regex regEx;

            try
            {
                regEx = new Regex(PurgeRegExString);
                return docElements.Select(e => e.Name).Where(n => regEx.IsMatch(n));
            }
            catch
            {
                return new List<String>();
            }
        }

        private void PurgeRegExTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<String> matchingLinePatterns = GetMatchingDocLinePatternNames();
            PurgeElementCountLabel.Content = 
                1 == matchingLinePatterns.Count() ?
                matchingLinePatterns.Count() + " element will be purged" :
                matchingLinePatterns.Count() + " elements will be purged";
            MatchingElementsListBox.ItemsSource = matchingLinePatterns;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
