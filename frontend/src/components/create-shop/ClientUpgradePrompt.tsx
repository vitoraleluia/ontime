import { useState } from 'react'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { ShieldAlert, Loader2, Sparkles } from 'lucide-react'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'

interface ClientUpgradePromptProps {
  refetchProfile: () => void
}

export function ClientUpgradePrompt({ refetchProfile }: ClientUpgradePromptProps) {
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const upgradeMutation = $api.useMutation('post', '/api/Account/assign-professional', {
    onSuccess: () => {
      refetchProfile()
    },
    onError: (err: unknown) => {
      setErrorMsg(ErrorUtils.extractMessage(err, 'Falha ao atualizar conta para profissional.'))
    },
  })

  return (
    <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6 lg:px-8">
      <div className="rounded-2xl border border-primary/20 bg-card p-8 shadow-xl backdrop-blur-sm">
        <div className="flex flex-col items-center text-center">
          <div className="mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-primary/10 text-primary">
            <ShieldAlert className="h-8 w-8" />
          </div>

          <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            Apenas Profissionais Podem Criar Lojas
          </h1>

          <p className="mt-3 max-w-xl text-base text-muted-foreground">
            Para criar e gerir a sua própria barbearia ou estabelecimento na OnTime, precisa de ter uma conta de profissional ativada.
          </p>

          {errorMsg && (
            <div className="mt-4 rounded-lg bg-destructive/10 p-3 text-sm text-destructive">
              {errorMsg}
            </div>
          )}

          <div className="mt-8 flex flex-col gap-3 sm:flex-row">
            <Button
              size="lg"
              onClick={() => upgradeMutation.mutate({})}
              disabled={upgradeMutation.isPending}
              className="cursor-pointer gap-2 font-semibold shadow-md"
            >
              {upgradeMutation.isPending ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  A atualizar conta...
                </>
              ) : (
                <>
                  <Sparkles className="h-4 w-4" />
                  Tornar-me Profissional Agora
                </>
              )}
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
