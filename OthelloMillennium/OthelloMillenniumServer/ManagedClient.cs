using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace OthelloMillenniumServer
{
    public class ManagedClient
    {
        public enum ManagedState
        {
            Undefined,
            Searching,
            Binded,
            InGame,
            ConnectionLost,
            Disconnected
        }

        // Informations
        public TcpClient Socket { get; private set; }
        public ManagedState State { get; private set; }

        // InGame
        public ManagedClient Opponent { get; private set; }
       
        // Time informations
        public DateTime RegisterDateTime { get; private set; }

        public ManagedClient(TcpClient client)
        {
            Socket = client;
            State = ManagedState.Undefined;
            RegisterDateTime = DateTime.Now;
        }

        public static void Bind(ManagedClient client1, ManagedClient client2)
        {
            if(client1.Opponent == null && client2.Opponent == null)
            {
                try
                {
                    // Open connection between the two client
                    var hp1 = TCPServer.Instance.GetHostNameAndPort(client1.Socket);
                    var hp2 = TCPServer.Instance.GetHostNameAndPort(client2.Socket);

                    client1.Socket.Connect(hp2.Item1, hp2.Item2);
                    client2.Socket.Connect(hp1.Item1, hp1.Item2);

                    // Assign opponent
                    client1.Opponent = client2;
                    client2.Opponent = client1;

                    // Set new state
                    client1.State = client2.State = ManagedState.Binded;
                }
                catch (Exception ex)
                {
                    Toolbox.LogError(ex);
                }
            }
            else
            {
                throw new Exception("Client already binded !");
            }
        }

        public bool Send(string message)
        {
            if (State == ManagedState.Binded || State == ManagedState.InGame)
            {
                try
                {
                    var stream = Opponent.Socket.GetStream();
                    if (stream.CanWrite)
                    {
                        byte[] vs = Encoding.ASCII.GetBytes(message);
                        try
                        {
                            stream.Write(vs, 0, vs.Length);
                        }
                        catch (Exception ex)
                        {
                            Toolbox.LogError(ex);
                        }

                        // Message send
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Toolbox.LogError(ex);
                }
            }
            return false;
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>string : the message received</returns>
        public string Receive()
        {
            try
            {
                if (State == ManagedState.Binded || State == ManagedState.InGame)
                {
                    var stream = this.Socket.GetStream();
                    if (stream.CanRead)
                    {
                        byte[] buffer = new byte[1024];
                        try
                        {
                            stream.Read(buffer, 0, buffer.Length);
                        }
                        catch (Exception ex)
                        {
                            Toolbox.LogError(ex);
                        }

                        // Message send
                        return Encoding.UTF8.GetString(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }

            // Nothing could be read
            return string.Empty;
        }
    }
}
