import { Button } from '@/components/ui/button'
import { CheckCircle, Loader2 } from 'lucide-react'
import type { components } from '@/generated/apiClient'

type UserProfileResponse = components['schemas']['UserProfileResponse']

interface ProfessionalAccountSectionProps {
  profile: UserProfileResponse
  isUpgrading: boolean
  onUpgrade: () => void
}

export function ProfessionalAccountSection({
  profile,
  isUpgrading,
  onUpgrade,
}: ProfessionalAccountSectionProps) {
  return (
    <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
      <h3 className="text-lg font-bold text-foreground border-b border-border pb-3 mb-5">
        Conta Profissional
      </h3>

      {profile.isProfessional ? (
        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 bg-primary/5 rounded-lg border border-primary/10 p-5">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary">
            <CheckCircle className="h-6 w-6" />
          </div>
          <div>
            <h4 className="text-sm font-semibold text-foreground">A sua conta profissional está ativa</h4>
            <p className="mt-1 text-xs text-muted-foreground max-w-lg">
              Agora já pode ser associado a salões ou lojas, gerir o seu horário de trabalho e organizar
              as suas marcações de serviços no OnTime.
            </p>
          </div>
        </div>
      ) : (
        <div className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Quer prestar serviços na nossa plataforma? Ao atualizar para uma conta profissional, poderá
            gerir a sua própria agenda, receber marcações de clientes e definir os seus serviços.
          </p>

          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 text-xs text-muted-foreground pb-2">
            <div className="flex items-center gap-2">
              <CheckCircle className="h-4 w-4 text-primary shrink-0" />
              <span>Criação e Gestão de Serviços</span>
            </div>
            <div className="flex items-center gap-2">
              <CheckCircle className="h-4 w-4 text-primary shrink-0" />
              <span>Controlo de Agenda e Horários</span>
            </div>
            <div className="flex items-center gap-2">
              <CheckCircle className="h-4 w-4 text-primary shrink-0" />
              <span>Gestão de Calendário e Férias</span>
            </div>
            <div className="flex items-center gap-2">
              <CheckCircle className="h-4 w-4 text-primary shrink-0" />
              <span>Reserva online direta por clientes</span>
            </div>
          </div>

          <div className="pt-2 border-t border-border flex justify-end">
            <Button
              type="button"
              onClick={onUpgrade}
              disabled={isUpgrading}
              className="cursor-pointer font-semibold shadow-xs bg-primary hover:bg-primary/90 text-primary-foreground"
            >
              {isUpgrading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  A ativar...
                </>
              ) : (
                'Ativar Conta Profissional'
              )}
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
