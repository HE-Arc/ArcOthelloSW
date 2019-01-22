﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class OthelloPlayerClient : OrderHandler
    {
        // properties

        #region Attributes
        private int avatarId;
        private Client client;

        #endregion

        #region Properties

        public int AvatarId
        {
            get { return avatarId; }
            set
            {
                if (!IsLocalPlayer && PlayerState.LOBBY_CHOICE != PlayerState)
                {
                    throw new Exception("This is a remote player you cannot edit it");
                }
                avatarId = value;
                client.Send(new AvatarChangedOrder(avatarId));
            }
        }

        // Client properties
        public string Name { get; private set; }
        public PlayerType PlayerType { get; set; }
        public BattleType BattleType { get; set; }
        public Color color { get; private set; }

        public bool IsLocalPlayer { get; private set; }
        public PlayerState PlayerState { get; private set; }
        #endregion


        public OthelloPlayerClient(PlayerType playerType, BattleType battleType, string name)
        {
            PlayerType = playerType;
            BattleType = battleType;
            Name = name;

            PlayerState = PlayerState.INITIAL;

            //TODO CHOOSE WHEN TO INIT CLIENT
        }

        public void Register()
        {
            if (PlayerState != PlayerState.INITIAL) throw new Exception("Action not available");
            client.Send(new RegisterRequestOrder(PlayerType, Name));
            PlayerState = PlayerState.REGISTERING;
        }

        public void SearchOpponent()
        {
            if (PlayerState != PlayerState.REGISTERED) throw new Exception("Action not available");
            PlayerType opponentType = BattleType == BattleType.AgainstPlayer ? PlayerType.Human : PlayerType.AI;
            client.Send(new SearchOrder(opponentType));
            PlayerState = PlayerState.SEARCHING;
        }

        public void ReadyToPlay()
        {
            if (PlayerState != PlayerState.LOBBY_CHOICE) throw new Exception("Action not allowed");
            client.Send(new ReadyOrder());
            PlayerState = PlayerState.READY;
        }

        public void HandleOrder(Order orderHandled)
        {
            switch (orderHandled)
            {

                case RegisterSuccessfulOrder order:
                    PlayerState = PlayerState.REGISTERED;
                    break;

                case OpponentFoundOrder order:
                    PlayerState = PlayerState.LOBBY_CHOICE;
                    break;

                case OpponentAvatarChangedOrder order:
                    //TODO
                    break;

                case GameReadyOrder order:
                    //TODO
                    PlayerState = PlayerState.ABOUT_TO_START;
                    break;

                case GameStartedOrder order:
                    PlayerState = PlayerState.IN_GAME;
                    //TODO
                    break;


            }
        }
    }

    public enum PlayerState
    {
        INITIAL,
        REGISTERING,
        REGISTERED,
        SEARCHING,
        LOBBY_CHOICE,
        READY,
        ABOUT_TO_START,
        IN_GAME;
    }
}
