﻿using System;
using System.Runtime.Serialization;

namespace Tools
{
    [Serializable]
    public class Data : ISerializable
    {
        public int PlayerType { get; private set; }
        public int Color { get; private set; }
        public string Name { get; private set; }
        public int AvatarID { get; private set; }

        public Data(PlayerType playerType, Color color, string name, int avatarID)
        {
            PlayerType = (int)playerType;
            Name = name;
            AvatarID = avatarID;
            Color = (int)color;
        }

        protected Data(SerializationInfo info, StreamingContext context)
        {
            PlayerType = (int)info.GetValue("PlayerType", typeof(int));
            Name = (string)info.GetValue("Name", typeof(string));
            AvatarID = (int)info.GetValue("AvatarID", typeof(int));
            Color = (int)info.GetValue("Color", typeof(int));
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
