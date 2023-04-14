using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;   // TCP-streaming
using System.Threading;     // the sleeping part...

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                /* *******************   Commands  ************************ */

                // Password
                string password = "test";

                // Command
                string command = "kill ADMIN";  // only chanche to see if it work`s ;)

                /* *******************   Server config  ******************* */

                // Server Port
                Int32 port = 5536;

                // Server Hostname
                TcpClient client = new TcpClient("192.168.10.1", port);

                /* *******************   Conversion  ********************** */

                // Convert password in ASCII and write it into a byte-array              
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(password);

                // Convert command in ASCII and write it into a byte-array                
                Byte[] data2 = System.Text.Encoding.ASCII.GetBytes(command);

                /* *******************   Initialisation  ****************** */

                // Client-Stream
                NetworkStream stream = client.GetStream();

                // Buffer size
                data = new Byte[256];
                data2 = new Byte[4096];

                /* *******************   Send commands  ******************* */

                // Send password
                Console.WriteLine("Sende Passwort\n");
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Passwort gesendet\n");

                // Just wait a sec...
                Console.WriteLine("Warte 1 Sekunde\n");
                Thread.Sleep(1000);
                Console.WriteLine("1 Sekunde gewartet\n");

                // Send command
                Console.WriteLine("Sende Befehl\n");
                stream.Write(data2, 0, data2.Length);
                Console.WriteLine("Befehl gesendet\n");

                /* *******************   Recieve  ************************* */

                // Strings for ASCII stream
                String responseData = String.Empty;
                String responseData2 = String.Empty;

                // Read first stream
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                // First stream output
                Console.WriteLine(responseData);

                // Read second steam
                Int32 bytes2 = stream.Read(data2, 0, data2.Length);
                responseData2 = System.Text.Encoding.ASCII.GetString(data2, 0, bytes);

                // Second stream output
                Console.WriteLine(responseData2);

                /* *******************   Close  **************************** */

                // Close stream/TCP-client
                stream.Close();
                client.Close();

                /* *******************   Error handling  ******************** */

            } // End try
            catch (Exception e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            } // End catch

            // Don`t close the console...
            Console.WriteLine("\n Press Enter...");
            Console.Read();

        } // End main
    } // End class
} // End namespace