using System.Collections.Generic;
using System.IO;
using JsonExSerializer;

namespace Mirage.Game.World
{
    /// <summary>
    /// SimpleRepository using a Json file as the backing store
    /// </summary>
    /// <typeparam name="T">the list item type</typeparam>
    public class JsonSimpleRepository<T> : AbstractSimpleRepository<T>
    {
        private string _fileName;

        public JsonSimpleRepository(string FileName)
        {
            _fileName = FileName;
        }

        protected override List<T> Load()
        {
            Serializer serializer = new Serializer(typeof(List<T>));
            using (StreamReader reader = new StreamReader(_fileName))
            {
                return (List<T>)serializer.Deserialize(reader);
            }
        }
    }
}
