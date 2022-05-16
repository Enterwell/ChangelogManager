/**
 * File defining our tests.
 */
import assert = require("assert");
import path = require("path");
import fs = require("fs");
import ttm = require("azure-pipelines-task-lib/mock-test");

/**
 * Path to the temporary test folder structure used in our test suite.
 */
const testRoot = path.join(__dirname, "test_structure");

/**
 * Removes folder on the provided path.
 * 
 * @param currentPath Path to the folder wanting to remove.
 */
const removeFolder = (currentPath: string) => {
    if(fs.existsSync(currentPath)) {
        fs.readdirSync(currentPath).forEach( (file) => {
            const newPath = path.join(currentPath, file);

            if(fs.lstatSync(newPath).isDirectory()) {
                removeFolder(newPath);
            } else {
                fs.unlinkSync(newPath);
            }
        });

        fs.rmdirSync(currentPath);
    }
};

/**
 * Creates directory 'changes' in our test folder and fills it with a few files.
 */
const createChanges = () => {
    
    const changesPath = path.join(testRoot, "changes");
    fs.mkdirSync(changesPath);

    fs.writeFileSync(path.join(changesPath, "Added [FE       ] First Change"), "");
    fs.writeFileSync(path.join(changesPath, "Removed [      API      ] First Deletion"), "");
    fs.writeFileSync(path.join(changesPath, "Removed [API      ] First Deletion2"), "");
    fs.writeFileSync(path.join(changesPath, "Removed [API] First Deletion3"), "");
}

/**
 * Creates a changelog file in our test folder and fills it with some initial data.
 */
const createChangelogFile = () => {

    const changelogContent = 
    `# Changelog
All notable changes to this project will be documented in this file.
    
The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2018-08-13
### Changed
- Migrated from .NET Framework 4.5 to .NET Standard 2.0`;

    fs.writeFileSync(path.join(testRoot, "Changelog.md"), changelogContent);
}

/**
 * Describes a Test Suite (root of our tests).
 */
describe("Merge Changelog Test Suite", function() {
    this.timeout(60000);

    /**
     * Runs before each individual test in our suite.
     */
    beforeEach( () => {
        fs.mkdirSync(testRoot);
    });

    /**
     * Runs after each individual test in our suite.
     */
    afterEach( () => {
        removeFolder(testRoot);
    })

    /**
     * Wraps all our assert calls in a try/catch and correctly handles them.
     * 
     * @param validator A function containing asserts.
     * @param tr MockTestRunner that runs the tests. Used to get output.
     * @param done Mocha.Done that signals when the test has finished.
     */
    function runValidations(validator: () => void, tr: ttm.MockTestRunner, done: Mocha.Done) {
        try {
            validator();
            done();
        }
        catch (error) {
            console.log("STDERR", tr.stderr);
            console.log("STDOUT", tr.stdout);
            done(error);
        }
    }

    /**
     * Describes tests with valid inputs.
     */
    describe("valid inputs", function() {

        /**
         * Testing the task when valid inputs are passed but the changelog file does not exist.
         */
        it("should fail if changelog.md does not exist", (done: Mocha.Done) => {
            this.timeout(5000);
    
            // Arrange
            createChanges();
    
            // Act 
            let tp: string = path.join(__dirname, "L0Valid.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);
    
            tr.run();
    
            // Assert
            runValidations(() => {
             assert(tr.failed, "should have failed");
             assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
             assert.strictEqual(tr.errorIssues.length, 1, "should have 1 error issue");
             assert(tr.errorIssues[0].startsWith("Error occurred while reading changelog file"), "should have correct message");
            }, 
            tr, done);
        });

        /**
         * Testing the task when valid inputs are passed but the changes directory does not exist.
         */
        it("should fail if changes directory does not exist", (done: Mocha.Done) => {
            this.timeout(5000);
    
            // Arrange
            createChangelogFile();
    
            // Act 
            let tp: string = path.join(__dirname, "L0Valid.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);
    
            tr.run();
    
            // Assert
            runValidations(() => {
             assert(tr.failed, "should have failed");
             assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
             assert.strictEqual(tr.errorIssues.length, 1, "should have 1 error issue");
             assert(tr.errorIssues[0].startsWith("Error occurred while reading changes folder"), "should have correct message");
            }, 
            tr, done);
        });

        /**
         * Testing the task when valid inputs are passed and everything exists.
         */
        it("should succeed when everything is at its place", (done: Mocha.Done) => {
            this.timeout(5000);

            // Arrange
            createChanges();
            createChangelogFile();

            // Act
            let tp: string = path.join(__dirname, "L0Valid.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);

            tr.run();

            // Assert
            runValidations(() => {
                assert(tr.succeeded, "should have succeeded");
                assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
                assert.strictEqual(tr.errorIssues.length, 0, "should have 0 error issue");
               }, 
               tr, done);
        });
    })

    /**
     * Describes tests with invalid inputs.
     */
    describe("invalid inputs", function() {

        /**
         * Testing the task when invalid changelog location is passed in.
         */
        it("should fail if input changelog location does not exist", (done: Mocha.Done) => {
            this.timeout(1000);
    
            // Arrange
            createChanges();
            createChangelogFile();
    
            // Act 
            let tp: string = path.join(__dirname, "L0InvalidChangelogLocation.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);
    
            tr.run();
    
            // Assert
            runValidations(() => {
             assert(tr.failed, "should have failed");
             assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
             assert.strictEqual(tr.errorIssues.length, 1, "should have 1 error issue");
             assert(tr.errorIssues[0].startsWith("Not found changelogLocation"), "should have correct message");
            }, 
            tr, done);
        });

        /**
         * Testing the task when invalid changes location is passed in.
         */
         it("should fail if input changes location does not exist", (done: Mocha.Done) => {
            this.timeout(1000);
    
            // Arrange
            createChanges();
            createChangelogFile();
    
            // Act 
            let tp: string = path.join(__dirname, "L0InvalidChangesLocation.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);
    
            tr.run();
    
            // Assert
            runValidations(() => {
             assert(tr.failed, "should have failed");
             assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
             assert.strictEqual(tr.errorIssues.length, 1, "should have 1 error issue");
             assert(tr.errorIssues[0].startsWith("Not found changesLocation"), "should have correct message");
            }, 
            tr, done);
        });

        /**
         * Testing the task when changes location is an empty string.
         */
         it("should fail if input changes location is an empty string", (done: Mocha.Done) => {
            this.timeout(1000);
    
            // Arrange
            createChanges();
            createChangelogFile();
    
            // Act 
            let tp: string = path.join(__dirname, "L0EmptyChangesLocation.js");
            let tr: ttm.MockTestRunner = new ttm.MockTestRunner(tp);
    
            tr.run();
    
            // Assert
            runValidations(() => {
             assert(tr.failed, "should have failed");
             assert.strictEqual(tr.warningIssues.length, 0, "should have no warnings");
             assert.strictEqual(tr.errorIssues.length, 1, "should have 1 error issue");
             assert(tr.errorIssues[0].startsWith("Input required"), "should have correct message");
            }, 
            tr, done);
        });
    })
});