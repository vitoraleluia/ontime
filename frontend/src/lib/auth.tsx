import { createContext, useContext } from 'react'
import type { ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import type { AuthContextType } from '@/domain/auth'
import { $api } from '@/lib/api'

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

  const loginMutation = $api.useMutation('post', '/api/auth/login')
  const registerMutation = $api.useMutation('post', '/api/auth/register')
  const logoutMutation = $api.useMutation('post', '/api/auth/logout')
  const forgotPasswordMutation = $api.useMutation('post', '/api/auth/forgot-password')
  const resetPasswordMutation = $api.useMutation('post', '/api/auth/reset-password')
  const confirmEmailMutation = $api.useMutation('post', '/api/auth/confirm-email')
  const resendConfirmationEmailMutation = $api.useMutation('post', '/api/auth/resend-confirmation-email')

  const isAuthenticated = !isLoading && !isError && !!profile

  const loginWithCredentials = async (email: string, password: string) => {
    try {
      await loginMutation.mutateAsync({
        body: { email, password },
      })
      await queryClient.invalidateQueries()
      await refetch()
      return { success: true }
    } catch (err: any) {
      let errorMsg = 'Credenciais inválidas. Verifique o email e a palavra-passe.'
      if (err) {
        if (typeof err === 'string') errorMsg = err
        else if (err?.detail) errorMsg = err.detail
        else if (err?.title) errorMsg = err.title
      }
      return { success: false, error: errorMsg }
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
    } catch (err: any) {
      let errorMsg = 'Erro ao criar conta. Verifique os dados fornecidos.'
      if (err) {
        if (typeof err === 'string') {
          errorMsg = err
        } else if (err?.errors) {
          const messages = Object.values(err.errors).flat()
          if (messages.length > 0) errorMsg = messages.join(' ')
        } else if (err?.detail) {
          errorMsg = err.detail
        }
      }
      return { success: false, error: errorMsg }
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
    } catch (err: any) {
      const errorMsg = typeof err === 'string' ? err : err?.detail || 'Erro ao processar pedido.'
      return { success: false, error: errorMsg }
    }
  }

  const resetPassword = async (email: string, token: string, newPassword: string) => {
    try {
      await resetPasswordMutation.mutateAsync({
        body: { email, token, newPassword },
      })
      return { success: true }
    } catch (err: any) {
      const errorMsg = typeof err === 'string' ? err : err?.detail || 'Erro ao redefinir palavra-passe.'
      return { success: false, error: errorMsg }
    }
  }

  const confirmEmail = async (userId: string, token: string) => {
    try {
      await confirmEmailMutation.mutateAsync({
        body: { userId, token },
      })
      return { success: true }
    } catch (err: any) {
      const errorMsg = typeof err === 'string' ? err : err?.detail ?? 'Erro ao confirmar email.'
      return { success: false, error: errorMsg }
    }
  }

  const resendConfirmationEmail = async (email: string) => {
    try {
      await resendConfirmationEmailMutation.mutateAsync({
        body: { email },
      })
      return { success: true }
    } catch (err: any) {
      const errorMsg = typeof err === 'string' ? err : err?.detail ?? 'Erro ao reenviar confirmação de email.'
      return { success: false, error: errorMsg }
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
