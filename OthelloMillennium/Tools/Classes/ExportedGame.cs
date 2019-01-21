using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tools
{
    [Serializable]
    public class ExportedGame : ISerializable
    {
        public List<GameState> GameStates { get; private set; }

        public Color CurrentPlayer { get; private set; }

        public GameType GameType { get; private set; }

        public ExportedGame(GameType gameType, List<GameState> gameStates, Color currentPlayer)
        {
            GameType = gameType;
            GameStates = gameStates;
            CurrentPlayer = currentPlayer;
        }

        protected ExportedGame(SerializationInfo info, StreamingContext context)
        {
            GameType = (GameType)info.GetValue("BattleType", typeof(BattleType));
            GameStates = (List<GameState>)info.GetValue("GameStates", typeof(List<GameState>));
            CurrentPlayer = (Color)info.GetValue("CurrentPlayer", typeof(Color));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BattleType", GameType);
            info.AddValue("GameStates", GameStates);
            info.AddValue("CurrentPlayer", CurrentPlayer);
        }
    }
}
