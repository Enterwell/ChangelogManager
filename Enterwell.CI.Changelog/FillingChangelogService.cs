using System;
using System.Threading.Tasks;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service charged with filling the repository changelog with our changes from the 'changes' folder. 
    /// </summary>
    public class FillingChangelogService
    {
        private readonly ParseInputService parseInputService;
        private readonly ChangeGatheringService changeGatheringService;
        private readonly MarkdownTextService markdownTextService;
        private readonly FileWriterService fileWriterService;

        /// <summary>
        /// Initializes a new instance of the <c>FillingChangelogService</c> class.
        /// </summary>
        /// <param name="parseInputService">Service that knows how to parse input arguments.</param>
        /// <param name="changeGatheringService">Service that knows how to gather changes from the changes location.</param>
        /// <param name="markdownTextService">Service that knows how to format changes into text for writing.</param>
        /// <param name="fileWriterService">Service that knows how to write changelog section into a file.</param>
        public FillingChangelogService(
            ParseInputService parseInputService,
            ChangeGatheringService changeGatheringService,
            MarkdownTextService markdownTextService,
            FileWriterService fileWriterService)
        {
            this.parseInputService = parseInputService;
            this.changeGatheringService = changeGatheringService;
            this.markdownTextService = markdownTextService;
            this.fileWriterService = fileWriterService;
        }

        /// <summary>
        /// Main method of the application. The method delegates all the work to the appropriate services.
        /// </summary>
        /// <param name="inputArguments">Input arguments passed in by the user.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task Run(string[] inputArguments)
        {
            var inputs = this.parseInputService.ParseInputs(inputArguments);

            var versionInformation = await this.changeGatheringService.GatherVersionInformation(inputs.ChangelogLocation, inputs.ChangesLocation);

            var newChangelogSection = this.markdownTextService.BuildChangelogSection(versionInformation);
            Console.WriteLine(newChangelogSection);

            var elementToInsertChangelogSectionBefore = this.markdownTextService.ToH2(string.Empty);

            await this.fileWriterService.WriteToChangelog(newChangelogSection, inputs.ChangelogLocation, elementToInsertChangelogSectionBefore);

            this.changeGatheringService.EmptyChangesFolder(inputs.ChangesLocation);
        }
    }
}