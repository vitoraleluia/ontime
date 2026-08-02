export class StringUtils {
  public static buildFriendlyUrl(text: string, maxLength = 50): string {
    if (!text) return ''
    let clean = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase()
    clean = clean.replace(/[^a-z0-9\s-]/g, '')
    clean = clean.replace(/[\s-]+/g, '-').replace(/^-+|-+$/g, '')
    return clean.slice(0, maxLength).replace(/-+$/g, '')
  }
}
