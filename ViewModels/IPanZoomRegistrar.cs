using System.Windows;

namespace WpfMap.ViewModels
{
    /// <summary>
    /// Registers map container in viewmodel
    /// </summary>
    interface IPanZoomRegistrar
    {
        void SubscribeFrameworkElementForPanAndZoom(FrameworkElement element);
    }
}
