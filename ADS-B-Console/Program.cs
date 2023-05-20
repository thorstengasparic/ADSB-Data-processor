// See https://aka.ms/new-console-template for more information
using ADS_B_Lib.dataprocessing;
using System.Net.Sockets;

Int32 port = 30003;

// Server Hostname
TcpClient client = new TcpClient("192.168.178.74", port);

IMessageFIFO messageFIFO = FIFOFactory.Create();
IADSB_DataProcessor dataProcessr = DataProcessorFactory.Create(messageFIFO);
    
MessageReceiver con = new MessageReceiver(client, dataProcessr, new CancellationTokenSource());




await con.RawReceiveAsync(new CancellationToken() );





