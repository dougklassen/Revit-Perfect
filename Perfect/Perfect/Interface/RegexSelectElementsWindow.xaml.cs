using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for RegexSelectElementsWindow.xaml
    /// </summary>
    public partial class RegexSelectElementsWindow : Window
    {
        private IEnumerable<Element> docElements;

        //TODO: does it really need to be a DependencyProperty?
        public String SelectRegExString
        {
            get
            {
                return (String)GetValue(SelectRegExStringProperty);
            }
            set
            {
                String oldValue = SelectRegExString;
                SetValue(SelectRegExStringProperty, value);
                OnPropertyChanged(
                    new DependencyPropertyChangedEventArgs(SelectRegExStringProperty, oldValue, value));
            }
        }
        public static readonly DependencyProperty SelectRegExStringProperty = DependencyProperty.Register(
            "SelectRegExString",
            typeof(String),
            typeof(Window),
            new UIPropertyMetadata(null));

        private RegexSelectElementsWindow()
        {
            InitializeComponent();
        }

        public RegexSelectElementsWindow(Document dbDoc, Type elementType) : this()
        {
            docElements = new FilteredElementCollector(dbDoc).OfClass(elementType).AsEnumerable();

            MatchingElementsListBox.ItemsSource = GetMatchingDocElementNames();
        }

        public IEnumerable<String> GetMatchingDocElementNames()
        {
            Regex regEx;

            try
            {
                regEx = new Regex(SelectRegExString);
                return docElements.Select(e => e.Name).Where(n => regEx.IsMatch(n));
            }
            catch
            {
                return new List<String>();
            }
        }

        private void SelectRegExTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<String> matchingElements = GetMatchingDocElementNames();
            PurgeElementCountLabel.Content =
                1 == matchingElements.Count() ?
                matchingElements.Count() + " element selected" :
                matchingElements.Count() + " elements selected";
            MatchingElementsListBox.ItemsSource = matchingElements;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
