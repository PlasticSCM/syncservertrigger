using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal void Save()
        {

        }

        static ConfigurationFile LoadConfigFile(string configFile)
        {
            ConfigurationFile result;

            string appPath = Path.GetDirectoryName(Utils.GetAssemblyLocation());
            if ((result = TryLoadOn(appPath, configFile))!= null)
                return result;

            string defaultPath = Utils.GetHomeBasedPath(configFile);
            if ((result = TryLoadOn(defaultPath, configFile)) != null)
                return result;

            return new ConfigurationFile(Path.Combine(defaultPath, configFile));
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
    }
}
