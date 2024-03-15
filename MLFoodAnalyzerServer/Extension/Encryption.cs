﻿using System.Security.Cryptography;
using System.Text;

namespace MLFoodAnalyzerServer.Extension
{
    public class Encryption
    {
        private const string SecurityKey = "QWERTY";

        public Encryption() { }

        public string EncryptText(string PlainText)
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

        public string DecryptText(string CipherText)
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

        public string ConvertToHash(string input) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));

        public string GetPassword() => SecurityKey;
    }
}
