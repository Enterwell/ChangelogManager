/**
 * File containing all the logic of our task.
 */

import path = require("path");
import tl = require("azure-pipelines-task-lib/task");
import fs = require("fs");
import { execFileSync } from "child_process";

/**
 * Prints the contents of the repository at the repository location passed to the task.
 * 
 * @param repoLocation Path to the repository.
 */
function printContents(repoLocation: string) {

  // Print the folder contents 
  var folderFiles = fs.readdirSync(repoLocation, {encoding: "utf-8"});

  let changelogName: string = "Changelog.md";

  console.log("=======FOLDER CONTENTS=======");
  folderFiles.forEach((file) => {

    if (file.toLowerCase() === "changelog.md") {
      changelogName = file;
    }

    console.log(file);
  })

  // Print the folder changes contents 
  let changesPath = path.join(repoLocation, "changes");

  try {
    let changesFiles = fs.readdirSync(changesPath, {encoding: "utf-8"});

    console.log("=======CHANGES CONTENTS=======");
    changesFiles.forEach((file) => {
      console.log(file);
    })
  } catch (err) {
    throw new Error("Error occurred while reading changes folder.\n" + err);
  }

  // Print the changelog.md contents
  let changelogPath = path.join(repoLocation, changelogName);

  try {
    var changelogFile = fs.readFileSync(changelogPath, {encoding: "utf-8"});

    console.log("=======CHANGELOG.MD=======");
    console.log(changelogFile);
  } catch (err){
    throw new Error("Error occurred while reading changelog file.\n" + err);
  }
  
}

/**
 * Entry method that runs the task.
 */
async function run() {
  try {

    // Get inputs.
    let input_semanticVersion: string = tl.getInput("semanticVersion", true) || "";
    let input_repoLocation: string = tl.getPathInput("repositoryLocation", true, true) || "";

    console.log("Input semantic version: " + input_semanticVersion);
    console.log("Input repository location: " + input_repoLocation);

    console.log("=============================================BEFORE EXECUTION=============================================");
    printContents(input_repoLocation);

    // Run the executable.
    let executablePath = path.join(__dirname, "cl.exe");

    try {
      let executableOutput = execFileSync(executablePath, [input_semanticVersion, input_repoLocation], {encoding: "utf-8"});
      console.log("=======EXECUTABLE OUTPUT=======");
      console.log(executableOutput);
    } catch (err) {
      throw new Error("Error occurred while running executable.\n" + err);
    }

    console.log("=============================================AFTER EXECUTION=============================================");
    printContents(input_repoLocation);

  } catch (err) {
    tl.setResult(tl.TaskResult.Failed, err.message);
  }
}

run();