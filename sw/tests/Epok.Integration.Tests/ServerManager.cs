using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Epok.Integration.Tests
{
    internal static class ServerManager
    {
        internal const string ServerName = "Epok.Presentation.WebApi.exe";

        private static string _serverDirectoryPath;
        internal static string ServerDirectoryPath
        {
            get
            {
                if (_serverDirectoryPath == null)
                {
                    var root = new DirectoryInfo(Directory.GetCurrentDirectory());
                    while (root?.Name != "sw")
                        root = root?.Parent;
                    _serverDirectoryPath = @$"{root}\src\Epok.Presentation.WebApi";
                }

                return _serverDirectoryPath;
            }
        }

        internal static void EnsureServerIsStarted()
        {
            if (WebApiClient.System.Ping())
                return;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = @$"/C cd {ServerDirectoryPath} & dotnet run",
                }
            };

            Task.Run(() => process.Start());

            const int timeOutSeconds = 60;
            var sw = new Stopwatch();
            sw.Start();

            while (!WebApiClient.System.Ping())
            {
                Thread.Sleep(100);
                if (sw.Elapsed.TotalSeconds >= timeOutSeconds)
                    throw new TimeoutException("Server is not responsive.");
            }
        }

        internal static void EnsureServerIsStopped()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C taskkill /IM \"{ServerName}\" /F",
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
