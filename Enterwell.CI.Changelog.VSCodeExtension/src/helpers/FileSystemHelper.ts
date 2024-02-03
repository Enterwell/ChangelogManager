import fs from 'fs';
import path from 'path';
import * as vscode from 'vscode';

const changesDirectoryName: string = 'changes';

/**
 * Determines the user's workspace folder in which to generate a change file.
 *
 * @async
 * @returns The workspace folder object.
 */
export async function getWorkspaceFolder() {
  const workspaceFolders = vscode.workspace.workspaceFolders;
  if (!workspaceFolders) {
    vscode.window.showErrorMessage('You have no workspaces open.');
    return undefined;
  }

  let workspaceFolder: vscode.WorkspaceFolder | undefined = undefined;

  // If the user has multiple workspace folders open
  // ask them for which one to generate a change file
  if (workspaceFolders.length > 1) {
    workspaceFolder = await vscode.window.showWorkspaceFolderPick({
      placeHolder: 'Choose a workspace for which to create a change file'
    });
  } else {
    workspaceFolder = vscode.workspace.workspaceFolders?.[0];
  }

  return workspaceFolder;
}

/**
 * Constructs the change file name.
 *
 * @param type Change type
 * @param category Change category
 * @param description Change description
 * @returns Name of the change file to be saved
 */
export function constructFileName(type: string, category: string, description: string) {
  const trimmedDescription = description.trim();
  const trimmedCategory = category.trim();

  let fileName;
  if (!trimmedCategory) {
    fileName = `${type} ${trimmedDescription}`;
  } else {
    fileName = `${type} [${trimmedCategory}] ${trimmedDescription}`;
  }

  // Replace multiple spaces with a single space
  return fileName.replace(/\s+/g, ' ');
}

/**
 * Ensures that the changes directory exists. If it does not exist, the directory is created.
 *
 * @param workspaceFolderPath User's workspace folder path
 */
export function ensureChangesDirectoryExists(workspaceFolderPath: string) {
  const changesDirectoryPath = path.join(workspaceFolderPath, changesDirectoryName);

  if (!fs.existsSync(changesDirectoryPath)) {
    fs.mkdirSync(changesDirectoryPath);
  }
}

/**
 * Creates the change file.
 *
 * @param workspaceFolderPath User's workspace folder path
 * @param fileName Name of the change file
 */
export function createFile(workspaceFolderPath: string, fileName: string) {
  const changesDirectoryPath = path.join(workspaceFolderPath, changesDirectoryName);

  fs.writeFileSync(path.join(changesDirectoryPath, fileName), '');
}