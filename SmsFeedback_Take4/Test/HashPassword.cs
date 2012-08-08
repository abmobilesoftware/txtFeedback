using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
namespace SmsFeedback_Take4.Test
{
    public class HashPassword
    {

        static void Main()
        {
            HashPassword hp = new HashPassword();
            Console.WriteLine(hp.doIt());
        }

        internal string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public string doIt()
        {
            string pass = "mike";
            string salt = GenerateSalt();
            byte[] bIn = System.Text.Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;
            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            
            HashAlgorithm s = HashAlgorithm.Create("SHA1");
            // Hardcoded "SHA1" instead of Membership.HashAlgorithmType
            bRet = s.ComputeHash(bAll);
            return Convert.ToBase64String(bRet);
        }

        private void EncryptPassword()
        {
            throw new NotImplementedException();
        }
       
    }
}