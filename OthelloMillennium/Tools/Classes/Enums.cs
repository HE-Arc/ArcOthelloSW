using System;
using System.Runtime.Serialization;

namespace Tools.Classes
{
    [DataContract]
    public enum GameType
    {
        [EnumMember]
        Local,
        [EnumMember]
        Online,
    }

    [DataContract]
    public enum BattleType
    {
        [EnumMember]
        AgainstAI,
        [EnumMember]
        AgainstPlayer,
    }

    [DataContract]
    public enum PlayerType
    {
        [EnumMember]
        Human,
        [EnumMember]
        AI,
        [EnumMember]
        Any,

        // Used only on the server side
        [EnumMember]
        None,
    }

    [DataContract]
    public enum Color
    { // Values have been choosen to fit GameState.CellState
        [EnumMember]
        Black = 1,
        [EnumMember]
        White = 2
    }
}
