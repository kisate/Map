using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMap.CommandSupport;

namespace WpfMap.ViewModels
{
    sealed class EditModeController
    {
        /// <summary>
        /// current mode
        /// </summary>
        public EditModeType EditMode { get; private set; }

        /// <summary>
        /// menu items
        /// </summary>
        public SimpleMenuItem AddPoint { get; private set; }
        public SimpleMenuItem MovePoint { get; private set; }

        public EditModeController()
        {
            AddPoint = new SimpleMenuItem(Properties.Resources.CreateNewHeader, CreateMovePoint, 
                (x) => EditMode != EditModeType.MovePoint)
            {
                IsCheckable = true,
                CommandParameter = EditModeType.NewPoint
            };

            MovePoint = new SimpleMenuItem(Properties.Resources.MovePointHeader, CreateMovePoint, true)
            {
                CommandParameter = EditModeType.MovePoint
            };

            ResetMode();
        }

        private void CreateMovePoint(object param)
        {
            var newMode = (EditModeType)param;
            if (newMode == EditMode)
            {
                EditMode = EditModeType.None;
                AddPoint.IsChecked = false;
                MovePoint.IsChecked = false;
            }
            else if(EditMode == EditModeType.None)
            {
                EditMode = newMode;
                AddPoint.IsChecked = newMode == EditModeType.NewPoint;
                MovePoint.IsChecked = newMode == EditModeType.MovePoint;
            }
            else if (newMode == EditModeType.MovePoint) // can switch from new to move
            {
                EditMode = EditModeType.MovePoint;
                AddPoint.IsChecked = false;
                MovePoint.IsChecked = true;
            }
            // can't switch from move to new
            AddPoint.Command.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// just set to none
        /// </summary>
        internal void ResetMode()
        {
            EditMode = EditModeType.None;
            AddPoint.IsChecked = false;
            MovePoint.IsChecked = false;
            AddPoint.Command.RaiseCanExecuteChanged();
        }
    }
}
