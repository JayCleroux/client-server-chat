using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;


class Program
{
    /// <summary>
    /// Simple program that will create a TcpListener and chat with a client on the same address
    /// </summary>
    public static void Main()
    {
        // parse the ip address
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        // declare and initialize the listener
        TcpListener server = null;
        // declare and initialize the port number
        int port = 5000;
        try
        {
            // declare the listener
            server = new TcpListener(localAddr, port);
            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            // string buffer for reading data
            String data = null;
            // welcome message and UI instructions
            Console.WriteLine("Welcome to Server!");
            Console.WriteLine("Please press I to input message, escape to exit or any other key to read message.");
            // Enter the listening loop.
            while (true)
            {
                data = null;
                int i;
            WaitForConnection:
                Console.Write("Waiting for a client connection... ");
                // wait for connection from tcp client
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");               



                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();              

            input:
                // check if there is a key available
                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo userKey = Console.ReadKey(true);
                    while (userKey.Key == ConsoleKey.I)
                    {
                        Console.Write("Insertion Mode >> ");
                        String str = Console.ReadLine();
                        Stream stm = client.GetStream(); // get the stream
                        ASCIIEncoding asen = new ASCIIEncoding(); // use ASCII encoding
                        byte[] ba = asen.GetBytes(str); // cast the message as bytes
                        //Console.WriteLine("Server: {0}", str);
                        stm.Write(ba, 0, ba.Length); // write to the stream
                        break;
                    } // if user would like to exit they will press escape
                    if (userKey.Key == ConsoleKey.Escape)
                    {
                        stream.Close();
                        client.Close();
                        return;
                    }
                }
                
                // if there is data in the stream read it
                if(stream.DataAvailable == true) 
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Client: {0}", data); // write to console
                        goto input;                                        
                    }
                }

                // check if there is an available connection. reference - https://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available

                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();
                if (tcpConnections != null && tcpConnections.Length > 0)
                {
                    TcpState stateOfConnection = tcpConnections.First().State;
                    if (stateOfConnection == TcpState.Established)
                    {
                        goto input;
                    }
                    else
                    {
                        Console.WriteLine("Client disconnected.");
                        goto WaitForConnection;
                    }

                } // go back to the input
                goto input;
            }
        } // error handling
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }
    }
}
