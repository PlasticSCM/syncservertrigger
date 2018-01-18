using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Codice.SyncServerTrigger.Models;

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

        internal RepoFilterConfiguration RepoFilterConfig
        {
            get
            {
                return new RepoFilterConfiguration(
                    mConfigFile.GetSection(REPO_FILTER_SECTION_NAME));
            }
        }

        internal RepoMapConfiguration RepoMapConfig
        {
            get
            {
                return new RepoMapConfiguration(
                    mConfigFile.GetSection(REPO_MAP_SECTION_NAME));
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
        const string REPO_FILTER_SECTION_NAME = "repofilters";
        const string REPO_MAP_SECTION_NAME = "repomappings";
    }

    internal class ServerConfiguration
    {
        internal ServerConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal List<string> GetServers()
        {
            return mSection.GetStringList(SERVERS_KEY, new string[] { });
        }

        internal void AddServer(string server)
        {
            List<string> servers = GetServers();
            servers.Add(server);
            mSection.SetStringList(SERVERS_KEY, servers);
        }

        internal void DeleteServer(string server)
        {
            List<string> servers = GetServers();
            servers.RemoveAll(item => item.Equals(
                server, StringComparison.InvariantCultureIgnoreCase));
            mSection.SetStringList(SERVERS_KEY, servers);
        }

        ConfigurationSection mSection;
        const string SERVERS_KEY = "servers";
    }

    internal class RepoFilterConfiguration
    {
        internal RepoFilterConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal List<string> GetFilteredRepos()
        {
            return mSection.GetStringList(FILTERS_KEY, new string[] { });
        }

        internal void AddFilteredRepo(string repo)
        {
            List<string> filteredRepos = GetFilteredRepos();
            filteredRepos.Add(repo);
            mSection.SetStringList(FILTERS_KEY, filteredRepos);
        }

        internal void DeleteFilteredRepo(string repo)
        {
            List<string> filteredRepos = GetFilteredRepos();
            filteredRepos.RemoveAll(item => item.Equals(
                repo, StringComparison.InvariantCultureIgnoreCase));
            mSection.SetStringList(FILTERS_KEY, filteredRepos);
        }

        ConfigurationSection mSection;
        const string FILTERS_KEY = "repofilters";
    }

    internal class RepoMapConfiguration
    {
        internal RepoMapConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal List<RepoMapping> GetMappedRepos()
        {
            return RepoMapping.ParseFromConfiguration(
                mSection.GetStringList(MAPS_KEY, new string[] { }));
        }

        internal void AddMappedRepo(RepoMapping mapping)
        {
            List<string> mappedRepos =
                mSection.GetStringList(MAPS_KEY, new string[] { });
            mappedRepos.Add(mapping.ToConfigurationString());
            mSection.SetStringList(MAPS_KEY, mappedRepos);
        }

        internal void DeleteMappedRepo(RepoMapping mapping)
        {
            string mapToRemove = mapping.ToConfigurationString();

            List<string> mappedRepos =
                mSection.GetStringList(MAPS_KEY, new string[] { });
            mappedRepos.RemoveAll(item =>
                item.Equals(mapToRemove, StringComparison.InvariantCultureIgnoreCase));
            mSection.SetStringList(MAPS_KEY, mappedRepos);
        }

        ConfigurationSection mSection;
        const string MAPS_KEY = "repomaps";

        const string FORMAT = "{0}#{1}";
        static readonly char[] SEPARATOR = new char[] { '#' };
    }
}
