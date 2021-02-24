/**
 * File that mocks task runner and sets up invalid repository location input.
 */

import tmrm = require("azure-pipelines-task-lib/mock-run");
import path = require("path");

let taskPath = path.join(__dirname, "..", "index.js");
let tmr: tmrm.TaskMockRunner = new tmrm.TaskMockRunner(taskPath);

tmr.setInput("semanticVersion", "1.2.0");
tmr.setInput("repositoryLocation", "something that's invalid");

tmr.run(true);