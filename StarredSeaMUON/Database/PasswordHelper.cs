using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    internal class PasswordHelper
    {
        /// <summary>
        /// hashes a new password using a randomly generated salt, returns the salt prepended to the password hash
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string hashPassword(string pass)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            byte[] hash = doHash(pass, salt);
            byte[] outputBytes = new byte[32 + 20]; //32 byte salt + 20 byte hash
            Array.Copy(salt, 0, outputBytes, 0, 32);
            Array.Copy(hash, 0, outputBytes, 32, 20);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// run the actual hashing algorithm, returning the 20 byte hash
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] doHash(string pass, byte[] salt)
        {
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(pass, salt, 10000, HashAlgorithmName.SHA512);
            return rdb.GetBytes(20);
        }
    }
}
