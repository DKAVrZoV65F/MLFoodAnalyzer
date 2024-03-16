using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerServer.Extension
{
    public class Encryption
    {
        private const string SecurityKey = "QWERTY";

        public Encryption() { }

        public static string EncryptText(string plainText)
        {
            byte[] toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(SecurityKey));

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
            byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(SecurityKey));

            using TripleDES des = TripleDES.Create();
            des.Key = securityKeyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform objCryptoTransform = des.CreateDecryptor();
            byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            des.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }

        public static string ConvertToHash(string input) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));

        public static string GetPassword() => SecurityKey;
    }
}
