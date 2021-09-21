using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serializer
{
    public class ObjectSerializer
    {
        public static string ToString(object obj)
        {
            MemoryStream ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, obj);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static object ToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return new BinaryFormatter().Deserialize(ms);
        }
    }
}