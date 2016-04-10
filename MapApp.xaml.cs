using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace WpfMap
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class MapApp : Application
    {
        /// <summary>
        /// Run during app startup. Register binding errors interception.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // base start up
            base.OnStartup(e);
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener());
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;
            

            // splash screen
            var splash = new Splash();
            splash.Topmost = true;
            splash.Show();

            // preload image before showing the main screen
            var model = new ViewModels.MainViewModel();
            model.Prepare();

            MainWindow = new MainWindow
            {
                DataContext = model,
                WindowState = WindowState.Maximized,                                
            };
            MainWindow.Loaded += new RoutedEventHandler(model.OnWindowLoad);
            MainWindow.Show();

            splash.Close(); // if closed before, will close application
        }

        public string GetResourceString(string key)
        {
            string res = null;
            try
            {
                res = WpfMap.Properties.Resources.ResourceManager.GetString(key, WpfMap.Properties.Resources.Culture);
                if (string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Empty resource string with key: " + key);
                }
            }
            catch (ResourceReferenceKeyNotFoundException rex)
            {
                MessageBox.Show("Can't find resource string with key: " + rex.Key);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error retrieving resource wiht key {0}: {1}", key, ex.Message));
            }
            return res;
        }

        public static void StartExternalPresentation(string filename)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentPath, "files", filename);
            var pi = new ProcessStartInfo(path);
            pi.Verb = "Open";
            pi.UseShellExecute = true;
            pi.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(pi);
        }
    }
}
