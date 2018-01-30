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

        internal static string ReadLine(string promptLine)
        {
            string prompt = string.Format("{0}: ", promptLine);
            Console.Write(prompt);

            string answer = Console.ReadLine();
            while (string.IsNullOrEmpty(answer))
            {
                Console.Write(prompt);
                answer = Console.ReadLine();
            }

            return answer;
        }

        internal static string ReadLine(string promptLine, string defaultValue)
        {
            string prompt = string.Format(
                "{0} [{1}]: ", promptLine, defaultValue);

            Console.Write(prompt);

            string answer = Console.ReadLine();
            return string.IsNullOrEmpty(answer) ? defaultValue : answer;
        }

        internal static string ReadPassword(string promptLine)
        {
            Console.Write("{0}: ", promptLine);
            Stack<string> passbits = new Stack<string>();
            for (ConsoleKeyInfo cki = Console.ReadKey(true); cki.Key != ConsoleKey.Enter; cki = Console.ReadKey(true))
            {
                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (passbits.Count > 0)
                    {
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        passbits.Pop();
                    }
                }
                else
                {
                    Console.Write("*");
                    passbits.Push(cki.KeyChar.ToString());
                }
            }
            Console.Write(Environment.NewLine);

            string[] pass = passbits.ToArray();
            Array.Reverse(pass);
            return string.Join(string.Empty, pass);
        }

        internal static int ReadInt(string promptLine)
        {
            string prompt = string.Format("{0}: ", promptLine);
            Console.Write(prompt);

            int answer = 0;
            while (!int.TryParse(Console.ReadLine(), out answer))
            {
                Console.Write(prompt);
            }

            return answer;
        }

        internal static bool ReadBool(string promptLine)
        {
            string prompt = string.Format("{0} [y/n]: ", promptLine);
            Console.Write(prompt);

            string answer = Console.ReadLine();
            bool? result;

            while (!(result = ParseYesOrNoAnswer(answer)).HasValue)
            {
                Console.Write(prompt);
                answer = Console.ReadLine();
            }

            return result.Value;
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

        static bool? ParseYesOrNoAnswer(string answer)
        {
            answer = answer.ToLowerInvariant();
            if (answer == "y" || answer == "yes")
                return true;

            if (answer == "n" || answer == "no")
                return false;

            return null;
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
