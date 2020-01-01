using System;
using System.IO;
using CommandLine;
using Microsoft.AspNetCore.Hosting;

namespace Enbilulu
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = ((Parsed<Options>)Parser.Default.ParseArguments<Options>(args)).Value;

            Environment.SetEnvironmentVariable("DataFolder", options.DataFolder);
            GetWebHost(options).Run();

            return 0;
        }

        public static IWebHost GetWebHost(Options options)
        {
            
                Uri uri = new Uri($"http://localhost:{options.Port}");
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(new[] { uri.ToString() })
                    .UseStartup<Startup>()
                    .Build();

                return host;
            }
        }
}
