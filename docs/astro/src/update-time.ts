import fs from 'node:fs/promises';
import path from 'node:path';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';

const execFileAsync = promisify(execFile);

const repositoryRootDirectory = path.resolve(process.cwd(), '..', '..');
const contentRootDirectory = path.resolve(process.cwd(), 'src', 'content');

interface Commit {
    shortHash: string;
    fullHash: string;
    author: string;
    date: Date;
}

const commitTimeTable = new Map<string, Commit>();

export async function getLatestCommit(routeUrl: string): Promise<Commit | null> {
    var parsedRoutePath = URL.canParse(routeUrl) ? new URL(routeUrl).pathname : routeUrl;
    var commitData = commitTimeTable.get(parsedRoutePath);

    if (commitData == null) {
        console.log(`[update-time] No commit found for route: ${parsedRoutePath}`);
        return null;
    }

    return commitData;
}

async function collectCommitData(targetDirectory: string): Promise<void> {
    var { stdout } = await execFileAsync('git', ['log', '--name-only', '--format=COMMIT|%h|%H|%an|%cI', '--', targetDirectory], { cwd: repositoryRootDirectory, maxBuffer: 1024 * 1024 * 25 });

    var currentCommitBlock: Commit | null = null;
    var logOutputLines = stdout.split('\n');

    for (var currentLine of logOutputLines) {
        var trimmedLine = currentLine.trim();

        if (trimmedLine === '')
            continue;

        if (trimmedLine.startsWith('COMMIT|')) {
            var commitParts = trimmedLine.split('|');
            currentCommitBlock = {
                shortHash: commitParts[1],
                fullHash: commitParts[2],
                author: commitParts[3],
                date: new Date(commitParts[4])
            };
        } else if (currentCommitBlock != null) {
            var absoluteFilePath = path.resolve(repositoryRootDirectory, trimmedLine);

            if (commitTimeTable.has(absoluteFilePath))
                continue;

            commitTimeTable.set(absoluteFilePath, currentCommitBlock);
        }
    }

    async function traverseAndMap(currentDirectory: string): Promise<void> {
        var directoryEntries = await fs.readdir(currentDirectory, { withFileTypes: true });

        var processingPromises = directoryEntries.map(async function (fileEntry) {
            var fullFilePath = path.join(currentDirectory, fileEntry.name);

            if (fileEntry.isDirectory()) {
                await traverseAndMap(fullFilePath);
            } else if (fileEntry.isFile() && /\.mdx?$/.test(fileEntry.name)) {
                // Absolute to relative path
                var relativeFilePath = path.relative(contentRootDirectory, fullFilePath);

                // Remove .md or .mdx extension
                var normalizedPosixPath = relativeFilePath.replace(/\.mdx?$/, '');

                // Replace path separators with '/'
                normalizedPosixPath = normalizedPosixPath.split(path.sep).join('/');

                // Remove Starlight 'docs/' prefix
                normalizedPosixPath = normalizedPosixPath.replace(/^docs\//, '');

                // Remove 'index' suffix from paths
                normalizedPosixPath = normalizedPosixPath.replace(/(^|\/)index$/, '');

                // Make the URL path start and end with a '/'
                var formattedUrlPath = normalizedPosixPath ? `/${normalizedPosixPath}/` : '/';

                var fileCommitData = commitTimeTable.get(fullFilePath);

                if (!fileCommitData)
                    return;

                commitTimeTable.set(formattedUrlPath, fileCommitData);
                console.log(`[update-time] Commit for ${formattedUrlPath}: ${fileCommitData.date.toISOString()}`);
            }
        });

        await Promise.all(processingPromises);
    }

    await traverseAndMap(targetDirectory);
}

await collectCommitData(contentRootDirectory);
