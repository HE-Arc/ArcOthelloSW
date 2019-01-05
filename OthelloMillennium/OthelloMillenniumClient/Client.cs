using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OthelloMillenniumServer
{
    public class Client
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

        // Time informations
        public DateTime RegisterDateTime { get; private set; }

        public Client(string serverHostname, int serverPort)
        {
            Socket = new TcpClient();
            State = ManagedState.Undefined;

            // Register this client to the server
            Socket.Connect(serverHostname, serverPort);

            // Wait for the confirmation to be registred
            Task waitForRegister = new Task(() =>
            {
                var stream = Socket.GetStream();
                while(true)
                {
                    if(stream.CanRead)
                    {
                        
                        byte[] buffer = new byte[4];

                        // Read the stream
                        string output = this.Receive();

                        if(!string.IsNullOrEmpty(output))
                        {
                            if(output == Toolbox.Protocole.RegisterSuccessful)
                            {
                                this.State = ManagedState.Searching;
                                break;
                            }
                            else
                            {
                                Toolbox.LogError(new Exception("Wrong message received!\r\nWaiting for RS"));
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            });

            //TODO handle in game data process
            
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">What to transfer</param>
        public void Send(string message)
        {
            try
            {
                var stream = Socket.GetStream();
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
                }
            }
            catch (Exception ex)
            {
                Toolbox.LogError(ex);
            }
        }

        /// <summary>
        /// Translate the message waiting on the socket
        /// </summary>
        /// <returns>string : the message received</returns>
        public string Receive()
        {
            try
            {
                var stream = Socket.GetStream();
                if (stream.CanRead)
                {
                    // Because a character is 2 byte and every message from the protocole is 2 char long
                    byte[] buffer = new byte[4];
                    try
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        Toolbox.LogError(ex);
                    }

                    // Return the decode message
                    return Encoding.UTF8.GetString(buffer);
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
