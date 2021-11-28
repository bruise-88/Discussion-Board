﻿/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the test communicator
///		for testing purpose
/// </summary>

using Networking;
using System;

namespace Testing.Dashboard.TestModels
{
    public class TestCommunicator : ICommunicator
    {
        public TestCommunicator()
        {
            sentData = null;
            isCommunicatorStopped = false;
        }

        public void AddClient<T>(string clientID, T socketObject)
        {
            clientCount++;
        }

        public void RemoveClient(string clientID)
        {
            throw new NotImplementedException();
        }

        public void Send(string data, string identifier)
        {
            sentData = data;
        }

        public void Send(string data, string identifier, string destination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// start function for testing room creation
        /// </summary>
        /// <returns> string port and IP needed for testing </returns>
        public string Start(string serverIP = null, string serverPort = null)
        {
            if (serverIP == null && serverPort == null)
                return ipAddressAndPort;
            if (serverIP + ":" + serverPort == ipAddressAndPort)
                return "1";
            return "0";
        }

        public void Stop()
        {
            isCommunicatorStopped = true;
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority = 1)
        {

        }

        public int clientCount;
        public string ipAddressAndPort;
        public string sentData;
        public bool isCommunicatorStopped;
    }
}
