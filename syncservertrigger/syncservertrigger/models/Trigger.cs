using System;
using System.Collections.Generic;

namespace Codice.SyncServerTrigger.Models
{
    internal class Trigger
    {
        // From 'cm listtriggers --help'
        internal const string CmFormat = "{0}#{1}#{2}#{3}#{4}#{5}";

        internal static class Names
        {
            internal const string AfterCi = "after-checkin";
            internal const string AfterRW = "after-replicationwrite";
            internal const string AfterMkLb = "after-mklabel";
            internal const string AfterChAttVal = "after-chattvalue";
        }

        internal int Position { get; private set; }
        internal string Name { get; private set; }
        internal string Path { get; private set; }
        internal string Owner { get; private set; }
        internal string Type { get; private set; }
        internal string Filter { get; private set; }

        internal Trigger(
            int position,
            string name,
            string path,
            string owner,
            string type,
            string filter)
        {
            Position = position;
            Name = name;
            Path = path;
            Owner = owner;
            Type = type;
            Filter = filter;
        }

        internal static List<Trigger> ParseFind(string cmOutput)
        {
            if (string.IsNullOrEmpty(cmOutput))
                return new List<Trigger>();

            List<Trigger> result = new List<Trigger>();

            foreach (string line in cmOutput.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries))
            {
                result.Add(ParseLine(line));
            }

            return result;
        }

        static Trigger ParseLine(string cmLine)
        {
            string[] fields = cmLine.Split(new char[] { '#' });
            if (fields.Length != 6)
                return null;

            return new Trigger(
                position: int.Parse(fields[0]),
                name: fields[1],
                path: fields[2],
                owner: fields[3],
                type: fields[4],
                filter: fields[5]);
        }
    }
}
