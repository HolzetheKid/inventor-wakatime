using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            WebClient client = new WebClient();

            client.DownloadFileCompleted += OnDownloadCompletetd;

            client.DownloadFileAsync(new Uri("https://github.com/wakatime/wakatime/archive/master.zip"), "C:\\Users\\holze\\AppData\\Local\\Temp\\wakatime-cli.zip");

            do
            {
                Task.Delay(500);
            } while (true);
        }

        private void OnDownloadCompletetd(object sender, AsyncCompletedEventArgs e)
        {
            Debug.WriteLine("check file");
        }
    }
}
