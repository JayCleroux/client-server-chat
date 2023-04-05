using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


class Program
{
    /// <summary>
    /// Simple program that will open a tcp client and chat on the server using a stream
    /// </summary>
    static void Main(string[] args)
    {
        try
        {
            
            // Buffer to store the response bytes.
            Byte[] data = new byte[256];
            Byte[] bytes = new Byte[256];

            String dat = null;

            int port = 5000, j;
            // parse the ip address
            IPAddress server = IPAddress.Parse("127.0.0.1");
            // Create a TcpClient.
            TcpClient client = new TcpClient("127.0.0.1", port);
            // welcome message
            Console.WriteLine("Welcome Client");
            Console.WriteLine("Please press I to input message, escape to exit or any other key to read message.");

            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();
        input:
            // check if key is available
            if(Console.KeyAvailable) 
            {// blocking statement
                ConsoleKeyInfo userKey = Console.ReadKey(true);
                // if user presses I, they will enter insertion mode
                while (userKey.Key == ConsoleKey.I)
                {                 
                    Console.Write("Insertion Mode >> "); // tell user they're now in insertion mode
                    String str = Console.ReadLine(); // get the string to send
                    Stream stm = client.GetStream(); // get the stream
                    ASCIIEncoding asen = new ASCIIEncoding(); // use ASCII endcoding
                    byte[] ba = asen.GetBytes(str); // cast the message as bytes
                    stm.Write(ba, 0, ba.Length); // send the message to the stream
                    break; // break 
                } // if the user presses escape they will close the stream and client
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
            while ((j = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                dat = System.Text.Encoding.ASCII.GetString(bytes, 0, j);
                Console.WriteLine("Server: {0}", dat); // write to console
                goto input;
            }
        } // go back to the input
        goto input;
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }
}
