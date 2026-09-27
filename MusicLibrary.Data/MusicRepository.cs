using System.IO;
using System.Xml.Serialization;
using MusicLibrary.Core;

namespace MusicLibrary.Data
{
    public class MusicRepository
    {
        public void Save(MusicCollection collection, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MusicCollection));

            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, collection);
            }
        }

        public MusicCollection Load(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Файл музыкальной коллекции не найден.", fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(MusicCollection));

            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                return (MusicCollection)serializer.Deserialize(stream);
            }
        }
    }
}