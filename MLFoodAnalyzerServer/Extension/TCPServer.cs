using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MLFoodAnalyzerServer.Extension;

public class TCPServer
{
    private int port;
    private int timeout;
    private IPAddress ip;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private TcpListener tcpListener;
    private static DateTime startUserOperation;
    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully";
    private Encryption? encryption = MLFoodAnalyzerServer.encryption;


    public TCPServer(int port = 55555, int timeout = 10000)
    {
        this.port = port;
        stream = null!;
        tcpClient = new();
        this.timeout = timeout;
        ip = GetIp();
        tcpListener = new TcpListener(ip, port);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler!);
    }


    protected void MyHandler(object sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("\nServer close.");
        args.Cancel = true;

        if(stream != null) stream.Close();
        if (tcpClient != null) tcpClient.Close();
        if (tcpListener != null) tcpListener.Stop();
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
                tcpClient.SendTimeout = timeout;
                startUserOperation = DateTime.Now;
                Console.WriteLine($"[{startUserOperation}] Client {tcpClient.Client.RemoteEndPoint} connected to server");
                stream = tcpClient.GetStream();
                //stream.ReadTimeout = timeout;
                //stream.WriteTimeout = timeout;
                _ = Task.Run(GetCommand);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            tcpListener.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
    }

    private async Task Send(string? text) => await stream.WriteAsync(Encoding.UTF8.GetBytes(text + '\0'));
    private async Task Send(string?[] text) => await stream.WriteAsync(Encoding.UTF8.GetBytes(string.Join("\n", text) + '\0'));

    private async Task GetCommand()
    {
        int bytesRead;
        var response = new List<byte>();
        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            response.Add((byte)bytesRead);
        }
        var word = Encoding.UTF8.GetString(response.ToArray());

        switch (word)
        {
            case "IMAGE":
                await ProcessImage(MLFoodAnalyzerServer.store!.GetPath());
                break;
            case "TEXT":
                await ProcessText();
                break;
            case "PING":
                await PingServer();
                break;
            case "LOGIN":
                await LogIn();
                break;
            case "FOOD":
                await GetAllFood();
                break;
            case "History":
                await GetAllHistory();
                break;
            case "Update":
                await UpdateFood();
                break;
            default:
                Stop();
                break;
        }
    }

    private async Task PingServer()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a ping");
        string message = "SUCCESS\0";
        await Send(message);
        Stop();
    }

    private async Task ProcessImage(string? folderPath)
    {
        int bytesRead;
        byte[] buffer = new byte[1024];
        var response = new List<byte>();
        string[] files = Directory.GetFiles(folderPath!);
        int numberOfFiles = files.Length;
        long sum = 0;

        string? fileName = $"{MLFoodAnalyzerServer.store!.GetName()}_{numberOfFiles}.{MLFoodAnalyzerServer.store.GetFormat()}";
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a picture \"{fileName}\"");

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        string word = Encoding.UTF8.GetString(response.ToArray());
        long sizeImage = long.Parse(word);

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
        string?[] message = mLFood.SetImage(filePath);
        message = await MLFoodAnalyzerServer.database!.SelectDescriptionFood(message!);
        await Send(message);
        Stop();
    }

    private async Task ProcessText()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a text");

        int bytesRead;
        List<byte> response = [];
        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        string word = Encoding.UTF8.GetString(response.ToArray());
        MLFood mLFood = new();
        string?[] message = mLFood.SetText(word);
        message = await MLFoodAnalyzerServer.database!.SelectDescriptionFood(message!);
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

    private async Task LogIn()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a login");

        int bytesRead;
        List<byte> response = [];
        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        string word = Encoding.UTF8.GetString(response.ToArray());
        word = encryption!.DecryptText(word);
        string[] textSplit = word.Split('|');
        string? message = await MLFoodAnalyzerServer.database!.DBLogIn(Encryption.ConvertToHash(textSplit[0]), Encryption.ConvertToHash(textSplit[1]));
        message = encryption.EncryptText(message!);
        await Send(message);
        Stop();
    }

    private async Task GetAllFood()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a foods");

        string? message = await MLFoodAnalyzerServer.database!.FoodSelect();
        await Send(message);
        Stop();
    }

    private async Task GetAllHistory()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a history");

        int bytesRead;
        List<byte> response = [];
        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        string word = Encoding.UTF8.GetString(response.ToArray());
        string? message = await MLFoodAnalyzerServer.database!.History(int.Parse(word));
        await Send(message);
        Stop();
    }

    private async Task UpdateFood()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested an update food");

        int bytesRead;
        List<byte> response = [];
        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        string word = Encoding.UTF8.GetString(response.ToArray());
        string[] textSplit = word.Split('|');
        string? message = await MLFoodAnalyzerServer.database!.UpdateDescriptionFood(textSplit[0], int.Parse(textSplit[1]), textSplit[2]);
        await Send(message);
        Stop();
    }

    public string GetInfo() => $"IP: {ip}\nPort: {port}\nTimeout: {timeout} ms";

    public IPAddress GetIp()
    {
        /*string Hostname = Environment.MachineName;
        IPHostEntry Host = Dns.GetHostEntry(Hostname);

        foreach (IPAddress IP in Host.AddressList)
        {
            if (IP.AddressFamily == AddressFamily.InterNetwork)
            {
                return IP;
            }
        }
        return IPAddress.Any;*/


        string Hostname = Environment.MachineName;
        IPHostEntry Host = Dns.GetHostEntry(Hostname);
        return Host.AddressList[^1];
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