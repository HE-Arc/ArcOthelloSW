﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Tools
{
    public class OthelloPlayerServer : IOrderHandler
    {
        #region Attributes
        private OthelloTCPClient client;
        private IOrderHandler orderHandler;
        #endregion

        #region Properties
        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType { get; private set; }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color { get; private set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tcpClient">socket</param>
        public OthelloPlayerServer(OthelloTCPClient tcpClient)
        {
            client = tcpClient;
            client.SetOrderHandler(this);
        }

        public void SetOrderHandler(IOrderHandler orderHandler)
        {
            this.orderHandler = orderHandler;
        }

        /// <summary>
        /// Send the color the the client-side
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            Color = color;
            client.Send(new AssignColorOrder(color));
        }

        /// <summary>
        /// Send the avatarID the the client-side
        /// </summary>
        /// <param name="avatarID"></param>
        public void SetAvatarID(int avatarID)
        {
            client.Send(new AssignAvatarIDOrder(avatarID));
        }

        /// <summary>
        /// Send the opponentName to the client-side
        /// </summary>
        /// <param name="opponentName"></param>
        public void OpponentFound(string opponentName, PlayerType opponentType)
        {
            client.Send(new OpponentFoundOrder(opponentName, opponentType));
        }

        /// <summary>
        /// Send the opponent's avatarID to the client-side
        /// </summary>
        /// <param name="avatarId"></param>
        public void OpponentAvatarChanged(int avatarId)
        {
            client.Send(new OpponentAvatarChangedOrder(avatarId));
        }

        public void OpponentConnectionLost()
        {
            client.Send(new OpponentConnectionLostOrder());
        }

        public void OpponentDisconnected()
        {
            client.Send(new OpponentDisconnectedOrder());
        }

        public void OpponentReconnected()
        {
            throw new NotImplementedException("[OpponentReconnected] : OthelloPlayerServer");
        }

        public void RegisterSuccessful(string name, PlayerType playerType)
        {
            Name = name;
            PlayerType = playerType;
            client.Send(new RegisterSuccessfulOrder());
        }

        public void SaveSuccessful(List<GameState> states)
        {
            client.Send(new SaveResponseOrder(new Classes.SaveFile(states)));
        }

        public void LoadSuccessful(int gameID)
        {
            client.Send(new LoadResponseOrder(gameID));
        }

        public void JoinSuccessful()
        {
            client.Send(new JoinResponseOrder());
        }

        /// <summary>
        /// Call by the gameHandler
        /// </summary>
        public void GameReady()
        {
            client.Send(new GameReadyOrder());
        }

        /// <summary>
        /// Call by the gameHandler
        /// </summary>
        public void GameStarted(GameState initialState)
        {
            client.Send(new GameStartedOrder(initialState));
        }

        /// <summary>
        /// Call by the gameHandler
        /// </summary>
        /// <param name="gameState"></param>
        public void UpdateGameboard(GameState gameState)
        {
            client.Send(new UpdateGameStateOrder(gameState));
        }

        /// <summary>
        /// Call by the gameHandler
        /// </summary>
        public void GameEnded()
        {
            client.Send(new GameEndedOrder());
        }

        /// <summary>
        /// Handle received orders
        /// </summary>
        /// <param name="order"></param>
        public void HandleOrder(IOrderHandler sender, Order order)
        {
            // If null, sender is this object otherwise the order has been redirected
            sender = sender ?? this;

            switch (order)
            {
                #region Forwarded orders

                #region To GameHandler
                case AvatarChangedOrder a:
                case PlayerReadyOrder b:
                case PlayMoveOrder c:
                case UndoRequestOrder d:
                case RedoRequestOrder e:
                case SaveRequestOrder f:
                    orderHandler.HandleOrder(sender, order);
                    break;

                #endregion

                #region To Matchmaker
                case RegisterRequestOrder castedOrder:
                    Name = !string.IsNullOrEmpty(castedOrder.Name) ? castedOrder.Name : throw new ArgumentException("name can't be null or empty");
                    PlayerType = (PlayerType)castedOrder.PlayerType;

                    Console.WriteLine("Send registerRequest to matchmaker");
                    orderHandler.HandleOrder(this, order);
                    break;

                case JoinRequestOrder castedOrder:
                    orderHandler.HandleOrder(this, order);
                    break;

                case SearchRequestOrder castedOrder:
                    orderHandler.HandleOrder(this, order);
                    break;

                case LoadRequestOrder castedOrder:
                    orderHandler.HandleOrder(this, order);
                    break;

                    #endregion

                    #endregion
            }
        }
    }
}
