#! /usr/bin/env node

const fs = require("fs");
const path = require("path");
const { spawn } = require("child_process");

let binaryName;

// Generate the binary name based on the process platform
if (process.platform === 'win32') {
  binaryName = 'cc-win.exe';
} else if (process.platform === 'darwin') {
  binaryName = 'cc-osx';
} else if (process.platform === 'linux') {
  binaryName = 'cc-linux';
} else {
  console.error('Unsupported platform:', process.platform);
  process.exit(1);
}

const binaryPath = path.resolve(__dirname, 'dist', binaryName);

if (process.platform !== 'win32') {
  fs.chmodSync(binaryPath, 0o777);
}

// Ignoring 'node' and the name of the script
const argsToForward = process.argv.slice(2);

const childProcess = spawn(binaryPath, argsToForward, { stdio: 'inherit', shell: process.platform === 'win32' });

childProcess.on("exit", (code) => {
  process.exit(code);
});
