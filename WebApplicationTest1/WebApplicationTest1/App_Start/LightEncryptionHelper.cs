using System;

using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace WebApplicationTest1.App_Start
{
    public class LightEncryptionHelper
    {
        // Simply random numbers used as common keys. 
        // Note that if these are changed, all previously encrypted values can no longer be decrypted.
        private static byte[] iv = { 2, 0, 24, 84, 12, 102, 214, 199 };
        private static byte[] key = { 12, 235, 234, 231, 12, 234, 76, 12 };

        // Encrypts a string
        public static string Encrypt(string data)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);

            using (MemoryStream mStream = new MemoryStream((data.Length * 2) - 1))
            {
                using (CryptoStream encStream = new CryptoStream(mStream, GetAlgorithm().CreateEncryptor(), CryptoStreamMode.Write))
                {
                    encStream.Write(plainBytes, 0, plainBytes.Length);
                    encStream.FlushFinalBlock();

                    byte[] encryptedBytes = new byte[mStream.Length];

                    mStream.Position = 0;
                    mStream.Read(encryptedBytes, 0, encryptedBytes.Length);

                    return System.Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string DecryptUrlEncoded(string data, bool generateWarning = true)
        {
            string result = String.Empty;
            try
            {
                result = Decrypt(data);
            }
            catch // The encrypted member id might be url encoded
            {
                try
                {
                    // Decode but check for spaces, that are + signs and valid characters in Base 64 strings
                    string tmpEncryptedMemberId = System.Web.HttpUtility.UrlDecode(data).Replace(" ", "+");

                    // Equals sign at end may be chopped off
                    if (!tmpEncryptedMemberId.EndsWith("="))
                    {
                        tmpEncryptedMemberId += "=";
                    }

                    // Finally, check if it's a valid Base 64 string
                    if (IsBase64CharacterString(tmpEncryptedMemberId))
                    {
                        result = Decrypt(tmpEncryptedMemberId);
                        // May have lost a preceding forward slash during url rewriting
                    }
                    else if (IsBase64CharacterString("/" + tmpEncryptedMemberId))
                    {
                        result = Decrypt("/" + tmpEncryptedMemberId);
                    }
                }
                catch (Exception ex)
                {
                    if (generateWarning)
                    {
                        //LogHelper.Log(LogHelper.Level.Error, "ErrorHandler - DecryptUrlEncoded problem", ex);
                    }
                }
            }
            return result;
        }

        // Decrypts a string
        public static string Decrypt(string data)
        {
            byte[] encryptedBytes = System.Convert.FromBase64String(data);

            using (MemoryStream mStream = new MemoryStream((data.Length * 2) - 1))
            {
                using (CryptoStream decStream = new CryptoStream(mStream, GetAlgorithm().CreateDecryptor(), CryptoStreamMode.Write))
                {
                    decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    decStream.FlushFinalBlock();

                    byte[] plainBytes = new byte[mStream.Length];

                    mStream.Position = 0;
                    mStream.Read(plainBytes, 0, plainBytes.Length);

                    return Encoding.UTF8.GetString(plainBytes);
                }
            }

        }

        // We use a simple 56 bit algorithm to keep the result value short.
        private static SymmetricAlgorithm GetAlgorithm()
        {
            SymmetricAlgorithm result = new DESCryptoServiceProvider();

            result.Key = key;
            result.IV = iv;

            return result;
        }

        // Check if the string is a valid Base 64 character string
        public static bool IsBase64CharacterString(string testString)
        {
            // If the test string's length is not a multiple of 4
            if (testString.Length % 4 != 0)
            {
                return false;
            }

            try
            {
                // Try to convert it, this will throw an exception if it is invalid
                if (System.Convert.FromBase64String(testString) == null)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}