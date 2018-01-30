using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Codice.SyncServerTrigger
{
    internal static class Utils
    {
        internal static string GetAssemblyLocation()
        {
            return Assembly.GetCallingAssembly().Location;
        }

        internal static string ReadStdInToEnd()
        {
            if (!Console.IsInputRedirected)
                return string.Empty;

            using (StreamReader reader = new StreamReader(
                Console.OpenStandardInput(), Console.InputEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        internal static bool CheckServerSpec(string serverSpec)
        {
            return true; // TODO
        }

        internal static string Encrypt(string clearData)
        {
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(
                ms, GetSymmetricAlgorithm().CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(Encoding.Unicode.GetBytes(clearData), 0, clearData.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        internal static string Decrypt(string cipherData)
        {
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(
                ms, GetSymmetricAlgorithm().CreateDecryptor(),
                CryptoStreamMode.Write))
            {
                cs.Write(Convert.FromBase64String(cipherData), 0, cipherData.Length);
                cs.FlushFinalBlock();

                return Encoding.Unicode.GetString(ms.ToArray());
            }
        }

        static SymmetricAlgorithm GetSymmetricAlgorithm()
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(
            "6bf7c2a0-e7db-4464-bffd-6c8751460d6f",
            new byte[] { 0x81, 0x54, 0x72, 0xa9, 0x37, 0x33, 0xb4, 0xa8, 0x30, 0xe1, 0x91, 0x67, 0x4d });

            SymmetricAlgorithm alg = TripleDES.Create();
            alg.Key = pdb.GetBytes(24);
            alg.IV = pdb.GetBytes(8);

            return alg;
        }
    }
}
