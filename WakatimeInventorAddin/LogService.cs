using System;
using System.Diagnostics;
using NLog;
using WakaTime;

namespace WakatimeInventorAddIn
{
    public class LogService : ILogService
    {
        private ILogger logger;
        public LogService(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void Log(string message)
        {
            Debug.WriteLine(message);
            logger.Debug(message);
        }
        
    }
}