using System;
using System.Collections.Generic;
using System.IO;

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

        internal RuntimeConfiguration RuntimeConfig
        {
            get
            {
                return new RuntimeConfiguration(
                    mConfigFile.GetSection(RUNTIME_SECTION_NAME));
            }
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

        internal EmailConfiguration EmailConfig
        {
            get
            {
                return new EmailConfiguration(
                    mConfigFile.GetSection(EMAIL_CONFIG_SECTION_NAME));
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
        const string RUNTIME_SECTION_NAME = "runtime";
        const string SERVER_SECTION_NAME = "servers";
        const string REPO_FILTER_SECTION_NAME = "repofilters";
        const string REPO_MAP_SECTION_NAME = "repomappings";
        const string EMAIL_CONFIG_SECTION_NAME = "email";
    }

    internal class RuntimeConfiguration
    {
        internal RuntimeConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal string MonoRuntimePath
        {
            get { return mSection.GetString(RUNTIME_PATH_KEY, string.Empty); }
            set { mSection.SetString(RUNTIME_PATH_KEY, value); }
        }

        ConfigurationSection mSection;
        const string RUNTIME_PATH_KEY = "runtimepath";
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

    internal class EmailConfiguration
    {
        internal EmailConfiguration(ConfigurationSection section)
        {
            mSection = section;
        }

        internal string SmptServer
        {
            get { return mSection.GetString(SMTP_SERVER_KEY, string.Empty); }
            set { mSection.SetString(SMTP_SERVER_KEY, value); }
        }

        internal bool EnableSsl
        {
            get { return mSection.GetBool(ENABLE_SSL_KEY, false); }
            set { mSection.SetBool(ENABLE_SSL_KEY, value); }
        }

        internal int Port
        {
            get { return mSection.GetInt(PORT_KEY, 587); }
            set { mSection.SetInt(PORT_KEY, value); }
        }

        internal string SourceEmail
        {
            get { return mSection.GetString(EMAIL_KEY, string.Empty); }
            set { mSection.SetString(EMAIL_KEY, value); }
        }

        internal string Password
        {
            get
            {
                return Utils.Decrypt(
                    mSection.GetString(PASSWORD_KEY, string.Empty));
            }

            set { mSection.SetString(PASSWORD_KEY, Utils.Encrypt(value)); }
        }

        internal List<string> GetDestinationEmails()
        {
            return mSection.GetStringList(
                DESTINATION_EMAILS_KEY, new string[] { });
        }

        internal void AddDestinationEmail(string email)
        {
            List<string> destinationEmails = GetDestinationEmails();
            destinationEmails.Add(email);
            mSection.SetStringList(DESTINATION_EMAILS_KEY, destinationEmails);
        }

        internal void RemoveDestinationEmail(string email)
        {
            List<string> destinationEmails = GetDestinationEmails();
            destinationEmails.RemoveAll(
                item => item.Equals(
                    email, StringComparison.InvariantCultureIgnoreCase));
            mSection.SetStringList(DESTINATION_EMAILS_KEY, destinationEmails);
        }

        ConfigurationSection mSection;
        const string SMTP_SERVER_KEY = "stmpserver";
        const string ENABLE_SSL_KEY = "enablessl";
        const string PORT_KEY = "port";
        const string EMAIL_KEY = "email";
        const string PASSWORD_KEY = "password";
        const string DESTINATION_EMAILS_KEY = "destinationemails";
    }
}
