using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMap.Models
{
    [Serializable]
    public class MapData
    {
        public List<MapDataItem> Items { get; private set; }

        public MapData()
        {
            Items = new List<MapDataItem>();
        }
    }
}
