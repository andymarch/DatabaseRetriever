using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;

namespace DatabaseRetrieverService
{
    public partial class DatabaseRetriever : ServiceBase
    {
        private TcpListener server;
        private TcpClient client;

        public DatabaseRetriever()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
              // Set the TcpListener on port 13000.
              Int32 port = 14747;
              IPAddress localAddr = IPAddress.Parse("127.0.0.1");
              
              // TcpListener server = new TcpListener(port);
              server = new TcpListener(localAddr, port);

              // Start listening for client requests.
              server.Start();
                 
              // Buffer for reading data
              Byte[] bytes = new Byte[256];
              String data = null;

              // Enter the listening loop.
              while(true) 
              {
                Console.Write("Waiting for a connection... ");
                
                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                client = server.AcceptTcpClient();            
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while((i = stream.Read(bytes, 0, bytes.Length))!=0) 
                {   
                  // Translate data bytes to a ASCII string.
                  data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                  Console.WriteLine(String.Format("Received: {0}", data));
               
                  // Process the data sent by the client.
                  data = data.ToUpper();

                  byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                  // Send back a response.
                  stream.Write(msg, 0, msg.Length);
                  Console.WriteLine(String.Format("Sent: {0}", data));            
                }
                 
                // Shutdown and end connection
                client.Close();
              }
            }
            catch(SocketException e)
            {
              Console.WriteLine("SocketException: {0}", e);
            }
              
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
          } 

        protected override void OnStop()
        {
            client.Close();
            server.Stop();
        }
    }
}
