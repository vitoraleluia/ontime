export interface AuthState {
  isAuthenticated: boolean
  isLoading: boolean
}

export interface AuthContextType extends AuthState {
  loginWithCredentials: (email: string, password: string) => Promise<{ success: boolean; error?: string }>
  registerWithCredentials: (
    email: string,
    password: string,
    firstName: string,
    lastName: string,
    phoneNumber?: string
  ) => Promise<{ success: boolean; error?: string }>
  loginWithGoogle: (returnUrl?: string) => void
  logout: () => Promise<void>
  refetchAuth: () => Promise<void>
  forgotPassword: (email: string) => Promise<{ success: boolean; error?: string }>
  resetPassword: (email: string, token: string, newPassword: string) => Promise<{ success: boolean; error?: string }>
  confirmEmail: (userId: string, token: string) => Promise<{ success: boolean; error?: string }>
  resendConfirmationEmail: (email: string) => Promise<{ success: boolean; error?: string }>
}
