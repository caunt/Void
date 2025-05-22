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
            const response = await fetch("https://api.github.com/repos/caunt/Void/releases");
            const data = await response.json();

            data.forEach((item : any) => {
                item.id = item.id.toString();
            });

            return data;
        }
    })
};
