import { defineRouteMiddleware } from '@astrojs/starlight/route-data'

export const onRequest = defineRouteMiddleware(({ locals, site, request }) => {
  const url = new URL(request.url)
  const canonical = new URL(url.pathname, site).href

  locals.starlightRoute.head.push({
    tag: 'meta',
    attrs: { property: 'og:url', content: canonical },
  })

  locals.starlightRoute.head.push({
    tag: 'meta',
    attrs: { name: 'twitter:url', content: canonical },
  })
})