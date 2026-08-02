import { useState } from 'react'
import { createFileRoute, useNavigate, useSearch, Link } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { CalendarRange, Mail, Lock, User, Phone, Loader2, AlertCircle } from 'lucide-react'
import { GoogleIcon } from '@/components/icons/GoogleIcon'

export class RegisterFormFields {
  public static readonly FIRST_NAME = 'firstName'
  public static readonly LAST_NAME = 'lastName'
  public static readonly PHONE_NUMBER = 'phoneNumber'
  public static readonly EMAIL = 'email'
  public static readonly PASSWORD = 'password'
  public static readonly CONFIRM_PASSWORD = 'confirmPassword'
}

export const Route = createFileRoute('/register')({
  component: RegisterPage,
})

const PT_PHONE_REGEX = /^(\+351)?9\d{8}$/

function RegisterPage() {
  const search = useSearch({ from: '/register' }) as { returnUrl?: string }
  const returnUrl = search?.returnUrl ?? '/'

  const { registerWithCredentials, loginWithGoogle, isAuthenticated } = useAuth()
  const navigate = useNavigate()

  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  // Redirect if already logged in
  if (isAuthenticated) {
    navigate({ to: returnUrl })
    return null
  }

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const firstName = (formData.get(RegisterFormFields.FIRST_NAME) as string)?.trim()
    const lastName = (formData.get(RegisterFormFields.LAST_NAME) as string)?.trim()
    const phoneNumber = (formData.get(RegisterFormFields.PHONE_NUMBER) as string)?.trim() ?? ''
    const email = (formData.get(RegisterFormFields.EMAIL) as string)?.trim()
    const password = formData.get(RegisterFormFields.PASSWORD) as string
    const confirmPassword = formData.get(RegisterFormFields.CONFIRM_PASSWORD) as string

    if (!firstName || !lastName || !email || !password || !confirmPassword) {
      setError('Por favor, preencha todos os campos obrigatórios.')
      return
    }

    if (phoneNumber && !PT_PHONE_REGEX.test(phoneNumber)) {
      setError('O número de telemóvel deve ser um número português válido (ex: 927431783).')
      return
    }

    if (password !== confirmPassword) {
      setError('As palavras-passe não coincidem.')
      return
    }

    if (password.length < 6) {
      setError('A palavra-passe deve ter pelo menos 6 caracteres.')
      return
    }

    setError(null)
    setIsLoading(true)

    const result = await registerWithCredentials(
      email,
      password,
      firstName,
      lastName,
      phoneNumber || undefined
    )
    setIsLoading(false)

    if (result.success) {
      navigate({ to: '/confirm-email-pending', search: { email } })
    } else {
      setError(result.error ?? 'Falha ao criar conta.')
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
            Criar Conta
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Registe-se e comece a agendar os seus serviços facilmente.
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

            {/* Registration Form */}
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                    Primeiro Nome *
                  </label>
                  <div className="relative">
                    <User className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                    <input
                      type="text"
                      name={RegisterFormFields.FIRST_NAME}
                      placeholder="João"
                      required
                      minLength={2}
                      maxLength={50}
                      disabled={isLoading}
                      className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                    Apelido *
                  </label>
                  <div className="relative">
                    <User className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                    <input
                      type="text"
                      name={RegisterFormFields.LAST_NAME}
                      placeholder="Silva"
                      required
                      minLength={2}
                      maxLength={50}
                      disabled={isLoading}
                      className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                    />
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Telemóvel <span className="text-muted-foreground font-normal">(Opcional)</span>
                </label>
                <div className="relative">
                  <Phone className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="tel"
                    name={RegisterFormFields.PHONE_NUMBER}
                    placeholder="927431783"
                    pattern="^(\+351)?9\d{8}$"
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Endereço de Email *
                </label>
                <div className="relative">
                  <Mail className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="email"
                    name={RegisterFormFields.EMAIL}
                    placeholder="exemplo@email.com"
                    required
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Palavra-passe *
                </label>
                <div className="relative">
                  <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="password"
                    name={RegisterFormFields.PASSWORD}
                    placeholder="Mínimo 6 caracteres"
                    required
                    minLength={6}
                    maxLength={100}
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
                </div>
              </div>

              <div>
                <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
                  Confirmar Palavra-passe *
                </label>
                <div className="relative">
                  <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
                  <input
                    type="password"
                    name={RegisterFormFields.CONFIRM_PASSWORD}
                    placeholder="Repita a palavra-passe"
                    required
                    minLength={6}
                    maxLength={100}
                    disabled={isLoading}
                    className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
                  />
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
                    A criar conta...
                  </>
                ) : (
                  'Criar Conta'
                )}
              </Button>
            </form>

            <div className="pt-4 text-center text-xs text-muted-foreground">
              Já tem uma conta?{' '}
              <Link to="/login" search={{ returnUrl }} className="font-semibold text-primary hover:underline">
                Inicie sessão
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
