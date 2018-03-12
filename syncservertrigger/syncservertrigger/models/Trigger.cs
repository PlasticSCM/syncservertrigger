using System;
using System.Collections.Generic;

namespace Codice.SyncServerTrigger.Models
{
    internal class Trigger
    {
        internal static class Names
        {
            internal const string AfterCi = "sync-afterci";
            internal const string AfterRW = "sync-afterreplication";
            internal const string AfterMkLb = "sync-aftermklabel";
            internal const string AfterChAttVal = "sync-afterchatt";
        }

        internal static class Types
        {
            internal const string AfterCi = "after-checkin";
            internal const string AfterRW = "after-replicationwrite";
            internal const string AfterMkLb = "after-mklabel";
            internal const string AfterChAttVal = "after-chattvalue";
        }

        // From 'cm listtriggers --help'
        // {0} -> Position
        // {1} -> Name
        // {2} -> Path
        // {3} -> Owner
        // {4} -> Type
        // {5} -> Filter
        internal const string CM_FORMAT = "{0}#{1}#{2}#{3}#{4}#{5}";

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

        internal static List<Trigger> ParseCmListTriggersOutput(string cmOutput)
        {
            if (string.IsNullOrEmpty(cmOutput))
                return new List<Trigger>();

            List<Trigger> result = new List<Trigger>();

            Array.ForEach(
                cmOutput.Split(NEWLINE, StringSplitOptions.RemoveEmptyEntries),
                line => result.Add(ParseLine(line)));

            return result;
        }

        static Trigger ParseLine(string cmLine)
        {
            string[] fields = cmLine.Split(FIELD_SEPARATOR);
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

        static readonly string[] NEWLINE = { Environment.NewLine };
        static readonly char[] FIELD_SEPARATOR = { '#' };
    }
}
