using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
    private const string SecurityKey = "QWERTY";
    private Store store;
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

        Console.WriteLine("\nHit enter to continue...");
    }

    private async Task Send(string text)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes(text + '\0'));
    }

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

        if (word[0] == '1')
        {
            word = DecryptText(word[1..]);
        }
        else
        {
            word = word[1..];
        }

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
        char code = word[0];
        if (code == '1')
        {
            word = DecryptText(word[1..]);
        }

        MLFood mLFood = new();
        Console.WriteLine($"[{DateTime.Now}] Client {tcpClient.Client.RemoteEndPoint} requested a text");
        // Text

        string message = mLFood.SetText(word);
        message += '\0';

        if (code == '1')
        {
            message = EncryptText('1' + message);
        }
        else
        {
            message = '0' + message;
        }
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

    private static string DecryptText(string CipherText)
    {
        byte[] toEncryptArray = Convert.FromBase64String(CipherText);

        MD5CryptoServiceProvider objMD5CryptoService = new();
        //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            //Assigning the Security key to the TripleDES Service Provider.
            Key = securityKeyArray,
            //Mode of the Crypto service is Electronic Code Book.
            Mode = CipherMode.ECB,
            //Padding Mode is PKCS7 if there is any extra byte is added.
            Padding = PaddingMode.PKCS7
        };

        var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
        //Transform the bytes array to resultArray
        byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        objTripleDESCryptoService.Clear();

        //Convert and return the decrypted data/byte into string format.
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    private static string EncryptText(string PlainText)
    {
        // Getting the bytes of Input String.
        byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

        MD5CryptoServiceProvider objMD5CryptoService = new();
        //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
        byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
        //De-allocatinng the memory after doing the Job.
        objMD5CryptoService.Clear();

        using TripleDESCryptoServiceProvider objTripleDESCryptoService = new()
        {
            //Assigning the Security key to the TripleDES Service Provider.
            Key = securityKeyArray,
            //Mode of the Crypto service is Electronic Code Book.
            Mode = CipherMode.ECB,
            //Padding Mode is PKCS7 if there is any extra byte is added.
            Padding = PaddingMode.PKCS7
        };


        var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
        //Transform the bytes array to resultArray
        byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
        objTripleDESCryptoService.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
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