using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
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
        public const int CommandId = 0x0100;

        public DTE2 dte;

        private readonly string solutionPath;
        private const string ChangesFolderName = "changes";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e2a9d4e5-33a0-4e98-ad0a-cda0dbae0af4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChangeCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AddChangeCommand(AsyncPackage package, OleMenuCommandService commandService, DTE2 dte)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            this.dte = dte;
            var solutionFolder = new DirectoryInfo(Path.Combine(dte.Solution.FullName)).Parent;
            this.solutionPath = solutionFolder?.FullName;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
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
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AddChangeCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
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
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var result = ShowDialogForAddingChange();

            if (result == string.Empty) return;
            
            EnsureChangesDirectoryExist();

            CreateFile(Path.Combine(this.solutionPath, ChangesFolderName, result));
        }

        private string ShowDialogForAddingChange()
        {
            var dialog = new AddChangeDialog(this.solutionPath);
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                if (string.IsNullOrWhiteSpace(dialog.ChangeCategory))
                    return $"{dialog.ChangeType} {dialog.ChangeDescription}";

                return $"{dialog.ChangeType} [{dialog.ChangeCategory}] {dialog.ChangeDescription}";
            }
            else
            {
                return string.Empty;
            }
        }

        private void EnsureChangesDirectoryExist()
        {
            var changesDirectoryPath = Path.Combine(solutionPath, ChangesFolderName);

            if (!Directory.Exists(changesDirectoryPath))
            {
                Directory.CreateDirectory(changesDirectoryPath);
            }
        }

        private void CreateFile(string filePath)
        {
            var file = File.Create(filePath);
            file.Close();
        }
    }
}
