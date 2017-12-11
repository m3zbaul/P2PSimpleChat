using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace P2PSimpleChat.BLL
{
    public class Server
    {

        private Client client;

        private static bool serverStartedSuccessfully = false;
        public static bool ServerStartedSuccessfully
        {
            get { return Server.serverStartedSuccessfully; }
        }
        private static string errorMessage;
        public static string ErrorMessage
        {
            get { return Server.errorMessage; }
        }

        private bool running;
        public bool Running
        {
            get { return this.running; }
            set { this.running = value; }
        }
        private TcpListener listener;

        //Event to pass recived data to the main class
        public delegate void ClientDataReceivedHandler(object sender, byte[] msg);
        public event ClientDataReceivedHandler ClientDataReceived;


        public Server(string ip, string port, bool autoStart = true)
        {
            this.listener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
            this.running = true;

            if (autoStart)
            {
                this.Run();
            }
        }

        //Starts the server.
        public void Run()
        {
            new Thread(() =>
            {
                try
                {
                    listener.Start();
                    serverStartedSuccessfully = true;
                    errorMessage = null;
                    
                }
                catch(Exception ex)
                {
                    serverStartedSuccessfully = false;
                    errorMessage = ex.Message;
                    return;
                }
                

                //While the server should run
                while (running)
                {
                    //Check if someone wants to connect
                    if (listener.Pending())
                    {
                        try
                        {
                            Client client = new Client(listener.AcceptTcpClient());
                            this.client = client;
                        }
                        catch
                        {
                            GotDataFromClient(this, new byte[] { 2, 1, 9 });
                        }
                        //Declare event
                        client.internalGotDataFromClient += GotDataFromClient;
                    }
                    else
                    {
                        Thread.Sleep(150);
                    }
                }
                this.listener.Stop();
                this.Stop();
            }).Start();
        }

        //Fires event for the user
        private void GotDataFromClient(object sender, byte[] data)
        {
            ClientDataReceived(sender, data);
        }
        //Stop server
        public void Stop()
        {
            this.running = false;
            if (this.client != null)
            {
                this.client.Close();
            }
            
        }
        public void CloseClient()
        {
            if (this.client != null)
            {
                try
                {
                    this.client.Close();
                }
                catch
                {
                }
                this.client = null;
            }
        }
    }
}