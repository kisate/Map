using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace WpfMap.Models
{
    class MapDataAdapter
    {
        private string _fileName;

        public MapDataAdapter(string dataFileName)
        {
            _fileName = dataFileName;
        }

        public MapData Load()
        {
            var ser = new XmlSerializer(typeof(MapData), new Type[] { typeof(MapDataItem) });
            var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, _fileName);
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ser.Deserialize(stream) as MapData;
            }
        }

        public void Save(MapData data)
        {
            var ser = new XmlSerializer(typeof(MapData), new Type[] { typeof(MapDataItem)});
            var basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, _fileName);
            if (File.Exists(path))
            {
                var name = Path.GetFileNameWithoutExtension(_fileName);
                var backup = Path.Combine(basePath, _fileName.Replace(name, string.Format("{0}_{1:yyMMddhhmmss}", name, DateTime.Now)));
                File.Copy(path, backup);
            }

            using (var writer = new StreamWriter(path))
            {
                ser.Serialize(writer, data);
            }
        }
    }
}
