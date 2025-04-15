using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using Enterwell.CI.Changelog.Shared;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace Enterwell.CI.Changelog.VSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddChangeCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = PackageIds.AddChangeCommandId;

        /// <summary>
        /// Top-level object in the Visual Studio automation object model.
        /// In this class used to get access to the current Solution's path.
        /// </summary>
        private readonly DTE2 dte;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = PackageGuids.guidVSIXPackageCmdSet;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChangeCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file) and gets current Solution's path.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="dte">Visual Studio automation object model, not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
        private AddChangeCommand(AsyncPackage package, OleMenuCommandService commandService, DTE2 dte)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            this.dte = dte ?? throw new ArgumentNullException(nameof(dte));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddChangeCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => this.package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AddChangeCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte2 = await package.GetServiceAsync(typeof(DTE)) as DTE2;

            Instance = new AddChangeCommand(package, commandService, dte2);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFolder = new DirectoryInfo(Path.Combine(dte.Solution.FullName)).Parent;
            var solutionPath = solutionFolder?.FullName;

            string fileName = ShowDialogForAddingChange(solutionPath);
            if (fileName == string.Empty)
            {
                await StatusBarLogAsync(false, "Cancelled by the user");
                return;
            }

            var changesDirectoryPath = FileSystemHelper.FindNearestChangesFolder(solutionPath);
            (bool isSuccessful, string reason) = FileSystemHelper.CreateFile(Path.Combine(changesDirectoryPath, fileName));

            await StatusBarLogAsync(isSuccessful, reason);
        }

        /// <summary>
        /// Uses the environment's status bar to notify the user if the change file was successfully created and added. 
        /// </summary>
        /// <param name="changeCreated"><see cref="bool"/> that is used to determine if the file was created or not.</param>
        /// <param name="reason"><see cref="string"/> that specifies the reason if the file was not created.</param>
        /// <returns>An awaitable.</returns>
        private async Task StatusBarLogAsync(bool changeCreated, string reason)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsStatusbar statusBar = (IVsStatusbar) await ServiceProvider.GetServiceAsync(typeof(SVsStatusbar));

            if (statusBar != null)
            {
                // Making sure the status bar is not frozen.
                statusBar.IsFrozen(out int frozen);

                // If the status bar is frozen, defrost it.
                if (frozen != 0)
                {
                    statusBar.FreezeOutput(0);
                }

                var statusBarText =
                    changeCreated ? "Change Added Successfully" : $"Adding Change Failed. Reason: {reason}";

                statusBar.SetText(statusBarText);

                // Freeze the status bar.
                statusBar.FreezeOutput(1);
            }
        }

        /// <summary>
        /// Displays the dialog asking the user to specify what change he would like to add.
        /// </summary>
        /// <param name="solutionPath">Path to the solution.</param>
        /// <returns>Fully named change ready to be created as a file if the user accepted the dialog or <see cref="string.Empty"/> if the user refused.</returns>
        private string ShowDialogForAddingChange(string solutionPath)
        {
            var dialog = new AddChangeDialog(solutionPath);
            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                return FileSystemHelper.ConstructFileName(dialog.ChangeType, dialog.ChangeCategory, dialog.ChangeDescription);
            }

            return string.Empty;
        }
    }
}