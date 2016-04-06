using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;

namespace WpfMap
{
    /// <summary>
    /// Intercept binding errors if app run in debug mode. Registered in app.xml
    /// </summary>
    class BindingErrorTraceListener : TraceListener
    {
        private readonly StringBuilder _messageBuilder = new StringBuilder();

        public override void Write(string message)
        {
            _messageBuilder.Append(message);
        }

        public override void WriteLine(string message)
        {
            Write(message);

            MessageBox.Show(_messageBuilder.ToString(), "Binding error", MessageBoxButton.OK, MessageBoxImage.Error);
            _messageBuilder.Clear();
        }
    }
}
