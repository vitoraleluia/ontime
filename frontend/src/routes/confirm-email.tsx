import { useState, useEffect } from 'react'
import { createFileRoute, useSearch, Link } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { Button } from '@/components/ui/button'
import { CheckCircle2, AlertCircle, Loader2 } from 'lucide-react'

export const Route = createFileRoute('/confirm-email')({
  component: ConfirmEmailPage,
})

function ConfirmEmailPage() {
  const search = useSearch({ from: '/confirm-email' }) as { userId?: string; token?: string }
  const userId = search?.userId ?? ''
  const token = search?.token ?? ''

  const { confirmEmail } = useAuth()

  const [isLoading, setIsLoading] = useState(true)
  const [isSuccess, setIsSuccess] = useState(false)
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    async function executeConfirmation() {
      if (!userId || !token) {
        if (isMounted) {
          setIsLoading(false)
          setErrorMsg('Parâmetros de confirmação de email inválidos ou em falta.')
        }
        return
      }

      const result = await confirmEmail(userId, token)

      if (isMounted) {
        setIsLoading(false)
        if (result.success) {
          setIsSuccess(true)
        } else {
          setErrorMsg(result.error ?? 'Falha ao confirmar o endereço de email.')
        }
      }
    }

    executeConfirmation()

    return () => {
      isMounted = false
    }
  }, [userId, token, confirmEmail])

  return (
    <div className="flex min-h-[80vh] flex-col items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
      <div className="w-full max-w-md space-y-8">
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8 text-center space-y-6">
          {isLoading && (
            <div className="py-8 space-y-4">
              <Loader2 className="mx-auto h-12 w-12 animate-spin text-primary" />
              <h2 className="text-xl font-bold text-foreground">A confirmar o seu email...</h2>
              <p className="text-sm text-muted-foreground">Aguarde um momento enquanto validamos a sua conta.</p>
            </div>
          )}

          {!isLoading && isSuccess && (
            <div className="py-6 space-y-6 animate-fade-in">
              <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-emerald-500/10 text-emerald-500 shadow-inner">
                <CheckCircle2 className="h-10 w-10" />
              </div>
              <div className="space-y-2">
                <h2 className="text-2xl font-bold text-foreground">Email Confirmado!</h2>
                <p className="text-sm text-muted-foreground">
                  A sua conta foi ativada com sucesso. Já pode iniciar sessão e utilizar a plataforma OnTime.
                </p>
              </div>
              <Link to="/login" className="block w-full">
                <Button className="w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5">
                  Iniciar Sessão
                </Button>
              </Link>
            </div>
          )}

          {!isLoading && !isSuccess && (
            <div className="py-6 space-y-6 animate-fade-in">
              <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-destructive/10 text-destructive shadow-inner">
                <AlertCircle className="h-10 w-10" />
              </div>
              <div className="space-y-2">
                <h2 className="text-2xl font-bold text-foreground">Falha na Confirmação</h2>
                <p className="text-sm text-destructive font-medium">{errorMsg}</p>
              </div>
              <Link to="/confirm-email-pending" className="block w-full">
                <Button variant="outline" className="w-full cursor-pointer rounded-xl font-semibold shadow-xs py-2.5">
                  Solicitar Novo Email de Confirmação
                </Button>
              </Link>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
