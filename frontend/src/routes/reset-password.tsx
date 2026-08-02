import { useState } from 'react'
import { createFileRoute, useSearch, Link } from '@tanstack/react-router'
import { Button } from '@/components/ui/button'
import { CalendarRange, Lock, Loader2, AlertCircle, CheckCircle2, ArrowLeft } from 'lucide-react'
import { api } from '@/lib/api'

export const Route = createFileRoute('/reset-password')({
  component: ResetPasswordPage,
})

function ResetPasswordPage() {
  const search = useSearch({ from: '/reset-password' }) as { email?: string; token?: string }
  const email = search?.email ?? ''
  const token = search?.token ?? ''

  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [isSuccess, setIsSuccess] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const isInvalidLink = !email || !token

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!newPassword || !confirmPassword) {
      setError('Por favor, preencha todos os campos.')
      return
    }

    if (newPassword !== confirmPassword) {
      setError('As palavras-passe não coincidem.')
      return
    }

    if (newPassword.length < 6) {
      setError('A palavra-passe deve ter pelo menos 6 caracteres.')
      return
    }

    setError(null)
    setIsLoading(true)

    try {
      await api.POST('/api/auth/reset-password', {
        body: {
          email,
          token,
          newPassword,
        },
      })
      setIsSuccess(true)
    } catch (err: any) {
      let errorMsg = 'Falha ao redefinir a palavra-passe. O link pode ter expirado.'
      if (err?.detail) {
        errorMsg = err.detail
      } else if (err?.title) {
        errorMsg = err.title
      }
      setError(errorMsg)
    } finally {
      setIsLoading(false)
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
            Redefinir Palavra-passe
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Introduza a sua nova palavra-passe nos campos abaixo.
          </p>
        </div>

        {/* Card */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8">
          <ResetPasswordCardContent
            isInvalidLink={isInvalidLink}
            isSuccess={isSuccess}
            newPassword={newPassword}
            setNewPassword={setNewPassword}
            confirmPassword={confirmPassword}
            setConfirmPassword={setConfirmPassword}
            error={error}
            isLoading={isLoading}
            onSubmit={handleSubmit}
          />
        </div>
      </div>
    </div>
  )
}

function ResetPasswordCardContent({
  isInvalidLink,
  isSuccess,
  newPassword,
  setNewPassword,
  confirmPassword,
  setConfirmPassword,
  error,
  isLoading,
  onSubmit,
}: {
  isInvalidLink: boolean
  isSuccess: boolean
  newPassword: string
  setNewPassword: (val: string) => void
  confirmPassword: string
  setConfirmPassword: (val: string) => void
  error: string | null
  isLoading: boolean
  onSubmit: (e: React.FormEvent) => void
}) {
  if (isInvalidLink) {
    return <InvalidLinkState />
  }

  if (isSuccess) {
    return <ResetSuccessState />
  }

  return (
    <ResetPasswordForm
      newPassword={newPassword}
      setNewPassword={setNewPassword}
      confirmPassword={confirmPassword}
      setConfirmPassword={setConfirmPassword}
      error={error}
      isLoading={isLoading}
      onSubmit={onSubmit}
    />
  )
}

function InvalidLinkState() {
  return (
    <div className="space-y-6 text-center animate-fade-in">
      <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-destructive/10 text-destructive">
        <AlertCircle className="h-8 w-8" />
      </div>

      <div className="space-y-2">
        <h3 className="text-lg font-bold text-foreground">Link Inválido ou Incompleto</h3>
        <p className="text-sm text-muted-foreground">
          O link de recuperação de palavra-passe é inválido ou faltam dados de verificação.
        </p>
      </div>

      <div className="pt-2">
        <Link to="/forgot-password">
          <Button className="w-full cursor-pointer rounded-xl font-semibold shadow-xs">
            Solicitar Novo Link de Recuperação
          </Button>
        </Link>
      </div>
    </div>
  )
}

function ResetSuccessState() {
  return (
    <div className="space-y-6 text-center animate-fade-in">
      <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-emerald-500/10 text-emerald-600 dark:text-emerald-400">
        <CheckCircle2 className="h-8 w-8" />
      </div>

      <div className="space-y-2">
        <h3 className="text-lg font-bold text-foreground">Palavra-passe Redefinida!</h3>
        <p className="text-sm text-muted-foreground">
          A sua palavra-passe foi alterada com sucesso. Já pode aceder à sua conta com as novas credenciais.
        </p>
      </div>

      <div className="pt-2">
        <Link to="/login">
          <Button className="w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Iniciar Sessão Agora
          </Button>
        </Link>
      </div>
    </div>
  )
}

function ResetPasswordForm({
  newPassword,
  setNewPassword,
  confirmPassword,
  setConfirmPassword,
  error,
  isLoading,
  onSubmit,
}: {
  newPassword: string
  setNewPassword: (val: string) => void
  confirmPassword: string
  setConfirmPassword: (val: string) => void
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
            Nova Palavra-passe
          </label>
          <div className="relative">
            <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              placeholder="Mínimo 6 caracteres"
              required
              disabled={isLoading}
              className="w-full rounded-xl border border-border bg-background py-2.5 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
            />
          </div>
        </div>

        <div>
          <label className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1.5">
            Confirmar Nova Palavra-passe
          </label>
          <div className="relative">
            <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Repita a nova palavra-passe"
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
              A redefinir...
            </>
          ) : (
            'Redefinir Palavra-passe'
          )}
        </Button>
      </form>
    </div>
  )
}
