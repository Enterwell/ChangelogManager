/**
 * File that mocks task runner and sets up empty changes location input.
 */

import tmrm = require('azure-pipelines-task-lib/mock-run');
import path = require('path');

let taskPath = path.join(__dirname, '..', 'index.js');
let tmr: tmrm.TaskMockRunner = new tmrm.TaskMockRunner(taskPath);

const testRoot: string = path.join(__dirname, 'test_structure');

tmr.setInput('changelogLocation', testRoot);
tmr.setInput('changesInDifferentLocation', 'true');
tmr.setInput('changesLocation', '');

tmr.run(true);