using System.Text;
using System.Security.Cryptography;

namespace MLFoodAnalyzerServer.Extension
{
    public class Encryption(string SecurityKey = "QWERTY")
    {
        private string SecurityKey = SecurityKey;

        public string EncryptText(string plainText)
        {
            byte[] toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(SecurityKey!));

            using TripleDES des = TripleDES.Create();
            des.Key = securityKeyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform objCryptoTransform = des.CreateEncryptor();
            byte[] resultArray = objCryptoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            des.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string DecryptText(string CipherText)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText);
            byte[] securityKeyArray = MD5.HashData(Encoding.UTF8.GetBytes(SecurityKey!));

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

        public string Password
        {
            get => SecurityKey;
            set => SecurityKey = value;
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