import fs from 'node:fs';
import path from 'node:path';
import { execFileSync } from 'node:child_process';

const repoRoot = path.resolve(process.cwd(), '..', '..');
const contentRoot = path.resolve(process.cwd(), 'src', 'content');

interface Commit {
    shortHash: string;
    fullHash: string;
    author: string;
    date: Date;
}

const timeTable = new Map<string, Commit>();

export function getLatestCommit(route: string): Commit | null {
    route = URL.canParse(route) ? new URL(route).pathname : route;

    const commit = timeTable.get(route);

    if (!commit) {
        console.log(`[update-time] No commit found for route: ${route}`);
        return null;
    }

    return commit;
}

function collect(directory: string) {
    for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
        if (entry.isDirectory()) {
            collect(path.join(directory, entry.name));
        } else if (entry.isFile() && /\.mdx?$/.test(entry.name)) {
            const fullPath = path.join(directory, entry.name);
            const relativePath = path.relative(contentRoot, fullPath);

            let posix = relativePath.replace(/\.mdx?$/, '').split(path.sep).join('/');

            if (posix.startsWith('docs/'))
                posix = posix.slice(5);

            if (posix === 'index')
                posix = '';
            else if (posix.endsWith('/index'))
                posix = posix.slice(0, -('/index'.length));

            const urlPath = posix ? '/' + posix + '/' : '/';
            const result = execFileSync('git', ['log', '-1', '--format=%h|%H|%an|%cI', '--', fullPath], { cwd: repoRoot }).toString().trim();
            const [shortHash, fullHash, author, isoDate] = result.split('|');

            timeTable.set(urlPath, { shortHash, fullHash, author, date: new Date(isoDate) });
            console.log(`[update-time] Commit for ${urlPath}: ${isoDate}`);
        }
    }
}

collect(contentRoot)
