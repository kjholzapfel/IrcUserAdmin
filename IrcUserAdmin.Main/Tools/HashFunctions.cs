using System;
using System.Security.Cryptography;
using System.Text;

namespace IrcUserAdmin.Tools
{
    public static class HashFunctions
    {
        public static string ComputeHash(string input)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes;
            using (var algorithm = new SHA256Managed())
            {
                hashedBytes = algorithm.ComputeHash(inputBytes);
            }
            var sb = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("x2"));
            }
            string encrypted = sb.ToString();
            return encrypted;
        }

        public static string ComputeSaltedHash(string input, string salt)
        {
            string saltedinput = string.Concat(input, salt);
            string hash = ComputeHash(saltedinput);
            return hash;
        }
    }
}