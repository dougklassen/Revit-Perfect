using System;
using System.Windows;

namespace DougKlassen.Revit.Perfect.Interface
{
    /// <summary>
    /// Interaction logic for AddCommentWindow.xaml
    /// </summary>
    public partial class AddCommentWindow : Window
    {
        public AddCommentWindow()
        {
            InitializeComponent();
        }

        public String CommentsToAdd
        {
            get;
            set;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
