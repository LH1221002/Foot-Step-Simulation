﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace HapticShoes
{
    public class ServerHandler : IDisposable
    {
        private BackgroundWorker serverWorker = new BackgroundWorker();
        public event EventHandler ServerResponse;

        private TcpClient socket;
        private NetworkStream networkStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public void ConnectToServer(string Address, int Port)
        {
            socket = new TcpClient(Address, Port);

            networkStream = socket.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream) { NewLine = "\n", AutoFlush = true };

            serverWorker.WorkerSupportsCancellation = true;
            serverWorker.DoWork += new DoWorkEventHandler(ServerWorker);
        }

        public bool IsConnected
        {
            get { return socket.Connected; }
        }

        public void StartListening()
        {
            if (ServerResponse == null)
                throw new Exception("There is no event to intercept the response!");

            if (!serverWorker.IsBusy)
                serverWorker.RunWorkerAsync();
        }

        //\item 0: Send Properties
        //\item 1: Calibrate maximal weight
        //\item 2: Calibrate minimal weight
        //\item 4: Activate Feedback
        //\item 5: Disable Feedback
        //\item 6: Static Vibration
        //\item 7: Exploding Vibration
        //\item 8: Hit Vibration

        public void SendMessage(string message)
        {
            Debug.Log(message);
            streamWriter.WriteLine(message);
            streamWriter.Flush();
        }

        private void ServerWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (true)
            {
                if (this.serverWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    string serverResponse = streamReader.ReadLine();

                    if (serverResponse != "")
                        ServerResponse(serverResponse, new EventArgs());
                }
                catch
                {

                }
            }
        }

        public void Dispose()
        {
            serverWorker.WorkerSupportsCancellation = true;
            serverWorker?.CancelAsync();
            serverWorker?.Dispose();
            networkStream?.Dispose();
            streamReader?.Dispose();
            streamWriter?.Dispose();
            socket?.Dispose();
        }
    }
}
