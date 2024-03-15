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
    // private readonly Settings settings;
    private Store store;
    private Database database;
    private static DateTime startUserOperation;
    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully";


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
        database = MLFoodAnalyzerServer.database;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
    }


    protected void myHandler(object sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("\nThe read operation has been interrupted.");
        Console.WriteLine($"  Key pressed: {args.SpecialKey}");
        Console.WriteLine($"  Cancel property: {args.Cancel}");
        // Set the Cancel property to true to prevent the process from terminating.
        Console.WriteLine("Setting the Cancel property to true...");
        args.Cancel = true;
        // Announce the new value of the Cancel property.
        Console.WriteLine($"  Cancel property: {args.Cancel}");
        Console.WriteLine("The read operation will resume...\n");


        stream.Close();
        tcpClient.Close();
        tcpListener.Stop();
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

    private async Task Send(string text) => await stream.WriteAsync(Encoding.UTF8.GetBytes(text + '\0'));

    private async Task GetCommand()
    {
        int bytesRead;
        store = MLFoodAnalyzerServer.store;
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
            case "PING":
                await PingServer();
                break;
            case "LOGIN":
                await LogIN();
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
        string message = mLFood.SetImage(filePath);
        // Message send
        await Send(message);
        Stop();
    }

    private async Task ProcessText()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a text");

        int bytesRead;
        store = MLFoodAnalyzerServer.store;
        List<byte> response = new List<byte>();
        // считываем данные до конечного символа
        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }
        string word = Encoding.UTF8.GetString(response.ToArray());
        MLFood mLFood = new();
        // Text
        string message = mLFood.SetText(word);
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

    private async Task LogIN()
    {
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a login");

        Encryption encryption = new();
        int bytesRead;
        List<byte> response = new List<byte>();
        // считываем данные до конечного символа
        while ((bytesRead = stream.ReadByte()) != '\0')
        {
            // добавляем в буфер
            response.Add((byte)bytesRead);
        }
        string word = Encoding.UTF8.GetString(response.ToArray());
        word = encryption.DecryptText(word);
        string[] textSplit = word.Split('|');
        string? message = await database.DBLogIn(encryption.ConvertToHash(textSplit[0]), encryption.ConvertToHash(textSplit[1]));
        message += '\0';
        message = encryption.EncryptText(message);
        await Send(message);
        Stop();
    }


    public string GetInfo() => $"IP: {ip}\nPort: {port}\nTimeout: {timeout}";

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
        return Host.AddressList[Host.AddressList.Length - 1];
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