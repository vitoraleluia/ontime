import type { components } from '../../generated/apiClient'

export type ErrorCode = components['schemas']['ErrorCode']
export type ErrorResponse = components['schemas']['ErrorResponse']

export class ErrorUtils {
  public static extractError(err: unknown): ErrorResponse | null {
    if (!err) return null
    if (typeof err === 'object' && err !== null) {
      const errorObj = err as Record<string, any>
      if (errorObj.errorCode || errorObj.message) {
        return {
          errorCode: errorObj.errorCode as ErrorCode,
          message: typeof errorObj.message === 'string' ? errorObj.message : undefined,
        }
      }
      if (errorObj.errors) {
        const messages = Object.values(errorObj.errors).flat()
        if (messages.length > 0) {
          return { message: messages.join(' ') }
        }
      }
      if (typeof errorObj.detail === 'string') {
        return { message: errorObj.detail }
      }
      if (typeof errorObj.title === 'string') {
        return { message: errorObj.title }
      }
    }
    if (typeof err === 'string') {
      return { message: err }
    }
    return null
  }

  public static extractMessage(err: unknown, fallbackMsg: string): string {
    const errorObj = this.extractError(err)
    if (errorObj?.message) return errorObj.message
    return fallbackMsg
  }

  public static extractCode(err: unknown): ErrorCode | undefined {
    return this.extractError(err)?.errorCode
  }
}
