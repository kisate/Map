using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using WpfMap.CommandSupport;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;
using WpfMap.Models;

namespace WpfMap.ViewModels
{
    class MainViewModel : INotifyPropertyChanged, IPanZoomRegistrar
    {
        const string MAPURI = "pack://application:,,,/{0};component/PlainMap.jpg";
        const string DATAFILE = "MapData.xml";

        private FrameworkElement _map;
        private MapPanZoomer _zoomer;        
        private bool _isViewMode, _itemCollectionChanged;
        private MapDataItemVM _currentMapItem;
        private EditModeController _editController;

        private SimpleMenuItem _editMenu;

        #region Properties

        /// <summary>
        /// Current menu
        /// </summary>
        public ObservableCollection<SimpleMenuItem> MenuItems { get; protected set; }

        /// <summary>
        /// Image itself
        /// </summary>
        public BitmapImage MapImageSource { get; private set; }

        /// <summary>
        /// Transformation matrix for screen map
        /// </summary>
        public MatrixTransform ImageMatrix { get { return new MatrixTransform(_zoomer.GetMatrix()); } }

        /// <summary>
        /// Collection of map points with data
        /// </summary>
        public ObservableCollection<MapDataItemVM> MapItems { get; private set; }

        /// <summary>
        /// True = view only, false = can be edited
        /// </summary>
        public bool IsViewMode
        {
            get { return _isViewMode; }
            set
            {
                if (_isViewMode != value)
                {
                    _isViewMode = value;
                    OnPropertyChanged("IsViewMode");
                }
            }
        }

        /// <summary>
        /// Currently selected item for a details display
        /// and editing. Move/delete is relevant to that item
        /// </summary>
        public MapDataItemVM CurrentMapItem { 
            get { return _currentMapItem; } 
            set 
            { 
                _currentMapItem = value; 
                OnPropertyChanged("CurrentMapItem"); 
            } 
        }

        /// <summary>
        /// Show details view (name/description)
        /// </summary>
        public Visibility MapDetailsVisible
        {
            get 
            { 
                return _currentMapItem == null ? Visibility.Hidden : 
                    _currentMapItem.State == MapItemViewStateType.ActiveDetailsShown ? Visibility.Visible : Visibility.Hidden; 
            }
        }

        /// <summary>
        /// Where to show details - on the left or on the right
        /// if current point is set and it's on the left side of 
        /// screen - details shown on the rigth and vice versa
        /// </summary>
        public string DetailsViewAlignment
        {
            get
            {
                return (_currentMapItem == null || _map == null) ? "Left" :
                    _zoomer.GetRelativeX(_currentMapItem.X) > 0.5 ? "Left" : "Right";

            }
        }
        #endregion

        /// <summary>
        /// Loads image on creation, sets default zoom and offset
        /// </summary>
        public MainViewModel()
        {
            _zoomer = new MapPanZoomer();
            MenuItems = new ObservableCollection<SimpleMenuItem>();
            CreateMenu();
            MapItems = new ObservableCollection<MapDataItemVM>();
            IsViewMode = true;
        }

        public void Prepare()
        {
            PreloadImage();
            LoadData();
        }
        
        /// <summary>
        /// Load data from file
        /// </summary>
        private void LoadData()
        {
            var adapter = new MapDataAdapter(DATAFILE);
            MapData data;
            try
            {
                data = adapter.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.ErrorOnLoad + " " + ex);
                return;
            }

            foreach (var d in data.Items)
            {
                var mapItem = new MapDataItemVM(d.X, d.Y, 0, 0, d.Name, d.Description);
                mapItem.Selected += new EventHandler<EventArgs>(OnSelectCurrentItem);
                MapItems.Add(mapItem);
            }

            // from now on, track changes
            MapItems.CollectionChanged +=
                new System.Collections.Specialized.NotifyCollectionChangedEventHandler((o, e) => { _itemCollectionChanged = true; });

        }

        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="param"></param>
        private void SaveData(object param)
        {
            var data = new MapData();
            data.Items.AddRange(
                MapItems.Select(i =>
                    new MapDataItem { X = i.RealCoords.X, Y = i.RealCoords.Y, Name = i.Title, Description = i.Description }));

            var adapter = new MapDataAdapter(DATAFILE);
            adapter.Save(data);

            foreach (var item in MapItems)
            {
                item.ResetChanged();
            }
            _itemCollectionChanged = false;
        }

        private void PreloadImage()
        {
            MapImageSource = new BitmapImage();
            MapImageSource.BeginInit();
            MapImageSource.CacheOption = BitmapCacheOption.OnLoad;
            MapImageSource.CreateOptions = BitmapCreateOptions.None;

            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            var uri = string.Format(MAPURI, name);

            MapImageSource.UriSource = new Uri(uri, UriKind.Absolute);
            MapImageSource.EndInit();

            _zoomer.ImageSize = new Size(MapImageSource.Width, MapImageSource.Height);
        }
        
        /// <summary>
        /// Creates initial menu
        /// </summary>
        protected virtual void CreateMenu()
        {
            MenuItems.Clear();

            var fileMenu = new SimpleMenuItem(Properties.Resources.FileHeader);
            
            SimpleMenuItem editModeMenu;
            editModeMenu = new SimpleMenuItem(Properties.Resources.EditModeHeader, ChangeEditMode, true);
            editModeMenu.CommandParameter = editModeMenu;
            fileMenu.SubMenuItems.Add(editModeMenu);

            fileMenu.SubMenuItems.Add(new SimpleMenuItem(Properties.Resources.ExitHeader, Exit));

            MenuItems.Add(fileMenu);
            MenuItems.Add(new SimpleMenuItem(Properties.Resources.FullScreenHeader, SetFullScreen));

            _editController = new EditModeController();
            _editMenu = new SimpleMenuItem(Properties.Resources.EditHeader);
            _editMenu.SubMenuItems.Add(_editController.AddPoint);
            _editMenu.SubMenuItems.Add(_editController.MovePoint);
            _editMenu.SubMenuItems.Add(new SimpleMenuItem(Properties.Resources.DeletePointHeader, DeleteCurrentPoint));
            _editMenu.SubMenuItems.Add(new SimpleMenuItem(Properties.Resources.SaveHeader, SaveData));
            _editMenu.Visibility = Visibility.Hidden;
            MenuItems.Add(_editMenu);
        }

        #region Commands support

        /// <summary>
        /// Deletes current point
        /// </summary>
        /// <param name="param"></param>
        private void DeleteCurrentPoint(object param)
        {
            var message = Properties.Resources.DeleteConfirmationText;
            var caption = Properties.Resources.ConfirmationCaption;
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var item = CurrentMapItem;
                CurrentMapItem = null;
                MapItems.Remove(item);
            }
        }

        /// <summary>
        /// Set/reset edit mode and check/uncheck edit menu item
        /// </summary>
        /// <param name="menuItem"></param>
        private void ChangeEditMode(object param)
        {
            var menuItem = (param as SimpleMenuItem);
            
            if (IsViewMode)
            {
                IsViewMode = false;
                menuItem.IsChecked = true;
                _editMenu.Visibility = Visibility.Visible;
                
            }
            else
            {
                IsViewMode = true;
                menuItem.IsChecked = false;
                _editMenu.Visibility = Visibility.Hidden;

                _editController.ResetMode();
            }
        }

        /// <summary>
        /// Adieu
        /// </summary>
        /// <param name="param"></param>
        public void Exit(object param)
        {
            var message = Properties.Resources.ExitConfirmationText;
            var caption = Properties.Resources.ConfirmationCaption;

            var hasChanges = false;
            foreach (var item in MapItems)
            {
                if (item.IsChanged)
                {
                    hasChanges = true;
                    break;
                }                
            }

            if (!hasChanges)
            {
                hasChanges = _itemCollectionChanged;
            }

            if (hasChanges)
            {
                message += "\n" + Properties.Resources.UnsavedEditsConfirmationText;
            }

            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MapApp.Current.MainWindow.Close();
            }
        }

        /// <summary>
        /// MAke full screen
        /// </summary>
        /// <param name="param"></param>
        public void SetFullScreen(object param)
        {
            MapApp.Current.MainWindow.Topmost = true;
            MapApp.Current.MainWindow.WindowStyle = WindowStyle.None;
            MapApp.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        public void OnWindowLoad(object sender, RoutedEventArgs e)
        {
            SetFullScreen(null);
        }
        #endregion

        #region NotifyPropertyChanged
        /// <summary>
        /// HAve map to refresh
        /// </summary>
        protected void RefreshMapTransformation()
        {
            // require matrix refresh by UI
            OnPropertyChanged("ImageMatrix");
        }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Map mouse events and IPanZoomRegistrar

        /// <summary>
        /// Start listening map events
        /// </summary>
        /// <param name="element"></param>
        public void SubscribeFrameworkElementForPanAndZoom(FrameworkElement element)
        {
            _map = element;
            _map.SizeChanged += new SizeChangedEventHandler(MapSizeChanged);
            _map.MouseWheel += new MouseWheelEventHandler(MapMouseWheel);
            _map.MouseLeftButtonDown += new MouseButtonEventHandler(MapMouseLeftButtonDown);
            _map.MouseMove += new MouseEventHandler(MapMouseMove);
            _map.MouseUp += new MouseButtonEventHandler(MapMouseUp);
        }

        /// <summary>
        /// Release one of buttons. If left - that's moving, if middle - editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    MapMouseLeftButtonUp(e);
                }
                else if (e.ChangedButton == MouseButton.Middle) 
                {
                    MapMouseMiddleButtonUp(e);
                }
            }
            
        }

        /// <summary>
        /// Middle button - for editing purposes
        /// </summary>
        /// <param name="e"></param>
        void MapMouseMiddleButtonUp(MouseButtonEventArgs e)
        {
            var stretchedPosition = e.GetPosition(_map);
            var realCoords = _zoomer.TranslatePointBack(stretchedPosition);
            var position = _zoomer.TranslatePoint(realCoords);

            switch (_editController.EditMode)
            {
                case EditModeType.NewPoint:
                    var item = new MapDataItemVM(realCoords.X, realCoords.Y, position.X, position.Y,
                        Properties.Resources.NewPointDefaultName, 
                        Properties.Resources.NewPointDefaultDescription);
                    item.Selected += OnSelectCurrentItem;
                    MapItems.Add(item);
                    OnSelectCurrentItem(item, new EventArgs());
                    break;

                case EditModeType.MovePoint:
                    if (CurrentMapItem != null)
                    {
                        CurrentMapItem.X = position.X;
                        CurrentMapItem.Y = position.Y;
                        CurrentMapItem.RealCoords = realCoords;
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// if it was Pan - release capture by map
        /// </summary>
        /// <param name="e"></param>
        void MapMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _map.ReleaseMouseCapture();
        }

        /// <summary>
        /// Mouse move for Pan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapMouseMove(object sender, MouseEventArgs e)
        {
            if (_map.IsMouseCaptured)
            {
                _zoomer.Pan(e.GetPosition(_map));
                AdjustMapItems();
                RefreshMapTransformation();
            }
        }

        /// <summary>
        /// Start panning: start capturing mouse by map and remember position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(_map);
            _map.CaptureMouse();
            _zoomer.PanOrigin = position;
        }

        /// <summary>
        /// Wheel rotate - zoom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _zoomer.Zoom(e.Delta > 0, e.GetPosition(_map));
            AdjustMapItems();
            RefreshMapTransformation();
        }

        /// <summary>
        /// Resize of UI - remmeber new map size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _zoomer.ResizeMap(e.NewSize);
            AdjustMapItems();
            RefreshMapTransformation();
        }

        /// <summary>
        /// Recalcluate coordinaates of all map items if map was zoomed, shifted or resized
        /// </summary>
        void AdjustMapItems()
        {
            foreach (var item in MapItems)
            {
                var newCoords = _zoomer.TranslatePoint(item.RealCoords);
                item.X = newCoords.X;
                item.Y = newCoords.Y;
            }
        }
        #endregion

        /// <summary>
        /// Called by map items when they become selected
        /// </summary>
        /// <param name="sender">new active map item</param>
        /// <param name="e"></param>
        private void OnSelectCurrentItem(object sender, EventArgs e)
        {
            foreach (var item in MapItems)
            {
                if (item != sender)
                {
                    item.State = MapItemViewStateType.Inactive;
                }
                else
                {
                    CurrentMapItem = item;
                }
            }
            OnPropertyChanged("MapDetailsVisible");
            OnPropertyChanged("DetailsViewAlignment");
        }
    }
}
