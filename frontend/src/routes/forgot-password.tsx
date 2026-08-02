import { useState } from 'react'
import { createFileRoute, Link } from '@tanstack/react-router'
import { Button } from '@/components/ui/button'
import { CalendarRange, Mail, Loader2, AlertCircle, CheckCircle2, ArrowLeft } from 'lucide-react'
import { $api } from '@/lib/api'

export const Route = createFileRoute('/forgot-password')({
  component: ForgotPasswordPage,
})

function ForgotPasswordPage() {
  const [email, setEmail] = useState('')
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const forgotPasswordMutation = $api.useMutation('post', '/api/auth/forgot-password', {
    onError: (err: any) => {
      let msg = 'Falha ao enviar o pedido de recuperação. Tente novamente.'
      if (err?.detail) {
        msg = err.detail
      } else if (err?.title) {
        msg = err.title
      }
      setErrorMsg(msg)
    },
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!email) {
      setErrorMsg('Por favor, introduza o seu endereço de email.')
      return
    }

    setErrorMsg(null)
    forgotPasswordMutation.mutate({
      body: { email: email.trim() },
    })
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
            Recuperar Palavra-passe
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Introduza o seu endereço de email para receber as instruções de recuperação.
          </p>
        </div>

        {/* Card */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8">
          {forgotPasswordMutation.isSuccess ? (
            <SuccessState email={email} />
          ) : (
            <ForgotPasswordForm
              email={email}
              setEmail={setEmail}
              error={errorMsg}
              isLoading={forgotPasswordMutation.isPending}
              onSubmit={handleSubmit}
            />
          )}
        </div>
      </div>
    </div>
  )
}

function SuccessState({ email }: { email: string }) {
  return (
    <div className="space-y-6 text-center animate-fade-in">
      <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-emerald-500/10 text-emerald-600 dark:text-emerald-400">
        <CheckCircle2 className="h-8 w-8" />
      </div>

      <div className="space-y-2">
        <h3 className="text-lg font-bold text-foreground">Email Enviado com Sucesso</h3>
        <p className="text-sm text-muted-foreground">
          Enviámos as instruções de recuperação de palavra-passe para{' '}
          <span className="font-semibold text-foreground">{email}</span>.
        </p>
      </div>

      <div className="rounded-xl border border-emerald-500/20 bg-emerald-500/10 p-4 text-xs font-medium text-emerald-700 dark:text-emerald-300">
        Por favor, verifique a sua caixa de entrada e a pasta de spam. Siga os passos indicados no email para redefinir a sua palavra-passe.
      </div>

      <div className="pt-2">
        <Link to="/login">
          <Button variant="outline" className="w-full cursor-pointer rounded-xl font-semibold shadow-xs">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Voltar para Iniciar Sessão
          </Button>
        </Link>
      </div>
    </div>
  )
}

function ForgotPasswordForm({
  email,
  setEmail,
  error,
  isLoading,
  onSubmit,
}: {
  email: string
  setEmail: (val: string) => void
  error: string | null
  isLoading: boolean
  onSubmit: (e: React.FormEvent) => void
}) {
  return (
    <div className="space-y-4">
      {error && (
        <div className="flex items-start gap-3 rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive text-sm font-medium animate-fade-in">
          <AlertCircle className="h-5 w-5 shrink-0 mt-0.5" />
          <div>{error}</div>
        </div>
      )}

      <form onSubmit={onSubmit} className="space-y-4">
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

        <Button
          type="submit"
          disabled={isLoading}
          className="mt-2 w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5"
        >
          {isLoading ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              A enviar pedido...
            </>
          ) : (
            'Enviar Pedido de Recuperação'
          )}
        </Button>
      </form>

      <div className="pt-4 text-center text-xs text-muted-foreground">
        Lembrou-se da palavra-passe?{' '}
        <Link to="/login" className="font-semibold text-primary hover:underline">
          Voltar ao início de sessão
        </Link>
      </div>
    </div>
  )
}
