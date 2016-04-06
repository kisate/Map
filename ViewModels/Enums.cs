namespace WpfMap.ViewModels
{
    /// <summary>
    /// Current edit state for a middle button click
    /// </summary>
    enum EditModeType { None, NewPoint, MovePoint };

    /// <summary>
    /// 3 states of map data item
    /// </summary>
    enum MapItemViewStateType { Inactive, ActiveDetailsHidden, ActiveDetailsShown };
}
