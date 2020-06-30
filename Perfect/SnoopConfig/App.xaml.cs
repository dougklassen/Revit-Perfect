using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace DougKlassen.Revit.SnoopConfigurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        public Boolean isExiting = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            notifyIcon.Icon = SnoopConfigurator.Properties.Resources.SnoopIcon;

            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            var showItem = notifyIcon.ContextMenuStrip.Items.Add("Snoop Configurator");
            showItem.Click += (s, args) => ShowMainWindow();
            var exitItem = notifyIcon.ContextMenuStrip.Items.Add("Exit");
            exitItem.Click += (s, args) => ExitApplication();

            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.Visible = true;

            MainWindow.Show();
        }

        /// <summary>
        /// Manually exit the application
        /// </summary>
        private void ExitApplication()
        {
            isExiting = true;
            MainWindow.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        /// <summary>
        /// Restore the main window from a hidden or minimized state and bring it to the front
        /// </summary>
        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            System.Drawing.Point mousePoint = Control.MousePosition;
            mousePoint.X -= 200;
            mousePoint.Y -= 60;
            notifyIcon.ContextMenuStrip.Show(mousePoint);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                MainWindow.Hide();
            }
        }
    }
}
