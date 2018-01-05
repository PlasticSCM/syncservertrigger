using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Codice.SyncServerTrigger.Configuration
{
    public class ConfigurationFile
    {
        public ConfigurationFile(string name)
        {
            mFileName = name;
            mSections = new Dictionary<string, ConfigurationSection>();
        }

        public void Load()
        {
            using (StreamReader reader = new StreamReader(mFileName))
            {
                string line;

                ConfigurationSection section = GetSection("");

                while ((line = reader.ReadLine()) != null)
                {
                    ProcessLine(ref section, line);
                }
            }
        }

        public string GetEntryValue(string sectionName, string key)
        {
            ConfigurationSection section = GetSection(sectionName);

            string result = section.GetValue(key) as string;

            return (result == null) ? string.Empty : result;
        }

        public void SetEntryValue(string sectionName, string key, string val)
        {
            ConfigurationSection section = GetSection(sectionName);
            section.SetValue(key, val);
        }

        public ConfigurationSection GetSection(string sectionName)
        {
            ConfigurationSection result;

            if (mSections.TryGetValue(sectionName, out result))
                return result;

            result = new ConfigurationSection(sectionName);
            mSections.Add(sectionName, result);

            return result;
        }

        void ProcessLine(ref ConfigurationSection section, string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                switch (line[0])
                {
                    case '[': ProcessSection(ref section, line); break;
                    case ';':
                    case '#':
                        break; // comment, will be overwritten, but ignore here
                    default: section.ParseEntry(line); break;
                }
            }
        }

        public void Save()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(mFileName, false))
                {
                    // If the first section to be read is the "default" one (""),
                    // it must be the first one to be saved too!!
                    string[] sortedSections = new string[mSections.Keys.Count];
                    mSections.Keys.CopyTo(sortedSections, 0);
                    Array.Sort<string>(sortedSections);

                    foreach (string section in sortedSections)
                    {
                        mSections[section].Write(writer);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to store: " + ex.Message);
            }
        }

        void ProcessSection(ref ConfigurationSection section, string line)
        {
            line = line.TrimStart('[', ' ');
            line = line.TrimEnd(']', ' ');

            section = GetSection(line);
        }

        string mFileName;
        Dictionary<string, ConfigurationSection> mSections;
    }

    public class ConfigurationSection
    {
        public string SectionName
        {
            get { return mSectionName; }
        }

        public ConfigurationSection(string sectionName)
        {
            mSectionName = sectionName;
        }

        public object GetValue(string name)
        {
            return mEntries[name];
        }

        public void SetValue(string name, object value)
        {
            mEntries[name] = value;
        }

        public int GetInt(string name, int defaultValue)
        {
            string res = mEntries[name] as string;

            int result;
            if (int.TryParse(res, out result))
                return result;

            return defaultValue;
        }

        public void SetInt(string name, int val)
        {
            mEntries[name] = val.ToString();
        }

        public double GetDouble(string name, float defaultValue)
        {
            string res = mEntries[name] as string;

            if (res != null)
            {
                return double.Parse(res);
            }

            return defaultValue;
        }

        public double GetDouble(string name, double defaultValue)
        {
            string res = mEntries[name] as string;

            if (res != null)
            {
                return double.Parse(res);
            }

            return defaultValue;
        }

        public float GetFloat(string name, float defaultValue)
        {
            string res = mEntries[name] as string;

            float result;
            if (float.TryParse(res, out result))
                return result;

            return defaultValue;
        }

        public void SetDouble(string name, double val)
        {
            mEntries[name] = val.ToString();
        }

        public void SetFloat(string name, float val)
        {
            mEntries[name] = val.ToString();
        }

        public long GetLong(string name, long defaultValue)
        {
            string res = mEntries[name] as string;

            long result;
            if (long.TryParse(res, out result))
                return result;

            return defaultValue;
        }

        public void SetLong(string name, long val)
        {
            mEntries[name] = val.ToString();
        }

        public string GetString(string name, string defaultValue)
        {
            return mEntries[name] != null
                ? mEntries[name] as string
                : defaultValue;
        }

        public string GetString(string name)
        {
            return mEntries[name] as string;
        }

        public void SetString(string name, string val)
        {
            mEntries[name] = val;
        }

        public string GetBase64String(string name)
        {
            if (string.IsNullOrEmpty(mEntries[name] as string))
                return null;

            return Encoding.UTF8.GetString(
                Convert.FromBase64String(mEntries[name] as string));
        }

        public void SetBase64String(string name, string val)
        {
            if (val == null)
                return;

            mEntries[name] = Convert.ToBase64String(Encoding.UTF8.GetBytes(val));
        }

        public bool GetBool(string name, bool defaultValue)
        {
            string res = mEntries[name] as string;

            if (res != null)
            {
                res = res.ToLower();

                return (res == "true" || res == "1");
            }

            return defaultValue;
        }

        public void SetBool(string name, bool val)
        {
            mEntries[name] = val ? "true" : "false";
        }

        public DateTime GetDateTime(string name, long defaultValue)
        {
            long ticks = GetLong(name, defaultValue);

            if (ticks == -1)
                return DateTime.MaxValue;

            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public void SetDateTime(string name, DateTime date)
        {
            long ticks = date.ToUniversalTime().Ticks;

            SetLong(name, ticks);
        }

        public List<string> GetStringList(string name, string[] defaultValue)
        {
            string res = mEntries[name] as string;

            if (string.IsNullOrEmpty(res))
                return new List<string>(defaultValue);

            List<string> result = DeserializeList<string>(res, str => str);

            return (result == null) ? new List<string>(defaultValue) : result;
        }

        public void SetStringList(string name, List<string> values)
        {
            string key = name;

            if (values == null)
                mEntries[key] = values;

            mEntries[key] = SerializeWithCommas(values);
        }

        internal void ParseEntry(string entryLine)
        {
            if (string.IsNullOrEmpty(entryLine))
                return;

            int split = entryLine.IndexOf('=');

            if (split < 0)
                return;

            string key = entryLine.Substring(0, split);

            string val = "";

            if (split < entryLine.Length - 1)
            {
                val = entryLine.Substring(split + 1);
            }

            mEntries[key] = val;
        }

        internal void Write(TextWriter writer)
        {
            if (!string.IsNullOrEmpty(mSectionName))
            {
                writer.WriteLine("[{0}]", mSectionName);
            }

            foreach (DictionaryEntry entry in mEntries)
            {
                writer.WriteLine("{0}={1}", (string)entry.Key, entry.Value);
            }

            writer.WriteLine();
        }

        public static string SerializeWithCommas(IEnumerable args)
        {
            bool isFirst = true;

            StringBuilder sb = new StringBuilder();

            foreach (object obj in args)
            {
                if (isFirst)
                {
                    sb.Append(obj.ToString());
                    isFirst = false;
                    continue;
                }

                sb.Append("," + obj.ToString());
            }

            return sb.ToString();
        }

        public static List<T> DeserializeList<T>(
            string target, Func<string, T> converter)
        {
            string[] strings = target.Split(',');

            if (strings == null || strings.Length == 0)
                return null;

            List<T> result = new List<T>();

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    result.Add(converter(strings[i]));
                }
                catch
                {
                    throw new Exception(string.Format(
                        "Failed to parse T value '{0}' in string '{1}'",
                        result[i], target));
                }
            }

            return result;
        }

        SortedList mEntries = new SortedList(StringComparer.InvariantCultureIgnoreCase);
        string mSectionName;
    }
}
