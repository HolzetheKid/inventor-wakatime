using System;
using System.IO;
using System.Reflection;
using Inventor;
using NLog;
using NLog.Config;
using Path = System.IO.Path;

namespace WakatimeInventorAddIn
{
    internal class MyLogManager
    {
        public static LogFactory Instance { get { return _instance.Value; } }
        private static Lazy<LogFactory> _instance = new Lazy<LogFactory>(BuildLogFactory);

     
        private static LogFactory BuildLogFactory()
        {
            var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogManager.Configuration = new XmlLoggingConfiguration($"{binPath}/nlog.config");

            // Use name of current assembly to construct NLog config filename 
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string configFilePath = $"{binPath}/nlog.config";

            LogFactory logFactory = new LogFactory();
            logFactory.Configuration = new XmlLoggingConfiguration(configFilePath, logFactory);
            return logFactory;
        }
    }
}