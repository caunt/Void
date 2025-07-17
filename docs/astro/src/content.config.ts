import { defineCollection } from 'astro:content';
import { docsLoader } from '@astrojs/starlight/loaders';
import { docsSchema } from '@astrojs/starlight/schema';

export const collections = {
    docs: defineCollection({
        loader: docsLoader(),
        schema: docsSchema()
    }),
    releases: defineCollection({
        loader: async () => {
            const headers: Record<string, string> = {};
            const token = process.env.GITHUB_TOKEN;
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }

            const response = await fetch(
                "https://api.github.com/repos/caunt/Void/releases",
                { headers }
            );
            const data = await response.json();

            if (!Array.isArray(data)) {
                throw new Error("Unexpected response from GitHub Releases API");
            }

            data.forEach((item: any) => {
                item.id = item.id.toString();
            });

            return data;
        }
    })
};
