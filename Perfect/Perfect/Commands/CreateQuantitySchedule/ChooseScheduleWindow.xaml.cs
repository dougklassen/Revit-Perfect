using System;
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
using System.Windows.Shapes;

namespace DougKlassen.Revit.Perfect.Commands
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChooseScheduleWindow : Window
    {
        public List<QuantityScheduleTemplate> Templates { get; set; }

        public ChooseScheduleWindow(List<QuantityScheduleTemplate> templateSource)
        {
            Templates = templateSource;
            InitializeComponent();
        }

        public IEnumerable<QuantityScheduleTemplate> GetCheckedTemplates()
        {
            List<QuantityScheduleTemplate> checkedTemplates = new List<QuantityScheduleTemplate>();

            foreach (ListViewItem item in selectTemplateListView.ItemsSource)
            {
                CheckBox check = item.TemplatedParent as CheckBox;
                if(check != null && check.IsChecked.Value)
                {
                    checkedTemplates.Add(item.DataContext as QuantityScheduleTemplate);
                }
            }

            return checkedTemplates;
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
            QuantityScheduleTemplate selectedTemplate = (QuantityScheduleTemplate)selectTemplateListView.SelectedItem;
            if (selectedTemplate != null)
            {
                descriptionTextBlock.Text = ((QuantityScheduleTemplate)selectTemplateListView.SelectedItem).GetDescription();
            }
            else
            {
                descriptionTextBlock.Text = String.Empty;
            }
        }
    }
}
