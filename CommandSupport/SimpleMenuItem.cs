using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WpfMap.Properties;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfMap.CommandSupport
{
    class SimpleMenuItem : INotifyPropertyChanged
    {
        private string _header;
        private bool _isChecked;
        private object _commandParameter;
        private Visibility _visibility;

        public virtual string Header
        {
            get { return _header; }
            protected set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged("Header");
                }
            }
        }

        public virtual bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }

        public virtual object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                if (_commandParameter != value)
                {
                    _commandParameter = value;
                    OnPropertyChanged("CommandParameter");
                }
            }
        }

        public virtual Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

        public virtual ObservableCollection<SimpleMenuItem> SubMenuItems { get; protected set; }

        public SimpleCommand Command { get; private set; }

        public bool IsCheckable { get; set; }

        /// <summary>
        /// Submenus - no action!
        /// </summary>
        /// <param name="header"></param>
        public SimpleMenuItem(string header)
        {
            Header = header; // GetHeader(headerKey);
            Command = null;
            SubMenuItems = new ObservableCollection<SimpleMenuItem>();
            Visibility = Visibility.Visible;
        }

        public SimpleMenuItem(string header, Action<object> execute)
            : this(header, execute, null)
        {
        }

        public SimpleMenuItem(string header, Action<object> execute, bool isCheckable)
            : this(header, execute, null)
        {
            IsCheckable = isCheckable;
            if (isCheckable)
            {
                CommandParameter = this;
            }
        }


        /// <summary>
        /// Gets a menu header from resources and set up command
        /// </summary>
        /// <param name="headerKey">resource key for menu item</param>
        /// <param name="execute">Execute handler for a command</param>
        /// <param name="canExecute">CanExecute handler for a command</param>
        public SimpleMenuItem(string header, Action<object> execute, Predicate<object> canExecute)
        {
            Header = header; //GetHeader(headerKey);
            Command = new SimpleCommand(execute, canExecute);
            SubMenuItems = new ObservableCollection<SimpleMenuItem>();
            Visibility = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        protected string GetHeader(string headerKey)
        {
            var app = (MapApp.Current as MapApp);
            if (app != null)
            {
                return app.GetResourceString(headerKey);
            }
            return null;
        }
    }
}