using System;
using System.Collections.Generic;
using System.IO;

namespace Codice.SyncServerTrigger.Configuration
{
    internal class ToolConfiguration
    {
        ToolConfiguration(ConfigurationFile configFile)
        {
            mConfigFile = configFile;
        }

        internal static ToolConfiguration Load()
        {
            return new ToolConfiguration(LoadConfigFile(ConfigFileName));
        }

        internal ServerConfiguration ServerConfig
        {
            get
            {
                return new ServerConfiguration(
                    mConfigFile.GetSection(SERVER_SECTION_NAME));
            }
        }

        internal void Save()
        {
            mConfigFile.Save();
        }

        static ConfigurationFile LoadConfigFile(string configFile)
        {
            ConfigurationFile result;

            string appPath = Path.GetDirectoryName(Utils.GetAssemblyLocation());
            if ((result = TryLoadOn(appPath, configFile))!= null)
                return result;

            if ((result = TryLoadOn(PlatformUtils.HomePath, configFile)) != null)
                return result;

            return new ConfigurationFile(
                Path.Combine(PlatformUtils.HomePath, configFile));
        }

        static ConfigurationFile TryLoadOn(string path, string configFile)
        {
            string fileName = Path.Combine(path, configFile);

            if (!File.Exists(fileName))
                return null;

            ConfigurationFile result = new ConfigurationFile(fileName);
            result.Load();

            return result;
        }

        ConfigurationFile mConfigFile;

        const string ConfigFileName = "syncservertrigger.conf";
        const string SERVER_SECTION_NAME = "servers";
    }

    internal class ServerConfiguration
    {
        internal ServerConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal List<string> GetServers()
        {
            return mSection.GetStrings(SERVERS_KEY, new string[] { });
        }

        internal void AddServer(string server)
        {
            List<string> servers = GetServers();
            servers.Add(server);
            mSection.SetStringList(SERVERS_KEY, servers);
        }

        internal void RemoveServer(string server)
        {
            List<string> servers = GetServers();
            servers.RemoveAll(item => item.Equals(
                server, StringComparison.InvariantCultureIgnoreCase));
            mSection.SetStringList(SERVERS_KEY, servers);
        }

        ConfigurationSection mSection;
        const string SERVERS_KEY = "servers";
    }
}
