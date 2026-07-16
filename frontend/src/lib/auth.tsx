import { createContext, useContext, useState, useEffect, useRef } from 'react'
import type { ReactNode } from 'react'
import Keycloak from 'keycloak-js'
import type { StoredTokens, AuthState, AuthContextType } from '@/domain/auth'
import { LocalStoreKeys } from '@/domain/constants/localStoreKeys'

const AuthContext = createContext<AuthContextType | undefined>(undefined)

const keycloakInstance = new Keycloak({
  url: "http://localhost:5000",
  realm: "OnTime",
  clientId: "ontime-api",
})

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    isAuthenticated: false,
    isLoading: true,
    token: null,
  })
  
  const isInitialized = useRef(false)

  useEffect(() => {
    if (isInitialized.current) return
    isInitialized.current = true

    let intervalId: any = null

    const initAuth = async () => {
      // Retrieve stored tokens if they exist
      const storedTokensStr = localStorage.getItem(LocalStoreKeys.AuthTokens)
      let storedTokens: StoredTokens | null = null
      try {
        if (storedTokensStr) {
          storedTokens = JSON.parse(storedTokensStr)
        }
      } catch (e) {
        console.warn("Failed to parse stored auth tokens", e)
        localStorage.removeItem(LocalStoreKeys.AuthTokens)
      }

      const initOptions: any = {
        checkLoginIframe: false,
        pkceMethod: "S256",
      }

      if (storedTokens) {
        initOptions.token = storedTokens.token
        initOptions.refreshToken = storedTokens.refreshToken
        if (storedTokens.idToken) {
          initOptions.idToken = storedTokens.idToken
        }
      }

      try {
        const authenticated = await keycloakInstance.init(initOptions)
        
        if (authenticated) {
          localStorage.setItem(
            LocalStoreKeys.AuthTokens,
            JSON.stringify({
              token: keycloakInstance.token,
              refreshToken: keycloakInstance.refreshToken,
              idToken: keycloakInstance.idToken,
            })
          )
        } else {
          localStorage.removeItem(LocalStoreKeys.AuthTokens)
        }

        setState({
          isAuthenticated: authenticated,
          token: keycloakInstance.token || null,
          isLoading: false,
        })

        // Set up periodic token refresh (check every 30s, refresh if expiring within 30s)
        intervalId = setInterval(async () => {
          try {
            const refreshed = await keycloakInstance.updateToken(30)
            if (refreshed) {
              localStorage.setItem(
                LocalStoreKeys.AuthTokens,
                JSON.stringify({
                  token: keycloakInstance.token,
                  refreshToken: keycloakInstance.refreshToken,
                  idToken: keycloakInstance.idToken,
                })
              )
              setState(prev => ({
                ...prev,
                token: keycloakInstance.token || null,
              }))
            }
          } catch (err) {
            console.error("Failed to refresh token", err)
            logout()
          }
        }, 30000)
      } catch (err) {
        console.error("Keycloak initialization failed", err)
        localStorage.removeItem(LocalStoreKeys.AuthTokens)
        setState({
          isAuthenticated: false,
          token: null,
          isLoading: false,
        })
      }
    }

    initAuth()

    return () => {
      if (intervalId) {
        clearInterval(intervalId)
      }
    }
  }, [])

  const login = async (returnUrl?: string) => {
    const redirectUri = returnUrl
      ? window.location.origin + returnUrl
      : window.location.href
    await keycloakInstance.login({ redirectUri })
  }

  const register = async (returnUrl?: string) => {
    const redirectUri = returnUrl
      ? window.location.origin + returnUrl
      : window.location.href
    await keycloakInstance.register({ redirectUri })
  }

  const logout = () => {
    localStorage.removeItem(LocalStoreKeys.AuthTokens)
    keycloakInstance.logout({
      redirectUri: window.location.origin + "/",
    })
  }

  return (
    <AuthContext.Provider
      value={{
        ...state,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
