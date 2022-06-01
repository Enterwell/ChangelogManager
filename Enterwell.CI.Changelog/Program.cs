using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enterwell.CI.Changelog
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await Host
                .CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<ChangeGatheringService>();
                    services.AddSingleton<MarkdownTextService>();
                    services.AddSingleton<FileWriterService>();
                })
                .RunCommandLineApplicationAsync<ChangelogManagerCommand>(args);
        }
    }
}