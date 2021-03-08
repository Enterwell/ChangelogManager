using System;
using System.Collections.Generic;
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
        /// Initializes FillingChangelogService object.
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
        /// Main method of the application. User interface calls it and the method delegates all the work to the appropriate services.
        /// </summary>
        /// <param name="inputArguments">Input arguments passed by the user to the application.</param>
        public async Task Run(string[] inputArguments)
        {
            (string semanticVersion, string changelogLocation, string changesLocation) = this.parseInputService.ParseInputs(inputArguments);

            Dictionary<string, List<string>> changes = await this.changeGatheringService.GatherChanges(changelogLocation, changesLocation);

            var newChangelogSection = this.markdownTextService.BuildChangelogSection(semanticVersion, changes);
            Console.WriteLine(newChangelogSection);

            var elementToInsertChangelogSectionBefore = this.markdownTextService.ToH2(string.Empty);

            await this.fileWriterService.WriteToChangelog(newChangelogSection, changelogLocation,
                elementToInsertChangelogSectionBefore);
            
            this.changeGatheringService.EmptyChangesFolder(changesLocation);
        }
    }
}