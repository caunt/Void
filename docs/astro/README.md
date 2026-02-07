# Void Documentation Site

[![Built with Starlight](https://astro.badg.es/v2/built-with-starlight/tiny.svg)](https://starlight.astro.build)

This directory contains the source files for the Void Proxy documentation website, built with Astro and Starlight.

The live documentation is available at [**https://void.caunt.world**](https://void.caunt.world).

## 🚀 Project Structure

```
.
├── public/               # Static assets (favicons, images)
├── src/
│   ├── assets/          # Optimized images and assets
│   ├── content/
│   │   └── docs/        # Documentation content (.md and .mdx files)
│   └── content.config.ts
├── astro.config.mjs     # Astro configuration
├── package.json
└── tsconfig.json
```

Documentation files are located in `src/content/docs/`. Each file is exposed as a route based on its file name.

## 🧞 Commands

All commands are run from this directory (`docs/astro`):

| Command                   | Action                                           |
| :------------------------ | :----------------------------------------------- |
| `npm install`             | Installs dependencies                            |
| `npm run dev`             | Starts local dev server at `localhost:4321`      |
| `npm run build`           | Build your production site to `./dist/`          |
| `npm run preview`         | Preview your build locally, before deploying     |
| `npm run astro ...`       | Run CLI commands like `astro add`, `astro check` |
| `npm run astro -- --help` | Get help using the Astro CLI                     |

## 📝 Contributing

When contributing to the documentation:

- Follow the [**documentation guidelines**](../../AGENTS.md#documentation) in the repository.
- Make link text bold: `[**link text**](https://example.com)`
- Never include inline code or backticks inside a link caption.

## 🔗 Resources

- [**Starlight documentation**](https://starlight.astro.build/)
- [**Astro documentation**](https://docs.astro.build)
- [**Main repository**](https://github.com/caunt/Void)
