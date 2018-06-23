using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Company
{
    class Action
    {
        public static string AccountName{get;set;}
        public static bool Acces { get; set; } = true;
        public static string Encrypt(string value)
        {
            byte[] data = new UTF8Encoding().GetBytes(value);
            SHA256 d = new SHA256Managed();
            return BitConverter.ToString(d.ComputeHash(data)).Replace("-", string.Empty);
        }
    }
}
