using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerClient.Extension;

internal class Connection
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
    private const string errorServer = "ErrorConToServ";







    public async Task<string?> SendText(string text)
    {
        if (string.IsNullOrWhiteSpace(AppShell.settings.Ip)) return LocalizationResourceManager[errorServer].ToString();

        using TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(AppShell.settings.Ip, AppShell.settings.Port);
        var stream = tcpClient.GetStream();

        var response = new List<byte>();
        NetworkStream networkStream = tcpClient.GetStream();

        int bytesRead = 10; //  To read bytes from a stream
        await stream.WriteAsync(Encoding.UTF8.GetBytes("TEXT\0"));
        await stream.WriteAsync(Encoding.UTF8.GetBytes($"{text}\0"));

        while ((bytesRead = stream.ReadByte()) != '\0')
            response.Add((byte)bytesRead);

        string translation = Encoding.UTF8.GetString(response.ToArray());

        response.Clear();
        networkStream.Close();
        return translation + '\n';
    }

    public static string EncryptText(string plainText)
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

    public static string DecryptText(string CipherText)
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
}
