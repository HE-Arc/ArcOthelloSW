using System;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public class Data : ISerializable
    {
        public PlayerType PlayerType { get; private set; }
        public Color Color { get; private set; }
        public string Name { get; private set; }
        public int AvatarID { get; private set; }

        public Data(PlayerType playerType, Color color, string name, int avatarID)
        {
            PlayerType = playerType;
            Name = name;
            AvatarID = avatarID;
            Color = color;
        }

        protected Data(SerializationInfo info, StreamingContext context)
        {
            PlayerType = (PlayerType)info.GetValue("PlayerType", typeof(PlayerType));
            Name = (string)info.GetValue("Name", typeof(string));
            AvatarID = (int)info.GetValue("AvatarID", typeof(int));
            Color = (Color)info.GetValue("Color", typeof(Color));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PlayerType", PlayerType);
            info.AddValue("Name", Name);
            info.AddValue("AvatarID", AvatarID);
            info.AddValue("Color", Color);
        }
    }
}
