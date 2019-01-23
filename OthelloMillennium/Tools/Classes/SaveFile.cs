using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tools.Classes
{
    [Serializable]
    public class SaveFile : ISerializable
    {
        private static readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public List<GameState> States { get; private set; }

        public SaveFile(List<GameState> game)
        {
            States = game;
        }

        protected SaveFile(SerializationInfo info, StreamingContext context)
            : this((List<GameState>)info.GetValue("States", typeof(List<GameState>)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("States", States);
        }

        public void Save()
        {
            using (FileStream fs = new FileStream("save.bin", FileMode.Create, FileAccess.Write))
            {
                binaryFormatter.Serialize(fs, this);
            }
        }

        public static SaveFile Load()
        {
            using (FileStream fs = new FileStream("save.bin", FileMode.Open, FileAccess.Read))
            {
                return (SaveFile)binaryFormatter.Deserialize(fs);
            }
        }
    }
}
