using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DougKlassen.Revit.Perfect.Commands
{
    public class TemplateSelection
    {
        public Boolean IsSelected
        {
            get;
            set;
        }

        public QuantityScheduleTemplate Template
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChooseScheduleWindow : Window
    {
        public List<TemplateSelection> Templates
        {
            get;
            set;
        }

        public ChooseScheduleWindow(List<QuantityScheduleTemplate> templateSource)
        {
            Templates = new List<TemplateSelection>();
            foreach (QuantityScheduleTemplate t in templateSource)
            {
                TemplateSelection ts = new TemplateSelection();
                ts.IsSelected = false;
                ts.Template = t;
                Templates.Add(ts);
            }

            InitializeComponent();
        }

        public IEnumerable<QuantityScheduleTemplate> GetCheckedTemplates()
        {
            return Templates.Where(ts => ts.IsSelected == true).Select(ts => ts.Template);
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void selectTemplateListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuantityScheduleTemplate selectedTemplate = ((TemplateSelection)selectTemplateListBox.SelectedItem).Template;
            if (selectedTemplate != null)
            {
                descriptionTextBlock.Text = ((TemplateSelection)selectTemplateListBox.SelectedItem).Template.GetDescription();
            }
            else
            {
                descriptionTextBlock.Text = String.Empty;
            }
        }
    }
}
