import { createContext, useContext } from 'react'
import type { ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import type { AuthContextType } from '@/domain/auth'
import { $api } from '@/lib/api'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient()

  const {
    data: profile,
    isLoading,
    isError,
    refetch,
  } = $api.useQuery(
    'get',
    '/api/Account',
    {},
    {
      retry: false,
    }
  )

  const loginMutation = $api.useMutation('post', '/api/Auth/login')
  const registerMutation = $api.useMutation('post', '/api/Auth/register')
  const logoutMutation = $api.useMutation('post', '/api/Auth/logout')
  const forgotPasswordMutation = $api.useMutation('post', '/api/Auth/forgot-password')
  const resetPasswordMutation = $api.useMutation('post', '/api/Auth/reset-password')
  const confirmEmailMutation = $api.useMutation('post', '/api/Auth/confirm-email')
  const resendConfirmationEmailMutation = $api.useMutation('post', '/api/Auth/resend-confirmation-email')

  const isAuthenticated = !isLoading && !isError && !!profile

  const loginWithCredentials = async (email: string, password: string) => {
    try {
      await loginMutation.mutateAsync({
        body: { email, password },
      })
      await queryClient.invalidateQueries()
      await refetch()
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Credenciais inválidas. Verifique o email e a palavra-passe.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const registerWithCredentials = async (
    email: string,
    password: string,
    firstName: string,
    lastName: string,
    phoneNumber?: string
  ) => {
    try {
      await registerMutation.mutateAsync({
        body: {
          email,
          password,
          firstName,
          lastName,
          phoneNumber: phoneNumber ?? undefined,
        },
      })
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Erro ao criar conta. Verifique os dados fornecidos.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const loginWithGoogle = (returnUrl?: string) => {
    const targetUrl = returnUrl
      ? `/api/GoogleAuth/login?returnUrl=${encodeURIComponent(returnUrl)}`
      : '/api/GoogleAuth/login'
    window.location.href = targetUrl
  }

  const logout = async () => {
    try {
      await logoutMutation.mutateAsync({})
    } catch {
      // Ignore network errors on logout
    } finally {
      queryClient.setQueryData(['get', '/api/Account'], null)
      await queryClient.invalidateQueries()
    }
  }

  const forgotPassword = async (email: string) => {
    try {
      await forgotPasswordMutation.mutateAsync({
        body: { email },
      })
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Erro ao processar pedido.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const resetPassword = async (email: string, token: string, newPassword: string) => {
    try {
      await resetPasswordMutation.mutateAsync({
        body: { email, token, newPassword },
      })
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Erro ao redefinir palavra-passe.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const confirmEmail = async (userId: string, token: string) => {
    try {
      await confirmEmailMutation.mutateAsync({
        body: { userId, token },
      })
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Erro ao confirmar email.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const resendConfirmationEmail = async (email: string) => {
    try {
      await resendConfirmationEmailMutation.mutateAsync({
        body: { email },
      })
      return { success: true }
    } catch (err: unknown) {
      const errorMsg = ErrorUtils.extractMessage(err, 'Erro ao reenviar confirmação de email.')
      const errorCode = ErrorUtils.extractCode(err)
      return { success: false, error: errorMsg, errorCode }
    }
  }

  const refetchAuth = async () => {
    await refetch()
  }

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        isLoading,
        profile: profile ?? null,
        loginWithCredentials,
        registerWithCredentials,
        loginWithGoogle,
        logout,
        refetchAuth,
        forgotPassword,
        resetPassword,
        confirmEmail,
        resendConfirmationEmail,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
