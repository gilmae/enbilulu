using System;
using System.IO;
using CommandLine;
using Microsoft.AspNetCore.Hosting;

namespace EnbiluluServer
{
    class Program
    {
        // There are no default options on the Options class, so that command line args can override ENV values only if provided. 
        // If there were default values on the Options class, those defaults would override the ENV values even when not intended to.
        internal static readonly Options DefaultOptions = new Options
        {
            Host = "localhost",
            Port = "367",
            DataFolder = Directory.GetCurrentDirectory()
        };

        static int Main(string[] args)
        {
            var options = ((Parsed<Options>)Parser.Default.ParseArguments<Options>(args)).Value;

            var dataFolder = Utils.Coalesce(options.DataFolder,
                Environment.GetEnvironmentVariable("EnbiluluDataFolder"), DefaultOptions.DataFolder);
            
            Environment.SetEnvironmentVariable("EnbiluluDataFolder", dataFolder);
            
            Console.WriteLine($"Streams Location: {dataFolder}");

            GetWebHost(options).Run();

            return 0;
        }

        public static IWebHost GetWebHost(Options options)
        {
            string hostname = Utils.Coalesce(options.Host, Environment.GetEnvironmentVariable("EnbiluluHost"), DefaultOptions.Host);
            string port = Utils.Coalesce(options.Port, Environment.GetEnvironmentVariable("EnbiluluPort"), DefaultOptions.Port);

            Uri uri = new Uri($"http://{hostname}:{port}");
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(new[] { uri.ToString() })
                .UseStartup<Startup>()
                .Build();

            return host;
        }
    }
}
