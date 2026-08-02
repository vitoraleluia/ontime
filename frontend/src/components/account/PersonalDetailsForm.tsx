import type { FormEvent } from 'react'
import { Button } from '@/components/ui/button'
import { Mail, Phone, Loader2 } from 'lucide-react'
import type { components } from '@/generated/apiClient'

type UserProfileResponse = components['schemas']['UserProfileResponse']

interface PersonalDetailsFormProps {
  profile: UserProfileResponse
  isSaving: boolean
  isUploading: boolean
  onSubmit: (e: FormEvent<HTMLFormElement>) => void
}

export function PersonalDetailsForm({
  profile,
  isSaving,
  isUploading,
  onSubmit,
}: PersonalDetailsFormProps) {
  return (
    <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
      <h3 className="text-lg font-bold text-foreground border-b border-border pb-3 mb-5">
        Dados Pessoais
      </h3>

      <form onSubmit={onSubmit} className="space-y-4">
        {/* Email (Read-only) */}
        <div>
          <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
            Endereço de Email
          </label>
          <div className="relative">
            <Mail className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
            <input
              type="email"
              value={profile.email || ''}
              disabled
              className="w-full rounded-lg border border-border bg-muted/50 py-2 pl-10 pr-4 text-sm text-muted-foreground focus:outline-none cursor-not-allowed"
            />
          </div>
        </div>

        {/* First Name & Last Name */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
              Nome
            </label>
            <input
              type="text"
              name="firstName"
              defaultValue={profile.firstName || ''}
              placeholder="Introduza o seu nome"
              required
              disabled={isSaving}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
            />
          </div>
          <div>
            <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
              Apelido
            </label>
            <input
              type="text"
              name="lastName"
              defaultValue={profile.lastName || ''}
              placeholder="Introduza o seu apelido"
              required
              disabled={isSaving}
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
            />
          </div>
        </div>

        {/* Phone Number */}
        <div>
          <label className="block text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-1.5">
            Número de Telemóvel
          </label>
          <div className="relative">
            <Phone className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
            <input
              type="tel"
              name="phoneNumber"
              defaultValue={profile.phoneNumber || ''}
              placeholder="Ex: 912345678"
              disabled={isSaving}
              className="w-full rounded-lg border border-border bg-background py-2 pl-10 pr-4 text-sm placeholder-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary disabled:opacity-50"
            />
          </div>
        </div>

        {/* Submit */}
        <div className="pt-2 flex justify-end">
          <Button
            type="submit"
            disabled={isSaving || isUploading}
            className="cursor-pointer font-semibold shadow-xs"
          >
            {isSaving ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                A guardar...
              </>
            ) : (
              'Guardar Alterações'
            )}
          </Button>
        </div>
      </form>
    </div>
  )
}
