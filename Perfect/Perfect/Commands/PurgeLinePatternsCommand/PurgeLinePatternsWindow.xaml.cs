using System;
using System.Collections.Generic;
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

namespace DougKlassen.Revit.Perfect.Commands.PurgeLinePatternsCommand
{
    /// <summary>
    /// Interaction logic for PurgeLinePatternsWindow.xaml
    /// </summary>
    public partial class PurgeLinePatternsWindow : Window
    {
        private List<String> purgeRegExStrings;
        private IEnumerable<Element> docElements;

        public IEnumerable<String> DocPatternNames
        {
            get
            {
                Regex regEx;
                List<String> matchingElementNames = new List<String>();
                foreach (String purgeRegExString in purgeRegExStrings)
                {
                    //If the regex is invalid, an ArgumentException will be thrown and the foreach will continue without using it
                    try
                    {
                        regEx = new Regex(purgeRegExString);
                        matchingElementNames.AddRange(
                            docElements.Select(e => e.Name).Where(n => regEx.IsMatch(n)));
                    }
                    catch
                    {
                        continue;
                    }
                }

                return matchingElementNames;
            }
        }


        private PurgeLinePatternsWindow()
        {
            InitializeComponent();
        }

        public PurgeLinePatternsWindow(Document dbDoc, Type elementType) : this()
        {
            purgeRegExStrings = new List<String>();
            purgeRegExStrings.Add("^IMPORT-.*");

            docElements = new FilteredElementCollector(dbDoc).OfClass(elementType).AsEnumerable();

            System.Windows.Data.Binding listBinding = new System.Windows.Data.Binding();
            listBinding.Source = DocPatternNames;
        }
    }
}
