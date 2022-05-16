/**
 * File that mocks task runner and sets up invalid changelog location input.
 */

import tmrm = require("azure-pipelines-task-lib/mock-run");
import path = require("path");

let taskPath = path.join(__dirname, "..", "index.js");
let tmr: tmrm.TaskMockRunner = new tmrm.TaskMockRunner(taskPath);

tmr.setInput("changelogLocation", "something that's invalid");
tmr.setInput("changesInDifferentLocation", "false");
tmr.setInput("changesLocation", "something");

tmr.run(true);