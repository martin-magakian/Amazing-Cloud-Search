using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace AmazingCloudSearch
{
    public static class DocumentSizeExtension
    {
        public static int GetSize(this object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, JsonConvert.SerializeObject(obj));
                return Convert.ToInt32(ms.Length);
            }
        }
    }
}
