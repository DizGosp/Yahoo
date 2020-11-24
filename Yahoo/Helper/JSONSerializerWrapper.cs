using System.IO;
using System.Runtime.Serialization.Json;

namespace Yahoo.Controllers
{
    //Klasa za konvertovanje iz JSona
    public class JSONSerializerWrapper
    {
        public static T Deserialize<T>(string jsonObj)
        {
            using (MemoryStream DeserializeStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                StreamWriter writter = new StreamWriter(DeserializeStream);
                writter.Write(jsonObj);
                writter.Flush();
                DeserializeStream.Position = 0;
                T deserializedObject = (T)serializer.ReadObject(DeserializeStream);
                return deserializedObject;
            }
        }
    }
}