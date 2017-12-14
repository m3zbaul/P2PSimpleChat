using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

namespace P2PSimpleChat.BLL
{
    public class Client
    {
        //Stores the TcpClient
        private TcpClient client;
        private NetworkStream netstream;

        //Defines if the client shuld look for incoming data
        private bool listen = true;
        public bool Listen
        {
            get { return this.listen; }
            set { this.listen = value; }
        }
        public static Exception ex;

        //Stores clientID. ClientID = clientCount on connection time
        public int id;

        //Event to pass recived data to the server class
        public delegate void internalGotDataFromClientHandler(object sender, byte[] message);
        public event internalGotDataFromClientHandler internalGotDataFromClient;

        //Constructor
        public Client(TcpClient client)
        {
            //Assain members
            this.client = client;
            this.id = 0;

            //Init the StreamWriter
            netstream = this.client.GetStream();
            new Thread(() =>
            {
                StartListener(netstream);
            }).Start();
        }
        //Reads data from the connection and fires an event wih the recived data
        public void StartListener(NetworkStream myNetworkStream)
        {
            //While we should look for new data
            while (listen)
            {
                if (myNetworkStream.CanRead)
                {

                    byte[] myReadBuffer = new byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int numberOfBytesRead = 0;
                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        try
                        {
                            numberOfBytesRead = myNetworkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        }
                        catch(Exception ex)
                        {
                            try
                            {
                                internalGotDataFromClient(this, new byte[] { 2, 1, 9 });
                                Close();
                            }
                            catch
                            {
                            }
                            // remote connection ended forcefully
                            P2PSimpleChat.BLL.Client.ex = ex;
                            
                            return;
                        }
                        myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));

                    }
                    while (myNetworkStream.DataAvailable);

                    if (myCompleteMessage == null || myCompleteMessage.Length < 3)
                    {
                        try
                        {
                            internalGotDataFromClient(this, new byte[] { 2, 1, 9 });
                        }
                        catch(Exception ex)
                        {
                            P2PSimpleChat.BLL.Client.ex = ex;
                            Close();
                            return;
                        }

                        Close();
                        return;
                    }
                    // Print out the received message to the console.
                    internalGotDataFromClient.Invoke(this, Encoding.UTF8.GetBytes(myCompleteMessage.ToString()));
                }
                else
                {
                    break;
                }
            }
        }

        //Sends the string "data" to the client
        public void SendMessage(string data)
        {
            if (netstream.CanWrite)
            {
                byte[] myWriteBuffer = Encoding.UTF8.GetBytes(data);
                byte[] finalBuffer = new byte[myWriteBuffer.Length+3];
                finalBuffer[0] = 2;
                finalBuffer[1] = 1;
                finalBuffer[2] = 2;
                // append message header
                System.Array.Copy(myWriteBuffer, 0, finalBuffer, 3, myWriteBuffer.Length);
                netstream.Write(finalBuffer, 0, finalBuffer.Length);
            }
            else
            {
                //Console.WriteLine("Sorry.  You cannot write to this NetworkStream.");
            }
        }
        /*private void SendFile()
        {

            var bytesToSend = File.ReadAllBytes(fileName);
            var header = BitConverter.GetBytes(bytesToSend.Length);
            ns.Write(header, 0, header.Length);
            ns.Write(bytesToSend, 0, bytesToSend.Length);
        }*/
        public void SendFile(string path)
        {
            if (netstream.CanWrite)
            {
                byte[] myWriteBuffer = Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.IO.File.ReadAllBytes(path)));
                byte[] finalBuffer = new byte[myWriteBuffer.Length + 4];
                finalBuffer[0] = 2;
                finalBuffer[1] = 1;
                finalBuffer[2] = 3;
                if (path.EndsWith("png"))
                {
                    finalBuffer[3] = 1;
                }
                else if (path.EndsWith("txt"))
                {
                    finalBuffer[3] = 2;
                }
                else
                {
                    finalBuffer[3] = 3;
                }
                // append message header
                System.Array.Copy(myWriteBuffer, 0, finalBuffer, 4, myWriteBuffer.Length);
                netstream.Write(finalBuffer, 0, finalBuffer.Length);
            }
            else
            {
                //Console.WriteLine("Sorry.  You cannot write to this NetworkStream.");
            }
        }
        public void SendRequest(string myIp, string myPort)
        {
            if (netstream.CanWrite)
            {
                byte[] myWriteBuffer = Encoding.UTF8.GetBytes(myIp+":"+myPort);
                byte[] finalBuffer = new byte[myWriteBuffer.Length + 3];
                finalBuffer[0] = 2;
                finalBuffer[1] = 1;
                finalBuffer[2] = 1;
                // append message header
                System.Array.Copy(myWriteBuffer, 0, finalBuffer, 3, myWriteBuffer.Length);
                netstream.Write(finalBuffer, 0, finalBuffer.Length);
            }
            else
            {
            }
        }
        public void SendDisconnect()
        {
            if (netstream.CanWrite)
            {
                byte[] finalBuffer = new byte[3];
                finalBuffer[0] = 2;
                finalBuffer[1] = 1;
                finalBuffer[2] = 9;
                netstream.Write(finalBuffer, 0, finalBuffer.Length);
            }
            else
            {
            }
        }
        public void SendTyping()
        {
            if (netstream.CanWrite)
            {
                byte[] finalBuffer = new byte[3];
                finalBuffer[0] = 2;
                finalBuffer[1] = 1;
                finalBuffer[2] = 4;
                netstream.Write(finalBuffer, 0, finalBuffer.Length);
            }
            else {
                //Console.WriteLine("Sorry.  You cannot write to this NetworkStream.");
            }
        }

        //Closes the connection
        public void Close()
        {
            listen = false;
            netstream.Close();
        }
        public static Client GetClientObject(string ip, string port)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), int.Parse(port)));
                return new Client(client);
            }
            catch
            {
                throw;
            }
        }
    }
}