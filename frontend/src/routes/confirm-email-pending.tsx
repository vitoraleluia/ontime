import { useState, useEffect } from 'react'
import { createFileRoute, useSearch, Link } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { Mail, RefreshCw, CheckCircle2, AlertCircle, ExternalLink } from 'lucide-react'

export const Route = createFileRoute('/confirm-email-pending')({
  component: ConfirmEmailPendingPage,
})

function ConfirmEmailPendingPage() {
  const search = useSearch({ from: '/confirm-email-pending' }) as { email?: string }
  const email = search?.email ?? ''

  const { resendConfirmationEmail } = useAuth()

  const [isLoading, setIsLoading] = useState(false)
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null)
  const [cooldown, setCooldown] = useState(0)

  useEffect(() => {
    if (cooldown <= 0) return
    const timer = setInterval(() => setCooldown((prev) => prev - 1), 1000)
    return () => clearInterval(timer)
  }, [cooldown])

  const handleResend = async () => {
    if (!email) {
      setMessage({ type: 'error', text: 'Endereço de email não especificado.' })
      return
    }

    setIsLoading(true)
    setMessage(null)

    const result = await resendConfirmationEmail(email)
    setIsLoading(false)

    if (result.success) {
      setMessage({ type: 'success', text: 'Email de confirmação reenviado com sucesso! Verifique a sua caixa de entrada.' })
      setCooldown(30)
    } else {
      setMessage({ type: 'error', text: result.error ?? 'Falha ao reenviar o email de confirmação.' })
    }
  }

  return (
    <div className="flex min-h-[80vh] flex-col items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
      <div className="w-full max-w-md space-y-8">
        <div className="text-center">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-primary/10 text-primary shadow-inner">
            <Mail className="h-8 w-8" />
          </div>
          <h2 className="mt-6 text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            Verifique o seu Email
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Enviámos um link de confirmação para{' '}
            <span className="font-semibold text-foreground">{email || 'o seu email'}</span>.
          </p>
        </div>

        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8 space-y-6">
          {message && (
            <div
              className={`flex items-start gap-3 rounded-xl border p-4 text-sm font-medium animate-fade-in ${
                message.type === 'success'
                  ? 'border-emerald-500/20 bg-emerald-500/10 text-emerald-600 dark:text-emerald-400'
                  : 'border-destructive/20 bg-destructive/10 text-destructive'
              }`}
            >
              {message.type === 'success' ? (
                <CheckCircle2 className="h-5 w-5 shrink-0 mt-0.5" />
              ) : (
                <AlertCircle className="h-5 w-5 shrink-0 mt-0.5" />
              )}
              <div>{message.text}</div>
            </div>
          )}

          <div className="rounded-xl border border-border bg-muted/40 p-4 text-xs text-muted-foreground space-y-2">
            <p className="font-semibold text-foreground">Não recebeu o email?</p>
            <ul className="list-disc pl-4 space-y-1">
              <li>Verifique a sua pasta de Spam ou Lixo Eletrónico.</li>
              <li>Aguarde alguns minutos pela entrega do email.</li>
            </ul>
          </div>

          {import.meta.env.DEV && (
            <a
              href="http://localhost:8025"
              target="_blank"
              rel="noreferrer"
              className="inline-flex w-full items-center justify-center gap-2 rounded-xl border border-border bg-background py-2.5 text-xs font-medium text-foreground hover:bg-muted transition-colors"
            >
              <span>Abrir Caixa de Correio (Mailpit - Dev Mode)</span>
              <ExternalLink className="h-3.5 w-3.5" />
            </a>
          )}

          <Button
            type="button"
            onClick={handleResend}
            disabled={isLoading || cooldown > 0}
            className="w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5"
          >
            <RefreshCw className={`mr-2 h-4 w-4 ${isLoading ? 'animate-spin' : ''}`} />
            {cooldown > 0 ? `Aguarde ${cooldown}s para reenviar` : 'Reenviar Email de Confirmação'}
          </Button>

          <div className="pt-2 text-center text-xs text-muted-foreground">
            Já confirmou o email?{' '}
            <Link to="/login" className="font-semibold text-primary hover:underline">
              Iniciar Sessão
            </Link>
          </div>
        </div>
      </div>
    </div>
  )
}
