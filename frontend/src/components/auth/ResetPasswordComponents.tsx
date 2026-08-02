import { Link } from '@tanstack/react-router'
import { Button } from '@/components/ui/button'
import { Lock, Loader2, AlertCircle, CheckCircle2, ArrowLeft } from 'lucide-react'

export class ResetPasswordFormFields {
  public static readonly NEW_PASSWORD = 'newPassword'
  public static readonly CONFIRM_PASSWORD = 'confirmPassword'
}

export function ResetPasswordCardContent({
  isInvalidLink,
  isSuccess,
  error,
  isLoading,
  onSubmit,
}: {
  isInvalidLink: boolean
  isSuccess: boolean
  error: string | null
  isLoading: boolean
  onSubmit: (e: React.FormEvent<HTMLFormElement>) => void
}) {
  if (isInvalidLink) {
    return <InvalidLinkState />
  }

  if (isSuccess) {
    return <ResetSuccessState />
  }

  return (
    <ResetPasswordForm
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
  error,
  isLoading,
  onSubmit,
}: {
  error: string | null
  isLoading: boolean
  onSubmit: (e: React.FormEvent<HTMLFormElement>) => void
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
              name={ResetPasswordFormFields.NEW_PASSWORD}
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
            Confirmar Nova Palavra-passe
          </label>
          <div className="relative">
            <Lock className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
            <input
              type="password"
              name={ResetPasswordFormFields.CONFIRM_PASSWORD}
              placeholder="Repita a nova palavra-passe"
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
