export interface AuthState {
  isAuthenticated: boolean
  isLoading: boolean
}

export interface AuthContextType extends AuthState {
  loginWithCredentials: (email: string, password: string) => Promise<{ success: boolean; error?: string }>
  registerWithCredentials: (email: string, password: string) => Promise<{ success: boolean; error?: string }>
  loginWithGoogle: (returnUrl?: string) => void
  logout: () => Promise<void>
  refetchAuth: () => Promise<void>
}
