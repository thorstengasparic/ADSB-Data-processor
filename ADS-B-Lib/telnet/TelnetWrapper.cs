using System.Drawing;
using System;
using System.Net.Sockets;
using System.Text;
using ADS_B_Lib.dataprocessing;

public  class MessageReceiver
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly Task _task;
    private readonly CancellationTokenSource cancelationTokenSource;
    private readonly IADSB_DataProcessor dataProcessor;

    private const int bufferSize = ushort.MaxValue; // 65536

    public MessageReceiver(TcpClient client, IADSB_DataProcessor dataProcessor, CancellationTokenSource? cancellationTokenSource)
    {
        if (cancellationTokenSource == null) return;
        _client = client;
        _stream = _client.GetStream();

        this.cancelationTokenSource = cancelationTokenSource;

        _task = RawReceiveAsync(cancelationTokenSource.Token);

        this.dataProcessor = dataProcessor;
        Console.WriteLine($"Client connected: {_client.Client.RemoteEndPoint}");
    }

    public void Start()
    {
        _task.Start();
    }

    public async Task SendAsync(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        ushort size = (ushort)data.Length;
        await _stream.WriteAsync(BitConverter.GetBytes(size)).ConfigureAwait(false);
        await _stream.WriteAsync(data).ConfigureAwait(false);
    }

    void ProcessRawBuffer(byte[] rawoutput)
    {
        this.dataProcessor.RawData(rawoutput);
    }

    public async Task RawReceiveAsync(CancellationToken token)
    {
        byte[] buffer = new byte[bufferSize];
        try
        {
            while (true)
            {
                await _stream.ReadAsync(buffer.AsMemory(0, 2), token).ConfigureAwait(false);
                int size = BitConverter.ToUInt16(buffer, 0);
                int offset = 0;

                while (offset < size)
                {
                    int received = await _stream.ReadAsync(buffer.AsMemory(offset, size - offset), token).ConfigureAwait(false);
                    if (received == 0)
                    {
                        Console.WriteLine($"Client {_client.Client.RemoteEndPoint} disconnected.");
                        return;
                    }
                    offset += received;
                }
                ProcessRawBuffer(buffer.AsSpan(0, size).ToArray());
            }
        }
        catch (OperationCanceledException)
        {
            if (_client.Connected)
            {
                _stream.Close();
                _client.Close();
                Console.WriteLine($"Connection to {_client.Client.RemoteEndPoint} closed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public void Close()
    {
        cancelationTokenSource.Cancel();
        _task.Wait();
    }
}