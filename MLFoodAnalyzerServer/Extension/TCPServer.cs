using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerServer.Extension;

internal class TCPServer
{
    private int port;
    private int timeout;
    private readonly IPAddress ip;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private readonly TcpListener tcpListener;
    // private readonly Settings settings;
    private readonly Store store;
    private static DateTime startUserOperation;
    private readonly string success = "Settings applied sucessfully";
    private readonly string unsuccess = "Settings applied unsucessfully";

    public TCPServer(int port = 55555, int timeout = 10000)
    {
        this.port = port;
        stream = null!;
        tcpClient = new();
        this.timeout = timeout;
        ip = GetIp();
        tcpListener = new TcpListener(ip, port);
        // settings = MLFoodAnalyzerServer.settings;
        store = MLFoodAnalyzerServer.store;
    }

    public async Task Run()
    {
        try
        {
            tcpListener.Start();
            Console.WriteLine("The server is running. Waiting for connections... ");
            while (true)
            {
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                tcpClient.ReceiveTimeout = timeout;
                startUserOperation = DateTime.Now;
                Console.WriteLine($"[{startUserOperation}] Client {tcpClient.Client.RemoteEndPoint} connected to server");
                stream = tcpClient.GetStream();
                _ = Task.Run(GetCommand);
            }
        }
        finally
        {
            tcpListener.Stop();
        }
    }

    private async Task Send(string text)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes(text + '\0'));
    }

    private async Task GetCommand()
    {
        int bytesRead;

        var response = new List<byte>();
        // считываем данные до конечного символа
        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }
        var word = Encoding.UTF8.GetString(response.ToArray());

        switch (word)
        {
            case "IMAGE":
                await ProcessImage(store.GetPath());
                break;
            case "TEXT":
                await ProcessText();
                break;
            default:
                Stop();
                break;
        }
    }

    private async Task ProcessImage(string folderPath)
    {
        // Get all the files in the folder
        int bytesRead;
        byte[] buffer = new byte[1024];
        var response = new List<byte>();
        string[] files = Directory.GetFiles(folderPath);
        int numberOfFiles = files.Length;
        long sum = 0;

        string fileName = $"{store.GetName()}_{numberOfFiles}.{store.GetFormat()}";
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a picture \"{fileName}\"");

        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }
        long sizeImage = long.Parse(Encoding.UTF8.GetString(response.ToArray()));


        string filePath = @$"{folderPath}\{fileName}";
        FileStream? fileStream;
        using (fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                fileStream.Write(buffer, 0, bytesRead);
                sum += bytesRead;
            } while (sizeImage != sum);
        }

        MLFood mLFood = new();
        string message = mLFood.SetImage(filePath);
        // Message send
        await Send(message);
        Stop();
    }

    private async Task ProcessText()
    {
        MLFood mLFood = new();
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a text");
        // Text
        string message = mLFood.SetText("Nothing)");
        message += '\0';
        await Send(message);
        Stop();
    }

    private void Stop()
    {
        DateTime end = DateTime.Now;
        TimeSpan difference = end.Subtract(startUserOperation);
        Console.WriteLine($"[{end}] Client {tcpClient.Client.RemoteEndPoint} disconnect and waste {difference} milliseconds");
        //DirectorySettings();

        stream.Close();
    }


    public string GetInfo() => $"IP: {ip}\nPort: {port}\nTimeout: {timeout}";

    private static IPAddress GetIp()
    {
        string Hostname = Environment.MachineName;
        IPHostEntry Host = Dns.GetHostEntry(Hostname);

        foreach (IPAddress IP in Host.AddressList)
        {
            if (IP.AddressFamily == AddressFamily.InterNetwork)
            {
                return IP;
            }
        }
        return IPAddress.Any;
    }


    public string GetPort() => port.ToString();

    public string GetTimeout() => timeout.ToString();


    public string SetPort(string text) => IsValidPort(text) ? success : unsuccess;

    public string SetTimeout(string text) => IsValidTimeout(text) ? success : unsuccess;


    private bool IsValidPort(string text)
    {
        _ = int.TryParse($"{text:D}", out int outputParse);
        if (outputParse >= 49152 && outputParse <= 65535)
        {
            port = outputParse;
            return true;
        }
        return false;
    }

    private bool IsValidTimeout(string text)
    {
        _ = int.TryParse($"{text:D}", out int outputParse);
        if (outputParse >= 0 && outputParse <= 10_000_000)
        {
            timeout = outputParse;
            return true;
        }
        return false;
    }
}





/*
                  ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/