using System;
using System.Security.Cryptography;
using System.Text;

namespace master.framework.security
{
    public static class Manager
    {
        public static string HashSHA1(string text)
        {
            return HashString(text, "sha1");
        }
        public static string HashString(string text, string hashName)
        {
            string ret = string.Empty;
            HashAlgorithm algorithm = HashAlgorithm.Create(hashName);//new SHA1Cng();
            if (algorithm == null)
            {
                throw new ArgumentException("Unrecognized hash name", "hashName");
            }
            using (algorithm)
            {
                ret = BinaryToHex(algorithm.ComputeHash(Encoding.UTF8.GetBytes(text)));
            }
            //byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
            return ret;
        }

        private static string BinaryToHex(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            char[] array = new char[checked(data.Length * 2)];
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                array[2 * i] = NibbleToHex((byte)(b >> 4));
                array[2 * i + 1] = NibbleToHex((byte)(b & 15));
            }
            return new string(array);
        }
        private static char NibbleToHex(byte nibble)
        {
            return (char)((nibble < 10) ? (nibble + 48) : (nibble - 10 + 65));
        }

    }
}
