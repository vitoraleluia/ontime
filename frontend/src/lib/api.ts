import createFetchClient from 'openapi-fetch'
import createClient from 'openapi-react-query'
import type { paths } from '@/generated/apiClient'
import { LocalStoreKeys } from '@/domain/constants/localStoreKeys'
import type { StoredTokens } from '@/domain/auth'

export const api = createFetchClient<paths>({
  baseUrl: '/', // Vite proxy routes `/api` to `http://localhost:3000`
})

api.use({
  async onRequest({ request }) {
    const tokensStr = localStorage.getItem(LocalStoreKeys.AuthTokens)
    if (tokensStr) {
      try {
        const tokens = JSON.parse(tokensStr) as StoredTokens
        if (tokens.token) {
          request.headers.set('Authorization', `Bearer ${tokens.token}`)
        }
      } catch (e) {
        console.warn('Failed to parse tokens in API client middleware', e)
      }
    }
    return request
  }
})

export const $api = createClient(api)

