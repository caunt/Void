export const wrapUnderSummary = (markdown, headerMarker) => {
  const esc = headerMarker.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')
  const regex = new RegExp(`^${esc}\\s+(.*)`)
  const out = []

  let title = '', buffer = []

  const flush = () => {
    out.push(
        `<details>
        <summary>${title}</summary>

        ${buffer.join('\n')}
        </details>`
    )
  }

  for (const line of markdown.split(/\r?\n/)) {
    const match = regex.exec(line)

    if (match) {
      if (title) 
        flush()

      title = match[1]
      buffer = []
    } else if (title) {
      buffer.push(line)
    } else {
      out.push(line)
    }
  }

  if (title) 
    flush()
  
  return out.join('\n')
}
