using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IrcUserAdmin.Tools
{
    public class XmlSerializerHelper<T>
    {
        private readonly Type _type;

        public XmlSerializerHelper()
        {
            _type = typeof(T);
        }

        public void Save(string path, object obj)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(_type);
                serializer.Serialize(textWriter, obj);
            }
        }

        public T CloneObject(T obj)
        {
            T cloneObj;
            var xs = new XmlSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    xs.Serialize(xmlTextWriter, obj);
                    memoryStream.Position = 0;
                    using (TextReader textReader = new StreamReader(memoryStream))
                    {
                        var deserializer = new XmlSerializer(typeof(T));
                        cloneObj = (T)deserializer.Deserialize(textReader);
                    }
                }
            }
            return cloneObj;
        }

        public T Read(string path)
        {
            T result;
            using (TextReader textReader = new StreamReader(path))
            {
                var deserializer = new XmlSerializer(_type);
                result = (T)deserializer.Deserialize(textReader);
            }
            return result;
        }
    }
}