using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace GPS.Components.Cryptography
{
    public static class Encryption
    {
        private static string gpsoft = "www.gpsoft.pl";

        public static string Encrypt(string input, byte[] key, CipherMode cipherMode, PaddingMode paddingMode, bool whiteSpacesBack)
        {
            if (whiteSpacesBack && input == "")
                return "";

            key = HashMD5Password(gpsoft);

            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = cipherMode;
            tripleDES.Padding = paddingMode;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string input, byte[] key, CipherMode cipherMode, PaddingMode paddingMode, bool whiteSpacesBack)
        {
            if (whiteSpacesBack && input == "")
                return "";

            key = HashMD5Password(gpsoft);

            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = cipherMode;
            tripleDES.Padding = paddingMode;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static byte[] HashSHA1Password(string password)
        {
            var provider = new SHA1CryptoServiceProvider();
            var encoding = new UnicodeEncoding();
            return provider.ComputeHash(encoding.GetBytes(password));
        }

        public static byte[] HashMD5Password(string password)
        {
            var provider = new MD5CryptoServiceProvider();
            var encoding = new UnicodeEncoding();

            return provider.ComputeHash(encoding.GetBytes(password));
        }
    }
}
