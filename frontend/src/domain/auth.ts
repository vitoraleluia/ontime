import type { ErrorCode } from './utils/ErrorUtils'

export interface AuthState {
  isAuthenticated: boolean
  isLoading: boolean
}

export interface AuthContextType extends AuthState {
  loginWithCredentials: (email: string, password: string) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
  registerWithCredentials: (
    email: string,
    password: string,
    firstName: string,
    lastName: string,
    phoneNumber?: string
  ) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
  loginWithGoogle: (returnUrl?: string) => void
  logout: () => Promise<void>
  refetchAuth: () => Promise<void>
  forgotPassword: (email: string) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
  resetPassword: (email: string, token: string, newPassword: string) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
  confirmEmail: (userId: string, token: string) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
  resendConfirmationEmail: (email: string) => Promise<{ success: boolean; error?: string; errorCode?: ErrorCode }>
}
