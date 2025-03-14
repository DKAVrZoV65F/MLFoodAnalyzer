﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Extension;

public class TCPServer
{
    private IPAddress ip;
    private int port;
    private int timeout;
    private TcpClient? tcpClient;
    private NetworkStream stream = null!;
    private TcpListener tcpListener = null!;
    private static DateTime startUserOperation;
    private readonly string success = "Settings applied successfully";
    private readonly string unsuccess = "Settings applied unsuccessfully tcp";

    private readonly Database database = Server.database ??= new();
    private readonly Encryption encryption = Server.encryption ??= new();
    private readonly ImageStore store = Server.store ??= new();

    public TCPServer(int? port = null, int? timeout = null)
    {
        ip = Ip;
        this.port = port ?? 55555;
        this.timeout = timeout ?? 10000;
    }

    public IPAddress Ip
    {
        get
        {
            if (ip is null)
            {
                string Hostname = Environment.MachineName;
                IPHostEntry Host = Dns.GetHostEntry(Hostname);
                ip = Host.AddressList[^1];
            }
            return ip;
        }
        set
        {
            Console.WriteLine(IPAddress.TryParse(value.ToString(), out ip!) ? success : unsuccess);
        }
    }


    protected void MyHandler(object sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("\nServer close.");
        args.Cancel = true;

        stream?.Close();
        tcpClient?.Close();
        tcpListener?.Stop();
    }

    public async Task Run()
    {
        tcpListener = new TcpListener(ip, port);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler!);
        try
        {
            tcpClient = new()
            {
                ReceiveTimeout = timeout,
                SendTimeout = timeout
            };

            tcpListener.Start();
            Console.WriteLine("The server is running. Waiting for connections... ");
            while (true)
            {
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                startUserOperation = DateTime.Now;
                Console.WriteLine($"[{startUserOperation}] Client {tcpClient.Client.RemoteEndPoint} connected to server");
                stream = tcpClient.GetStream();
                stream.ReadTimeout = timeout;
                stream.WriteTimeout = timeout;
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
    }


    private async Task GetCommand()
    {
        string query = await GetMessage();
        string? result = string.Empty;
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient?.Client.RemoteEndPoint} requested a/an {query}");
        switch (query)
        {
            case "IMAGE":
                string language = await GetMessage();
                result = await GetMessage();
                result = await GetImage(result, store.PathFolder);
                result = await ProcessImage(language, result);
                break;
            case "TEXT":
                result = await GetMessage();
                result = await ProcessText(result);
                break;
            case "PING":
                result = await PingServer();
                break;
            case "LOGIN":
                result = await GetMessage();
                result = await LogIn(result);
                break;
            case "FOOD":
                result = await GetAllFood();
                break;
            case "History":
                result = await GetMessage();
                result = await GetAllHistory(result);
                break;
            case "Update":
                result = await GetMessage();
                result = await UpdateFood(result);
                break;
            default:
                break;
        }
        await Send(result);
        await Stop();
    }



    private async Task<string> GetMessage()
    {
        int bytesRead;
        List<byte> response = [];
        await Task.Delay(0);
        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);
        return Encoding.UTF8.GetString(response.ToArray());
    }

    private async Task<string> GetImage(string imageSize, string? folderPath)
    {
        int bytesRead;
        string[] files = Directory.GetFiles(folderPath!);
        int numberOfFiles = files.Length;
        long size = long.Parse(imageSize);
        long sum = 0;
        byte[] buffer = new byte[1024];
        string fileName = $"{store.NameFile}_{numberOfFiles}.{store.Format}";
        string fileFullPath = @$"{folderPath}\{fileName}";
        FileStream? fileStream;
        using (fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write))
        {
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                fileStream.Write(buffer, 0, bytesRead);
                sum += bytesRead;
            } while (size != sum);
        }
        await Task.Delay(0);
        return fileFullPath;
    }

    private async Task Send(string? text) => await stream.WriteAsync(Encoding.UTF8.GetBytes(text + '\0'));

    private async Task Stop()
    {
        DateTime end = DateTime.Now;
        TimeSpan difference = end.Subtract(startUserOperation);
        Console.WriteLine($"[{end}] Client {tcpClient?.Client.RemoteEndPoint} disconnect and waste {difference} milliseconds");
        await Task.Delay(0);
        stream.Close();
    }

    private async Task<string> PingServer()
    {
        await Task.Delay(0);
        return "SUCCESS";
    }

    private async Task<string?> ProcessImage(string language, string fileFullPath)
    {
        MLFood mLFood = new()
        {
            Image = fileFullPath
        };
        Dictionary<float, string> message = mLFood.PredictImage(language);
        return await database.ExecuteQuery(message);
    }

    private async Task<string?> ProcessText(string message)
    {
        MLFood mLFood = new()
        {
            Text = message
        };
        Dictionary<float, string> result = mLFood.PredictText();
        return await database.ExecuteQuery(result);
    }

    private async Task<string> LogIn(string message)
    {
        message = encryption.DecryptText(message);
        string[] textSplit = message.Split('|');
        string? response = await database.ExecuteQuery("LogIn", Encryption.ConvertToHash(textSplit[0]), Encryption.ConvertToHash(textSplit[1]));
        return encryption.EncryptText(response!);
    }

    private async Task<string?> GetAllFood() => await database.ExecuteQuery("AllFood");

    private async Task<string?> GetAllHistory(string message) => await database.ExecuteQuery("History", message);

    private async Task<string?> UpdateFood(string message)
    {
        string[] textSplit = message.Split('|');
        return await database.ExecuteQuery("Update", textSplit);
    }

    public override string ToString() => $"IP: {Ip}\nPort: {port}\nTimeout: {timeout} ms";

    public int Port
    {
        get => port;
        set
        {
            _ = int.TryParse($"{value:D}", out int outputParse);
            if (outputParse >= 49152 && outputParse <= 65535)
            {
                port = outputParse;
                Console.WriteLine(success);
                return;
            }
            Console.WriteLine(unsuccess);
        }
    }

    public int Timeout
    {
        get => timeout;
        set
        {
            _ = int.TryParse($"{value:D}", out int outputParse);
            if (outputParse >= 0 && outputParse <= 10_000_000)
            {
                timeout = outputParse;
                Console.WriteLine(success);
                return;
            }
            Console.WriteLine(unsuccess);
        }
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