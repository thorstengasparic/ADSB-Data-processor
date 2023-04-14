using System.Net.Sockets;
using System.Text;
public  class Connection
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly Task _task;
    private readonly CancellationTokenSource _cts;

    private const int bufferSize = ushort.MaxValue; // 65536

    public Connection(TcpClient client)
    {
        _client = client;
        _stream = _client.GetStream();
        _cts = new CancellationTokenSource();
        _task = ReceiveAsync(_cts.Token);
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

    public async Task ReceiveAsync(CancellationToken token)
    {
        byte[] buffer = new byte[bufferSize];
        try
        {
            while (true)
            {
                await _stream.ReadAsync(buffer.AsMemory(0, 2), token).ConfigureAwait(false);
                int size = BitConverter.ToUInt16(buffer, 0);
                int offset = 0;

                // normally there will be only one iteration of this loop but
                // ReadAsync doesn't guarantee that 'received' will always match
                // requested bytes amount
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

                // probably firing an event here would be helpful
                // byte[] output = buffer.AsSpan(0, size).ToArray();
                var data = Encoding.UTF8.GetString(buffer.AsSpan(0, size));
                Console.WriteLine($"Data received: {data}");
            }
        }
        catch (OperationCanceledException)
        {
            if (_client.Connected)
            {
                _stream.Close();
                Console.WriteLine($"Connection to {_client.Client.RemoteEndPoint} closed.");
            }
        }
        catch (Exception ex)
        {
            // Test the class a lot with closing the connection on both sides
            // I'm not sure how it will behave because I didn't test it
            Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public void Close()
    {
        _cts.Cancel();
        _task.Wait();
    }
}