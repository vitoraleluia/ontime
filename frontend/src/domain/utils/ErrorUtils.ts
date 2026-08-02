export class ErrorUtils {
  public static extractMessage(err: unknown, fallbackMsg: string): string {
    if (!err) return fallbackMsg
    if (typeof err === 'string') return err
    if (typeof err === 'object' && err !== null) {
      const errorObj = err as Record<string, any>
      if (errorObj.errors) {
        const messages = Object.values(errorObj.errors).flat()
        if (messages.length > 0) return messages.join(' ')
      }
      if (typeof errorObj.detail === 'string') return errorObj.detail
      if (typeof errorObj.title === 'string') return errorObj.title
    }
    return fallbackMsg
  }
}
