import fs from 'fs';
import path from 'path';

/**
 * Class representing the changelog configuration.
 *
 * @class Configuration
 */
export default class Configuration {
  /**
   * Changelog configuration name.
   *
   * @memberof Configuration
   */
  static name: string = '.changelog.json';

  /**
   * Loads the changelog configuration from the directory root.
   *
   * @param workspaceFolderPath User's workspace folder path
   */
  static loadConfiguration(workspaceFolderPath: string): { categories: string[] } | undefined | null {
    // Notify that the incorrect directory was passed in
    if (!fs.existsSync(workspaceFolderPath)) {
      return null;
    }

    let currentDirectory = workspaceFolderPath;

    // Search for the changelog config upwards
    while (path.basename(currentDirectory) !== '') {
      const possibleConfigFilePath = path.join(currentDirectory, this.name);

      // If the file exists, parse it
      if (fs.existsSync(possibleConfigFilePath)) {
        const configFile = fs.readFileSync(possibleConfigFilePath, 'utf-8');

        return JSON.parse(configFile);
      }

      currentDirectory = path.dirname(currentDirectory);
    }

    return undefined;
  }
}