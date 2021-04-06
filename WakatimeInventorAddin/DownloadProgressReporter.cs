using System.ComponentModel;
using System.Net;
using WakaTime;

namespace WakatimeInventorAddIn
{
    public class DownloadProgressReporter : IDownloadProgressReporter
    {
        public void Report(DownloadProgressChangedEventArgs value)
        {
        }

        public void Dispose()
        {
        }

        public void Show(string message = "")
        {
        }

        public void Close(AsyncCompletedEventArgs e)
        {
        }
    }
}