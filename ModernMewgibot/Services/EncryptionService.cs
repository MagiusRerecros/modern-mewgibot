using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModernMewgibot.Services
{
    static class EncryptionService
    {
        static byte[] entropy = Encoding.Unicode.GetBytes("MewgiBot Salting String - Very Unoriginal");

        /// <summary>
        /// Creates an encrypted string from a SecureString, using
        /// a salt value.
        /// </summary>
        /// <param name="input">SecureString to encrypt</param>
        /// <returns>Encrypted string</returns>
        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts an encrypted string with a specific salt value
        /// into a SecureString.
        /// </summary>
        /// <param name="encryptedData">Encrypted string to decrypt</param>
        /// <returns>SecureString containing decrypted text</returns>
        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        /// <summary>
        /// Converts a string to a SecureString
        /// </summary>
        /// <param name="input">string to convert</param>
        /// <returns>Read-Only SecureString</returns>
        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        /// <summary>
        /// Converts a SecureString to a string
        /// </summary>
        /// <param name="input">SecureString to convert</param>
        /// <returns>string containing text from SecureString</returns>
        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);

            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }

            return returnValue;
        }
    }
}