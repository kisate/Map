using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using WpfMap.Models;
using WpfMap.CommandSupport;

namespace WpfMap.ViewModels
{
    sealed class MapDataItemVM: INotifyPropertyChanged
    {
        private double _x, _y;
        private Point _realCoords;
        private string _title, _description;
        private MapItemViewStateType _state;
        private bool _trackChanges;

 
        /// <summary>
        /// Has changes 
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// Command on clicking on a circle
        /// </summary>
        public SimpleCommand Activate { get; set; }

        /// <summary>
        /// Is actively shown
        /// </summary>
        public MapItemViewStateType State
        {
            get { 
                return _state; 
            }
            set 
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }

        #region Data related properties
        public Point RealCoords
        {
            get { return _realCoords; }
            set
            {
                if (_realCoords != value)
                {
                    _realCoords = value;
                    if (_trackChanges)
                    {
                        IsChanged = true;
                    }
                }
            }
        }

        public double X
        {
            get { 
                return _x; 
            }
            set {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get { return _y; }
            set {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged("Y");
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set 
            {
                if (_title != value)
                {
                    _title = value;
                    if (_trackChanges)
                    {
                        IsChanged = true;
                    }
                    OnPropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set 
            {
                if (_description != value)
                {
                    _description = value;
                    if (_trackChanges)
                    {
                        IsChanged = true;
                    }
                    OnPropertyChanged("Description");
                }
            }
        }
        #endregion

        public MapDataItemVM(double x_image, double y_image, double x, double y,
            string name, string description)
        {
            RealCoords = new Point(x_image, y_image);
            X = x; Y = y;
            Title = name;
            Description = description;
            State = MapItemViewStateType.Inactive;
            Activate = new SimpleCommand(OnActivate);
            _trackChanges = true;
        }

        /// <summary>
        /// On click - mark as active and if needed, flip details visibility
        /// </summary>
        /// <param name="param"></param>
        private void OnActivate(object param)
        {
            if (State == MapItemViewStateType.ActiveDetailsShown)
            {
                State = MapItemViewStateType.ActiveDetailsHidden;
            }
            else
            {
                State = MapItemViewStateType.ActiveDetailsShown;
            }

            if (Selected != null)
            {
                Selected(this, new EventArgs());
            }
        }

        /// <summary>
        /// resets the changes flag on save
        /// </summary>
        public void ResetChanged()
        {
            IsChanged = false;
        }

        /// <summary>
        /// this item was selected by user
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        #region INotifyPropertyChanged
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion;
    }
}
