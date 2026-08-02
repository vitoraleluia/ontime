import { useState } from 'react'
import { createFileRoute, useNavigate, useSearch, Link } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { CalendarRange, Mail, Lock, Loader2, AlertCircle } from 'lucide-react'
import { GoogleIcon } from '@/components/icons/GoogleIcon'

export const Route = createFileRoute('/login')({
  component: LoginPage,
})

function LoginPage() {
  const search = useSearch({ from: '/login' }) as { returnUrl?: string; error?: string }
  const returnUrl = search?.returnUrl ?? '/'
  const queryError = search?.error

  const { loginWithCredentials, loginWithGoogle, isAuthenticated } = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  let initialError: string | null = null
  if (queryError === 'GoogleAuthFailed') {
    initialError = 'Falha ao autenticar com a conta Google.'
  } else if (queryError === 'EmailMissing') {
    initialError = 'A conta Google não forneceu um endereço de email válido.'
  }

  const [error, setError] = useState<string | null>(initialError)
  const [isLoading, setIsLoading] = useState(false)

  // Redirect if already logged in
  if (isAuthenticated) {
    navigate({ to: returnUrl })
    return null
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email || !password) {
      setError('Por favor, preencha todos os campos.')
      return
    }

    setError(null)
    setIsLoading(true)

    const result = await loginWithCredentials(email, password)
    setIsLoading(false)

    if (result.success) {
      navigate({ to: returnUrl })
    } else {
      if (result.error && (result.error.toLowerCase().includes('não foi confirmado') || result.error.toLowerCase().includes('verifique a sua caixa'))) {
        navigate({ to: '/confirm-email-pending', search: { email: email.trim() } })
        return
      }
      setError(result.error ?? 'Falha ao iniciar sessão.')
    }
  }

  return (
    <div className="flex min-h-[80vh] flex-col items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
      <div className="w-full max-w-md space-y-8">
        {/* Header */}
        <div className="text-center">
          <Link to="/" className="inline-flex items-center gap-2 text-2xl font-bold tracking-tight text-foreground">
            <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground shadow-md shadow-primary/20">
              <CalendarRange className="h-6 w-6" />
            </div>
          </Link>
          <h2 className="mt-6 text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            Iniciar Sessão
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Bem-vindo de volta! Introduza as suas credenciais para aceder à sua conta.
          </p>
        </div>

        {/* Card */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8">
          
          {/* Error Alert */}
          {error && (
            <div className="mb-6 flex items-start gap-3 rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive text-sm font-medium animate-fade-in">
              <AlertCircle className="h-5 w-5 shrink-0 mt-0.5" />
              <div>{error}</div>
            </div>
          )}

          {/* External Providers ALWAYS FIRST */}
          <div className="space-y-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => loginWithGoogle(returnUrl)}
              className="relative flex w-full cursor-pointer items-center justify-center gap-3 rounded-xl border border-border bg-background py-2.5 text-sm font-semibold text-foreground hover:bg-muted transition-colors shadow-xs"
            >
              <GoogleIcon />
              <span>Continuar com Google</span>
            </Button>

            {/* Visual Divider */}
            <div className="relative my-6 flex items-center justify-center">
              <div className="w-full border-t border-border" />
              <span className="absolute bg-card px-3 text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                ou
              </span>
            </div>

            {/* Email/Password Form */}
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Endereço de Email
                </label>
                <div className="relative">
                  <Mail className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="exemplo@email.com"
                    required
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Palavra-passe
                </label>
                <div className="relative">
                  <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="••••••••"
                    required
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
                </div>
                <div className="mt-1.5 text-right">
                  <Link
                    to="/forgot-password"
                    className="text-xs font-semibold text-primary hover:underline"
                  >
                    Esqueceu-se da palavra-passe?
                  </Link>
                </div>
              </div>

              <Button
                type="submit"
                disabled={isLoading}
                className="mt-2 w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5"
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    A iniciar sessão...
                  </>
                ) : (
                  'Iniciar Sessão'
                )}
              </Button>
            </form>

            <div className="pt-4 text-center text-xs text-muted-foreground">
              Ainda não tem conta?{' '}
              <Link to="/register" search={{ returnUrl }} className="font-semibold text-primary hover:underline">
                Registe-se
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
