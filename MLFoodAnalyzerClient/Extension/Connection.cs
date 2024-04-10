using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Extension;

internal class Connection
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private const string errorServer = "ErrorConToServ";


    public async Task<string?> SendText(string text) => await Send(op: Operation.Text, text) + '\n';

    public async Task<string?> SendPicture(string path)
    {
        string fileName = path;
        FileInfo fileInfo = new(fileName);
        long fileSize = fileInfo.Length;
        if (fileSize >= 8_000_000) return $"{LocalizationResourceManager["LimitImage"]}{fileSize / 1_000_000} MB";

        return await Send(op: Operation.Picture, path) + '\n';
    }

    public async Task<string?> LogIn()
    {
        string? result = await Send(op: Operation.LogIn, string.Empty) ?? string.Empty;
        return (!result.Equals(LocalizationResourceManager[errorServer].ToString())) ? DecryptText(result) : LocalizationResourceManager[errorServer].ToString();
    }

    public async Task<string?> Food() => await Send(op: Operation.LoadFood, string.Empty);

    public async Task<string?> History(int count) => await Send(op: Operation.LoadHistory, count.ToString());

    public async Task<string?> Update(string data) => await Send(op: Operation.UpdateFood, data);

    public async Task<string?> PingServer() => await Send(op: Operation.Ping, string.Empty);

    private async Task<string?> Send(Operation op, string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(AppShell.settings.Ip) || AppShell.settings.Port == 0) throw new Exception();

            using TcpClient tcpClient = new();
            await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);

            NetworkStream stream = tcpClient.GetStream();
            List<byte> response = [];
            int bytesRead = 10;

            _ = op switch
            {
                Operation.Text => Text(stream, query),
                Operation.Picture => Image(stream, tcpClient, query),
                Operation.LogIn => LogIn(stream),
                Operation.LoadFood => LoadFood(stream),
                Operation.LoadHistory => LoadHistory(stream, int.Parse(query)),
                Operation.UpdateFood => UpdateFood(stream, query),
                Operation.Ping => Ping(stream),
                _ => throw new NotImplementedException(),
            };

            while ((bytesRead = stream.ReadByte()) != '\0') response.Add((byte)bytesRead);

            string result = Encoding.UTF8.GetString(response.ToArray());
            response.Clear();
            tcpClient.Close();
            return result;
        }
        catch (Exception)
        {
            return LocalizationResourceManager[errorServer].ToString();
        }
    }

    private async Task Text(NetworkStream stream, string text)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{text}\0"));
    }

    private async Task Image(NetworkStream stream, TcpClient tcpClient, string path)
    {
        CultureInfo cultureInfo = new(AppShell.settings.Language);
        FileInfo fileInfo = new(path);
        long fileSize = fileInfo.Length;
        await stream.WriteAsync(Encoding.UTF8.GetBytes("IMAGE\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes(cultureInfo.TwoLetterISOLanguageName + '\0'));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{fileSize}\0"));

        byte[] buffer = new byte[1024];
        int bytesReadImg;
        FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);

        while ((bytesReadImg = fileStream.Read(buffer, 0, buffer.Length)) != 0) tcpClient.GetStream().Write(buffer, 0, bytesReadImg);
        fileStream.Close();
    }

    private async Task LogIn(NetworkStream stream)
    {
        string encryptText = EncryptText($"{AppShell.settings.Login}|{AppShell.settings.SavedPassword}");
        await stream.WriteAsync(Encoding.UTF8.GetBytes("LOGIN\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{encryptText}\0"));
    }

    private async Task LoadFood(NetworkStream stream) => await stream.WriteAsync(Encoding.UTF8.GetBytes("FOOD\0"));

    private async Task LoadHistory(NetworkStream stream, int count = 0)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes("History\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{count}\0"));
    }

    private async Task UpdateFood(NetworkStream stream, string data)
    {
        string[] rows = data.Split('|');
        await stream.WriteAsync(Encoding.UTF8.GetBytes("Update\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{rows[0]}|{rows[1]}|{rows[2]}\0"));
    }

    private async Task Ping(NetworkStream stream) => await stream.WriteAsync(Encoding.UTF8.GetBytes("PING\0"));

    private string EncryptText(string plainText)
    {
        byte[] toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
        byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(AppShell.settings.Password));

        using TripleDES des = TripleDES.Create();
        des.Key = securityKeyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform objCryptoTransform = des.CreateEncryptor();
        byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
        des.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private string DecryptText(string CipherText)
    {
        byte[] toEncryptArray = Convert.FromBase64String(CipherText);
        byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(AppShell.settings.Password));

        using TripleDES des = TripleDES.Create();
        des.Key = securityKeyArray;
        des.Mode = CipherMode.ECB;
        des.Padding = PaddingMode.PKCS7;

        ICryptoTransform objCryptoTransform = des.CreateDecryptor();
        byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        des.Clear();
        return Encoding.UTF8.GetString(resultArray);
    }

    enum Operation
    {
        Text,
        Picture,
        LogIn,
        LoadFood,
        LoadHistory,
        UpdateFood,
        Ping
    }
}
