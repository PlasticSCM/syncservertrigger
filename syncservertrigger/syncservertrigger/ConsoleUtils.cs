using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codice.SyncServerTrigger
{
    internal static class ConsoleUtils
    {
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

        static bool? ParseYesOrNoAnswer(string answer)
        {
            answer = answer.ToLowerInvariant();
            if (answer == "y" || answer == "yes")
                return true;

            if (answer == "n" || answer == "no")
                return false;

            return null;
        }
    }
}
