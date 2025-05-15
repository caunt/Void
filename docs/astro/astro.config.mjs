// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';

import { ExpressiveCodeTheme } from '@astrojs/starlight/expressive-code'
import fs from 'node:fs'

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
                autogenerate: { directory: 'getting-started' }
            },
            {
                label: 'Configuration',
                autogenerate: { directory: 'configuration' }
            },
            { slug: 'watchdog' },
            {
                label: 'Developing Plugins',
                items: [
                    { slug: 'developing-plugins/development-kit' },
                    {
                        label: 'Events',
                        collapsed: true,
                        autogenerate: { directory: 'developing-plugins/events' }
                    },
                    {
                        label: 'Services',
                        collapsed: true,
                        autogenerate: { directory: 'developing-plugins/services' }
                    },
                    { slug: 'developing-plugins/commands' },
                    { slug: 'developing-plugins/text-components' },
                    { slug: 'developing-plugins/nbt' },
                    {
                        label: 'Network',
                        autogenerate: { directory: 'developing-plugins/network' }
                    },
                    {
                        label: 'Forwarding',
                        collapsed: true,
                        autogenerate: { directory: 'developing-plugins/forwarding' }
                    },
                    { slug: 'developing-plugins/serializers' }
                ],
            },
            {
                label: 'Containers',
                autogenerate: { directory: 'containers' }
            },
            { slug: 'faq' },
            { slug: 'troubleshooting' }
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
        ],
        editLink: {
            baseUrl: 'https://github.com/caunt/void/edit/main/docs/astro',
        },
        lastUpdated: true,
        expressiveCode: {
            themes: [ExpressiveCodeTheme.fromJSONString(fs.readFileSync(new URL(`./themes/visual-studio-2019-dark.jsonc`, import.meta.url), 'utf-8'))]
        },
        logo: {
            src: '/public/logo.svg'
        },
        favicon: '/logo.svg',
        customCss: ['./src/assets/landing.css']
    }), sitemap({
        changefreq: 'daily',
        priority: 1,
        lastmod: new Date()
    })],
});