// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';

// https://astro.build/config
export default defineConfig({
    site: 'https://void.caunt.world',
    integrations: [starlight({
        title: 'Void Docs',
        social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/caunt/void' }],
        sidebar: [
            {
                label: 'Getting Started',
                items: [
                    { label: 'Configuration', slug: 'getting-started/configuration' },
                    { label: 'Running', slug: 'getting-started/running' },
                ],
            },
            {
                label: 'Developing Plugins',
                items: [
                    {
                        label: 'Events',
                        items: [
                            { label: 'Overview', slug: 'developing-plugins/events/overview' }
                        ]
                    },
                    {
                        label: 'Services',
                        items: [
                            { label: 'Overview', slug: 'developing-plugins/services/overview' },
                            { label: 'Singleton Services', slug: 'developing-plugins/services/singleton' },
                            { label: 'Scoped Services', slug: 'developing-plugins/services/scoped' },
                            { label: 'Transient Services', slug: 'developing-plugins/services/transient' }
                        ]
                    }
                ],
            },
            {
                label: 'Reference',
                autogenerate: { directory: 'reference' },
            },
        ],
    }), sitemap()],
});