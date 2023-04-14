// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;

Int32 port = 30003;

// Server Hostname
TcpClient client = new TcpClient("192.168.178.74", port);



Connection con = new Connection(client);
await con.ReceiveAsync(new CancellationToken() );
Console.WriteLine(con.ToString());
await con.SendAsync("2");
Console.WriteLine(con.ToString());
await con.SendAsync("3");
Console.WriteLine(con.ToString());
await con.SendAsync("4");
Console.WriteLine(con.ToString());




