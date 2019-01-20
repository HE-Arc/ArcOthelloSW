using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [Serializable]
    public class ExportedGame : ISerializable
    {
        public List<GameState> GameStates { get; private set; }

        public Color CurrentPlayer { get; private set; }

        public BattleType BattleType { get; private set; }

        public ExportedGame(BattleType battleType, List<GameState> gameStates, Color currentPlayer)
        {
            BattleType = battleType;
            GameStates = gameStates;
            CurrentPlayer = currentPlayer;
        }

        protected ExportedGame(SerializationInfo info, StreamingContext context)
        {
            BattleType = (BattleType)info.GetValue("BattleType", typeof(BattleType));
            GameStates = (List<GameState>)info.GetValue("GameStates", typeof(List<GameState>));
            CurrentPlayer = (Color)info.GetValue("CurrentPlayer", typeof(Color));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BattleType", BattleType);
            info.AddValue("GameStates", GameStates);
            info.AddValue("CurrentPlayer", CurrentPlayer);
        }
    }
}
