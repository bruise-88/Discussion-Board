﻿/// <author>Tausif Iqbal</author>
/// <created>13/10/2021</created>
/// <summary>
///     This file contains the class definition of ClientCommunicator.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    internal class ClientCommunicator : ICommunicator
    {
        // Declare queue variable for receiving messages
        private readonly Queue _receiveQueue = new();

        // Declare queue variable for sending messages
        private readonly Queue _sendQueue = new();
        private readonly Dictionary<string, INotificationHandler> _subscribedModules = new();

        // Declare socket object for client
        private TcpClient _clientSocket;

        private ReceiveQueueListener _receiveQueueListener;

        //Declare ReceiveSocketListener variable for listening messages 
        private ReceiveSocketListener _receiveSocketListener;

        // Declare sendSocketListenerClient variable for sending messages 
        private SendSocketListenerClient _sendSocketListenerClient;

        /// <summary>
        ///     This method connects client to server
        /// </summary>
        /// <param name="serverIp"> Ip of server</param>
        /// <param name="serverPort"> port of server</param>
        /// <returns> String </returns>
        string ICommunicator.Start(string serverIp, string serverPort)
        {
            try
            {
                //try to connect with server
                var ip = IPAddress.Parse(serverIp);
                var port = int.Parse(serverPort);
                _clientSocket = new TcpClient();
                // _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
                _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                _clientSocket.Connect(ip, port);

                //Start receiveSocketListener of the client  
                //for listening message from the server
                _receiveSocketListener = new ReceiveSocketListener(_receiveQueue, _clientSocket);
                _receiveSocketListener.Start();

                //start sendSocketListener of client for sending message 
                _sendSocketListenerClient = new SendSocketListenerClient(_sendQueue, _clientSocket);
                _sendSocketListenerClient.Start();

                _receiveQueueListener = new ReceiveQueueListener(_receiveQueue, _subscribedModules);
                _receiveQueueListener.Start();

                return "1";
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Networking] {e.Message}");
                return "0";
            }
        }

        /// <summary>
        ///     This method stops all the running thread
        ///     of client and closes the connection
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Stop()
        {
            if (!_clientSocket.Connected) return;
            // stop the listener of the client 
            _sendSocketListenerClient.Stop();
            _receiveSocketListener.Stop();
            _receiveQueueListener.Stop();

            //close stream  and connection of the client
            _clientSocket.GetStream().Close();
            _clientSocket.Close();
        }

        void ICommunicator.AddClient<T>(string clientId, T socketObject)
        {
            throw new NotSupportedException();
        }

        void ICommunicator.RemoveClient(string clientId)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     This method is for sending message
        /// </summary>
        /// <param name="data"> data to be sent</param>
        /// <param name="identifier"> module Id </param>
        /// <returns> void </returns>
        void ICommunicator.Send(string data, string identifier)
        {
            var packet = new Packet {ModuleIdentifier = identifier, SerializedData = data};
            try
            {
                _sendQueue.Enqueue(packet);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Networking] {ex.Message}");
                throw;
            }
        }

        void ICommunicator.Send(string data, string identifier, string destination)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     This method registers different handler
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Subscribe(string identifier, INotificationHandler handler, int priority)
        {
            _subscribedModules.Add(identifier, handler);
            _sendQueue.RegisterModule(identifier, priority);
            _receiveQueue.RegisterModule(identifier, priority);
            Trace.WriteLine(
                $"[Networking] Module Registered with ModuleIdentifier: {identifier} and Priority: {priority.ToString()}");
        }
    }
}