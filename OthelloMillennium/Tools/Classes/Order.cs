﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tools.Classes;

namespace Tools
{
    [Serializable]
    public abstract class Order : ISerializable
    {
        public Order() { }

        protected Order(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
        }

        public abstract string GetAcronym();
        public abstract string GetDefinition();

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }

    #region Handshake
    [Serializable]
    public class RegisterRequestOrder : Order
    {
        public string Name { get; private set; }
        public int PlayerType { get; private set; }

        public RegisterRequestOrder(PlayerType playerType, string name) {
            PlayerType = (int)playerType;
            Name = name;
        }

        protected RegisterRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PlayerType = info.GetInt32("PlayerType");
            Name = info.GetString("Name");
        }

        public override string GetAcronym()
        {
            return "RRO";
        }

        public override string GetDefinition()
        {
            return "Register to the matchmaking";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PlayerType", PlayerType);
            info.AddValue("Name", Name);
        }
    }

    [Serializable]
    public class RegisterSuccessfulOrder : Order
    {
        protected RegisterSuccessfulOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "RS";
        }

        public override string GetDefinition()
        {
            return "Inform current player that he has been registred successfully";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public RegisterSuccessfulOrder() { }
    }

    [Serializable]
    public class SearchRequestOrder : Order
    {
        public int OpponentType { get; private set; }

        public SearchRequestOrder(PlayerType opponentType)
        {
            OpponentType = (int)opponentType;
        }

        protected SearchRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OpponentType = info.GetInt32("OpponentType");
        }

        public override string GetAcronym()
        {
            return "SRO";
        }

        public override string GetDefinition()
        {
            return "Search a battle against an opponent of the specified type";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OpponentType", OpponentType);
        }
    }

    [Serializable]
    public class AssignColorOrder : Order
    {
        public int Color { get; private set; }

        public AssignColorOrder(Color opponentType)
        {
            Color = (int)opponentType;
        }

        protected AssignColorOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Color = info.GetInt32("Color");
        }

        public override string GetAcronym()
        {
            return "ACO";
        }

        public override string GetDefinition()
        {
            return "Server assign a color";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Color", Color);
        }
    }

    [Serializable]
    public class AssignAvatarIDOrder : Order
    {
        public int AvatarID { get; private set; }

        public AssignAvatarIDOrder(int avatarID)
        {
            AvatarID = avatarID;
        }

        protected AssignAvatarIDOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            AvatarID = info.GetInt32("AvatarID");
        }

        public override string GetAcronym()
        {
            return "AAO";
        }

        public override string GetDefinition()
        {
            return "Server assign an avatar";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AvatarID", AvatarID);
        }
    }

    [Serializable]
    public class OpponentFoundOrder : Order
    {
        public string OpponentName { get; private set; }
        public int OpponentType { get; private set; }

        public OpponentFoundOrder(string opponentName, PlayerType opponentType)
        {
            OpponentName = opponentName;
            OpponentType = (int)opponentType;
        }

        protected OpponentFoundOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OpponentType = info.GetInt32("OpponentType");
            OpponentName = info.GetString("OpponentName");
        }

        public override string GetAcronym()
        {
            return "OFO";
        }

        public override string GetDefinition()
        {
            return "Inform current player that an opponent has been found";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OpponentName", OpponentName);
            info.AddValue("OpponentType", OpponentType);
        }
    }

    #endregion

    #region GameLogic

    [Serializable]
    public class PlayerReadyOrder : Order
    {
        protected PlayerReadyOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        
        public override string GetAcronym()
        {
            return "PRO";
        }

        public override string GetDefinition()
        {
            return "Inform the gameHandler that the player is ready";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public PlayerReadyOrder() { }
    }

    [Serializable]
    public class GameReadyOrder : Order
    {
        protected GameReadyOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GRO";
        }

        public override string GetDefinition()
        {
            return "Inform the player that the game is ready";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public GameReadyOrder() { }
    }

    [Serializable]
    public class GameStartedOrder : Order
    {
        public GameState InitialState { get; private set; }

        protected GameStartedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitialState = (GameState)info.GetValue("InitialState", typeof(GameState));
        }

        public override string GetAcronym()
        {
            return "GSO";
        }

        public override string GetDefinition()
        {
            return "Inform the player that the game started";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InitialState", InitialState);
        }

        public GameStartedOrder(GameState initialGameState) {
            InitialState = initialGameState;
        }
    }

    [Serializable]
    public class GameEndedOrder : Order
    {
        protected GameEndedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GEO";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the game has ended";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public GameEndedOrder() { }
    }

    [Serializable]
    public class PlayMoveOrder : Order
    {
        public Tuple<char, int> Coords { get; set; }

        public PlayMoveOrder(Tuple<char, int> coords)
        {
            Coords = coords;
        }

        protected PlayMoveOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Coords = (Tuple<char, int>)info.GetValue("Coords", typeof(Tuple<char, int>));
        }

        public override string GetAcronym()
        {
            return "PMO";
        }

        public override string GetDefinition()
        {
            return "Contains coords where the player want to place an othello's game piece";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Coords", Coords);
        }
    }

    

    #endregion

    #region Synchronization

    [Serializable]
    public class GameStateRequestOrder : Order
    {
        protected GameStateRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "GSRO";
        }

        public override string GetDefinition()
        {
            return "Ask the server for the current gameState";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public GameStateRequestOrder() { }
    }

    /// <summary>
    /// Client send this whenever it changes his datas
    /// </summary>
    [Serializable]
    public class AvatarChangedOrder : Order
    {
        public int AvatarID { get; private set; }

        public AvatarChangedOrder(int avatarID)
        {
            AvatarID = avatarID;
        }

        protected AvatarChangedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            AvatarID = info.GetInt32("AvatarID");
        }

        public override string GetAcronym()
        {
            return "OAC";
        }

        public override string GetDefinition()
        {
            return "Client changed his avatar";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AvatarID", AvatarID);
        }
    }

    /// <summary>
    /// Server send this to update client-side gameState
    /// </summary>
    [Serializable]
    public class UpdateGameStateOrder : Order
    {
        public GameState GameState { get; private set; }

        public UpdateGameStateOrder(GameState gameState)
        {
            GameState = gameState;
        }

        protected UpdateGameStateOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            GameState = (GameState)info.GetValue("GameState", typeof(GameState));
        }

        public override string GetAcronym()
        {
            return "UGO";
        }

        public override string GetDefinition()
        {
            return "Update gameState";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GameState", GameState);
        }
    }

   

    #region Opponent

    /// <summary>
    /// Client send this whenever it changes his datas
    /// </summary>
    [Serializable]
    public class OpponentAvatarChangedOrder : Order
    {
        public int AvatarID { get; private set; }

        public OpponentAvatarChangedOrder(int avatarID)
        {
            AvatarID = avatarID;
        }

        protected OpponentAvatarChangedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            AvatarID = info.GetInt32("AvatarID");
        }

        public override string GetAcronym()
        {
            return "OACO";
        }

        public override string GetDefinition()
        {
            return "Opponent changed his avatar";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AvatarID", AvatarID);
        }
    }

    #endregion

    #endregion

    #region TCP

    [Serializable]
    public class OpponentDisconnectedOrder : Order
    {
        protected OpponentDisconnectedOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "OD";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the opponent has disconnected";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public OpponentDisconnectedOrder() { }
    }

    [Serializable]
    public class OpponentConnectionLostOrder : Order
    {
        protected OpponentConnectionLostOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "OL";
        }

        public override string GetDefinition()
        {
            return "Inform current player that the opponent has lost connection with the server";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public OpponentConnectionLostOrder() { }
    }

    #endregion

    #region GameManager

    [Serializable]
    public class SaveRequestOrder : Order
    {
        protected SaveRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "SRO";
        }

        public override string GetDefinition()
        {
            return "Ask the server to save the game";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public SaveRequestOrder() { }
    }

    /// <summary>
    /// Server send this to export the current game
    /// </summary>
    [Serializable]
    public class SaveResponseOrder : Order
    {
        public SaveFile SaveFile { get; private set; }

        public SaveResponseOrder(SaveFile saveFile)
        {
            SaveFile = saveFile;
        }

        protected SaveResponseOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SaveFile = (SaveFile)info.GetValue("SaveFile", typeof(SaveFile));
        }

        public override string GetAcronym()
        {
            return "SRO";
        }

        public override string GetDefinition()
        {
            return "Request asking the server to load a game";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SaveFile", SaveFile);
        }
    }

    /// <summary>
    /// Client send this to load the current game
    /// </summary>
    [Serializable]
    public class LoadRequestOrder : Order
    {
        public SaveFile SaveFile { get; private set; }

        public LoadRequestOrder(SaveFile saveFile)
        {
            SaveFile = saveFile;
        }

        protected LoadRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SaveFile = (SaveFile)info.GetValue("SaveFile", typeof(SaveFile));
        }

        public override string GetAcronym()
        {
            return "LRO";
        }

        public override string GetDefinition()
        {
            return "Request asking the server to load a game";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SaveFile", SaveFile);
        }
    }

    [Serializable]
    public class LoadResponseOrder : Order
    {
        public int GameID { get; private set; }

        public LoadResponseOrder(int gameID)
        {
            GameID = gameID;
        }

        protected LoadResponseOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            GameID = info.GetInt32("GameID");
        }

        public override string GetAcronym()
        {
            return "LRO";
        }

        public override string GetDefinition()
        {
            return "Used by server to share the loadedGame's ID";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GameID", GameID);
        }
    }

    [Serializable]
    public class JoinRequestOrder : Order
    {
        public int GameID { get; private set; }

        public JoinRequestOrder(int gameID)
        {
            GameID = gameID;
        }

        protected JoinRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            GameID = info.GetInt32("GameID");
        }

        public override string GetAcronym()
        {
            return "JRO";
        }

        public override string GetDefinition()
        {
            return "Used by client to join a loaded game";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GameID", GameID);
        }
    }

    [Serializable]
    public class JoinResponseOrder : Order
    {
        public JoinResponseOrder()
        { }

        protected JoinResponseOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "JRO";
        }

        public override string GetDefinition()
        {
            return "Used by server to confirm a join request";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }
    }

    [Serializable]
    public class UndoRequestOrder : Order
    {
        protected UndoRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "URO";
        }

        public override string GetDefinition()
        {
            return "Ask the server to undo the last move";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public UndoRequestOrder() { }
    }

    [Serializable]
    public class RedoRequestOrder : Order
    {
        protected RedoRequestOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public override string GetAcronym()
        {
            return "RRO";
        }

        public override string GetDefinition()
        {
            return "Ask the server to redo the last move";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        { }

        public RedoRequestOrder() { }
    }

    #endregion

}