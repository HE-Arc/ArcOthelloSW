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
        public string Player1Name { get; private set; }
        public string Player2Name { get; private set; }
        public int Player1AvatarID { get; private set; }
        public int Player2AvatarID { get; private set; }
        public int Player1Color { get; private set; }
        public int Player2Color { get; private set; }

        public SaveFile(List<GameState> game)
        {
            States = game;
        }

        protected SaveFile(SerializationInfo info, StreamingContext context)
            : this((List<GameState>)info.GetValue("States", typeof(List<GameState>)))
        {
            Player1Name = info.GetString("Player1Name");
            Player1AvatarID = info.GetInt32("Player1AvatarID");
            Player1Color = info.GetInt32("Player1Color");
            Player2Name = info.GetString("Player2Name");
            Player2AvatarID = info.GetInt32("Player2AvatarID");
            Player2Color = info.GetInt32("Player2Color");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("States", States);
            info.AddValue("Player1Name", Player1Name);
            info.AddValue("Player1AvatarID", Player1AvatarID);
            info.AddValue("Player1Color", Player1Color);
            info.AddValue("Player2Name", Player2Name);
            info.AddValue("Player2AvatarID", Player2AvatarID);
            info.AddValue("Player2Color", Player2Color);
        }

        public void Save(string player1Name, int player1AvatarID, Color player1Color, string player2Name, int player2AvatarID, Color player2Color)
        {
            Player1Name = player1Name;
            Player1AvatarID = player1AvatarID;
            Player1Color = (int)player1Color;

            Player2Name = player2Name;
            Player2AvatarID = player2AvatarID;
            Player2Color = (int)player2Color;

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
