using System;
using System.Security.Cryptography;
using System.Text;

namespace TestSolution.Api.Helpers
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// GetSHA256
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetSHA256(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
