// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import sitemap from '@astrojs/sitemap';

import { ExpressiveCodeTheme } from '@astrojs/starlight/expressive-code'
import fs from 'node:fs'

import starlightLinksValidator from 'starlight-links-validator'

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
            {
                label: 'Containers',
                autogenerate: { directory: 'containers' }
            },
            {
                label: 'Forwarding',
                autogenerate: { directory: 'forwardings' }
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
                    { slug: 'developing-plugins/text-formatting' },
                    { slug: 'developing-plugins/nbt' },
                    { slug: 'developing-plugins/configuration' },
                    {
                        label: 'Network',
                        collapsed: true,
                        badge: { text: 'expert', variant: 'danger' },
                        autogenerate: { directory: 'developing-plugins/network' }
                    },
                    { slug: 'developing-plugins/serializers' }
                ],
            },
            { slug: 'faq' },
            { slug: 'troubleshooting' }
        ],
        head: [
            {
                tag: 'meta',
                attrs: {
                    property: 'og:image',
                    content: 'https://void.caunt.world/logo-text-horizontal.svg'
                }
            },
            {
                tag: 'meta',
                attrs: {
                    property: 'twitter:image',
                    content: 'https://void.caunt.world/logo-text-horizontal.svg'
                }
            },
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
                    `
            },
            {
                tag: 'script',
                attrs: { type: 'application/ld+json' },
                content: `
                    {
                        "@context": "https://schema.org",
                        "@type": "SoftwareSourceCode",
                        "name": "Void",
                        "url": "https://github.com/caunt/Void",
                        "codeRepository": "https://github.com/caunt/Void",
                        "author": {
                            "@type": "Person",
                            "name": "Vitalii",
                            "url": "https://github.com/caunt"
                        }
                    }
                    `
            }
        ],
        editLink: {
            baseUrl: 'https://github.com/caunt/void/edit/main/docs/astro'
        },
        lastUpdated: true,
        expressiveCode: {
            themes: [ExpressiveCodeTheme.fromJSONString(fs.readFileSync(new URL(`./themes/visual-studio-2019-dark.jsonc`, import.meta.url), 'utf-8'))],
            tabWidth: 4
        },
        logo: {
            src: '/public/logo.svg'
        },
        favicon: '/logo.svg',
        customCss: ['./src/assets/landing.css'],
        plugins: [starlightLinksValidator()]
    }), sitemap({
        changefreq: 'daily',
        priority: 1,
        lastmod: new Date()
    })]
});