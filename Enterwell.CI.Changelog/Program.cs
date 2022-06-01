using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enterwell.CI.Changelog
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<FillingChangelogService>();

                    services.AddSingleton<ParseInputService>();
                    services.AddSingleton<ChangeGatheringService>();
                    services.AddSingleton<MarkdownTextService>();
                    services.AddSingleton<FileWriterService>();
                })
                .Build();

            var changelogService = ActivatorUtilities.CreateInstance<FillingChangelogService>(host.Services);

            try
            {
                await changelogService.Run(args);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}