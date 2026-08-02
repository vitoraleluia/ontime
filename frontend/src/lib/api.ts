import createFetchClient from 'openapi-fetch'
import createClient from 'openapi-react-query'
import type { paths } from '@/generated/apiClient'

const api = createFetchClient<paths>({
  baseUrl: '/', // Vite proxy routes `/api` to `http://localhost:3000`
  credentials: 'include',
})

export const $api = createClient(api)
