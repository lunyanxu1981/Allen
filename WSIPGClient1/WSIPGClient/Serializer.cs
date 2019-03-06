using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WSIPGClient.WebReference;

namespace WSIPGClient
{
    public static class Serializer
    {
        #region Functions

        public static void SerializeToXML<T>(T t, String inFilename)
        {
            XmlSerializer serializer = new XmlSerializer(t.GetType());
            TextWriter textWriter = new StreamWriter(inFilename);
            serializer.Serialize(textWriter, t);
            textWriter.Close();
        }

        public static T DeserializeFromXML<T>(String inFilename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            TextReader textReader = new StreamReader(inFilename);
            T retVal = (T)deserializer.Deserialize(textReader);
            textReader.Close();
            return retVal;
        }

        #endregion
    }
}
