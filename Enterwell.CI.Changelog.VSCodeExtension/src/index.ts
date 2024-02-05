import * as vscode from 'vscode';

import { allowedTypes } from './constants';
import {
  constructFileName,
  createFile,
  ensureChangesDirectoryExists,
  getWorkspaceFolder,
  loadConfiguration
} from './helpers/FileSystemHelper';

/**
 * Handles the command termination with a user error message.
 */
const terminate = () => {
  vscode.window.showErrorMessage('Change creation aborted');
};

/**
 * Handles the changelog create command asynchronously.
 */
const handleCommand = async () => {
  // First, ask the user for the change type
  const changeType = await vscode.window.showQuickPick(allowedTypes, {
    title: 'Change Type',
    placeHolder: 'Choose a change type to be created'
  });

  // If the user aborted the change type input
  if (changeType === undefined) {
    return terminate();
  }

  // Second, ask the user for the change category
  const workspaceFolder = await getWorkspaceFolder();
  if (!workspaceFolder) {
    return terminate();
  }

  const configuration = loadConfiguration(workspaceFolder.uri.fsPath);
  if (configuration === null) {
    vscode.window.showErrorMessage('Failed to load the changelog configuration.');
    return terminate();
  }

  let categoryText: string | undefined = '';

  // If the configuration was not found or the categories list is empty
  // give the option to the user to enter any text for the category
  if (configuration === undefined || (!configuration.categories || !configuration.categories.length)) {
    categoryText = await vscode.window.showInputBox({
      title: 'Change Category',
      placeHolder: '(Optional) Enter the change category'
    });
  } else {
    categoryText = await vscode.window.showQuickPick(configuration.categories, {
      title: 'Change Category',
      placeHolder: 'Choose a category for the change to be created'
    });
  }

  // If the user aborted the change category input
  if (categoryText === undefined) {
    return terminate();
  }

  // Third, ask the user for the change description
  const descriptionText = await vscode.window.showInputBox({
    title: 'Change Description',
    placeHolder: 'Enter the change description',
    validateInput: (value) => !value.trim() ? 'Change description is required!' : undefined
  });

  // If the user aborted the change description input
  if (descriptionText === undefined) {
    return terminate();
  }

  // Construct the change file name
  const fileName = constructFileName(changeType, categoryText, descriptionText);
  if (!fileName) {
    return terminate();
  }

  ensureChangesDirectoryExists(workspaceFolder.uri.fsPath);

  try {
    createFile(workspaceFolder.uri.fsPath, fileName);
    return vscode.window.showInformationMessage('Change added successfully!');
  } catch (err) {
    return terminate();
  }
};

/**
 * Method called when the extension is activated.
 * The extension is activated the first time command is executed.
 */
export function activate(context: vscode.ExtensionContext) {
  // The command name that has been defined in the package.json file (extension manifest)
  const commandName = 'changelog-change-create.addchange';

  const commandSubscription = vscode.commands.registerCommand(commandName, handleCommand);
  context.subscriptions.push(commandSubscription);
}

/**
 * Method called when the extension is deactivated.
 */
export function deactivate() {}
