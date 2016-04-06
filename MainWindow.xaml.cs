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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMap
{
    /// <summary>
    /// All interaction logic for MainWindow.xaml in viewmodel
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (imgMap is Image) // byPass initial initialization
            {
                var registrar = (e.NewValue as ViewModels.IPanZoomRegistrar);
                if (registrar != null)
                {
                    registrar.SubscribeFrameworkElementForPanAndZoom(imgMap);
                }
            }
        }

        public void ItemClick(object sender, MouseEventArgs e)
        {
            var x = e.LeftButton;
        }

    }
}
