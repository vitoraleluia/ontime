export interface StoredTokens {
  token: string
  refreshToken: string
  idToken?: string
}

export interface AuthState {
  isAuthenticated: boolean
  isLoading: boolean
  token: string | null
}

export interface AuthContextType extends AuthState {
  login: (returnUrl?: string) => Promise<void>
  register: (returnUrl?: string) => Promise<void>
  logout: () => void
}
