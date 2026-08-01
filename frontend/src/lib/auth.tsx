import { createContext, useContext, useState, useEffect, useCallback } from 'react'
import type { ReactNode } from 'react'
import type { AuthState, AuthContextType } from '@/domain/auth'
import { api } from '@/lib/api'

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    isAuthenticated: false,
    isLoading: true,
  })

  const checkAuth = useCallback(async () => {
    try {
      const { response } = await api.GET('/api/Account')

      if (response.ok) {
        setState({ isAuthenticated: true, isLoading: false })
      } else {
        setState({ isAuthenticated: false, isLoading: false })
      }
    } catch {
      setState({ isAuthenticated: false, isLoading: false })
    }
  }, [])

  useEffect(() => {
    checkAuth()
  }, [checkAuth])

  const loginWithCredentials = async (email: string, password: string) => {
    try {
      const { response, error } = await api.POST('/api/auth/login', {
        params: {
          query: { useCookies: true },
        },
        body: { email, password },
      })

      if (response.ok) {
        await checkAuth()
        return { success: true }
      }

      let errorMsg = 'Credenciais inválidas. Verifique o email e a palavra-passe.'
      if (error) {
        const errData = error as any
        if (errData?.detail) errorMsg = errData.detail
        else if (errData?.title) errorMsg = errData.title
      }

      return { success: false, error: errorMsg }
    } catch (err: any) {
      return { success: false, error: err?.message || 'Erro ao comunicar com o servidor.' }
    }
  }

  const registerWithCredentials = async (email: string, password: string) => {
    try {
      const { response, error } = await api.POST('/api/auth/register', {
        body: { email, password },
      })

      if (response.ok) {
        // Automatically log in after registration
        return await loginWithCredentials(email, password)
      }

      let errorMsg = 'Erro ao criar conta. Verifique os dados fornecidos.'
      if (error) {
        const errData = error as any
        if (errData?.errors) {
          const messages = Object.values(errData.errors).flat()
          if (messages.length > 0) errorMsg = messages.join(' ')
        } else if (errData?.detail) {
          errorMsg = errData.detail
        }
      }

      return { success: false, error: errorMsg }
    } catch (err: any) {
      return { success: false, error: err?.message || 'Erro ao comunicar com o servidor.' }
    }
  }

  const loginWithGoogle = (returnUrl?: string) => {
    const targetUrl = returnUrl
      ? `/api/auth/login/google?returnUrl=${encodeURIComponent(returnUrl)}`
      : '/api/auth/login/google'
    window.location.href = targetUrl
  }

  const logout = async () => {
    try {
      await api.POST('/api/auth/logout', {
        body: {} as Record<string, never>,
      })
    } catch {
      // Ignore network errors on logout
    } finally {
      setState({ isAuthenticated: false, isLoading: false })
    }
  }

  return (
    <AuthContext.Provider
      value={{
        ...state,
        loginWithCredentials,
        registerWithCredentials,
        loginWithGoogle,
        logout,
        refetchAuth: checkAuth,
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
