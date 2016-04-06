using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WpfMap.Models
{
    [Serializable]
    public class MapDataItem
    {
        [XmlAttribute]
        public double X { get; set; }
        [XmlAttribute]
        public double Y { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }
    }
}
