import fs from 'node:fs';
import { execSync } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(__dirname, '..', '..');
const contentRoot = path.join(__dirname, 'src', 'content');
export const routeLastmod = new Map();

function collect(dir, route = '') {
    for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
        if (entry.isDirectory()) {
            const nextRoute = route === '' && entry.name === 'docs'
                ? route
                : path.posix.join(route, entry.name);

            collect(path.join(dir, entry.name), nextRoute);
        } else if (entry.isFile() && /\.mdx?$/.test(entry.name)) {
            const fullPath = path.join(dir, entry.name);
            const name = entry.name.replace(/\.mdx?$/, '');
            const lookup = name === 'index' ? route : path.posix.join(route, name);
            const urlPath = lookup ? '/' + lookup + '/' : '/';

            const result = execSync(`git log -1 --format='%h|%H|%an|%cI' -- "${fullPath}"`, { cwd: repoRoot }).toString().trim();
            const [shortHash, fullHash, author, isoDate] = result.split('|');
            routeLastmod.set(urlPath, { shortHash, fullHash, author, date: new Date(isoDate), iso: isoDate });
        }
    }
}

collect(contentRoot);
