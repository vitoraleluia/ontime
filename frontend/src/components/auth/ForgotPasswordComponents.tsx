import { Link } from '@tanstack/react-router'
import { Button } from '@/components/ui/button'
import { Mail, Loader2, AlertCircle, CheckCircle2, ArrowLeft } from 'lucide-react'

export class ForgotPasswordFormFields {
  public static readonly EMAIL = 'email'
}

export function ForgotPasswordSuccessState({ email }: { email: string }) {
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

export function ForgotPasswordForm({
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
            Endereço de Email
          </label>
          <div className="relative">
            <Mail className="absolute left-3.5 top-3 h-4 w-4 text-muted-foreground" />
            <input
              type="email"
              name={ForgotPasswordFormFields.EMAIL}
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
