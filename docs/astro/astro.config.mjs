// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';

const googleAnalyticsId = 'G-3KT5D46L8T'

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
                    { slug: 'getting-started/running' },
                    { slug: 'getting-started/configuration' }
                ],
            },
            {
                label: 'Developing Plugins',
                items: [
                    { slug: 'developing-plugins/development-kit' },
                    {
                        label: 'Events',
                        items: [
                            { slug: 'developing-plugins/events/overview' }
                        ]
                    },
                    {
                        label: 'Services',
                        items: [
                            { slug: 'developing-plugins/services/overview' },
                            { slug: 'developing-plugins/services/singleton' },
                            { slug: 'developing-plugins/services/scoped' },
                            { slug: 'developing-plugins/services/transient' }
                        ]
                    }
                ],
            },
            {
                label: 'Reference',
                autogenerate: { directory: 'reference' }
            }
        ],
        head: [
            {
                tag: 'script',
                attrs: { src: `https://www.googletagmanager.com/gtag/js?id=${googleAnalyticsId}` }
            },
            {
                tag: 'script',
                content: `
                    window.dataLayer = window.dataLayer || [];
                    function gtag(){dataLayer.push(arguments);}
                    gtag('js', new Date());
                    
                    gtag('config', '${googleAnalyticsId}');
                    `,
            }
        ]
    }), sitemap()],
});