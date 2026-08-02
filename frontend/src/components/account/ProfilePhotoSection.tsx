import type { RefObject, ChangeEvent } from 'react'
import { Button } from '@/components/ui/button'
import { User, Briefcase, Camera, Loader2 } from 'lucide-react'
import type { components } from '@/generated/apiClient'

type UserProfileResponse = components['schemas']['UserProfileResponse']

interface ProfilePhotoSectionProps {
  profile: UserProfileResponse
  displayPictureUrl: string | null
  initials: string
  isUploading: boolean
  isSaving: boolean
  fileInputRef: RefObject<HTMLInputElement | null>
  onPhotoUpload: (e: ChangeEvent<HTMLInputElement>) => void
  onTriggerUpload: () => void
}

export function ProfilePhotoSection({
  profile,
  displayPictureUrl,
  initials,
  isUploading,
  isSaving,
  fileInputRef,
  onPhotoUpload,
  onTriggerUpload,
}: ProfilePhotoSectionProps) {
  return (
    <div className="flex flex-col items-center">
      <div className="w-full rounded-xl border border-border bg-card p-6 shadow-xs text-center">
        <h3 className="text-sm font-semibold text-foreground mb-4">Foto de Perfil</h3>

        <div className="relative mx-auto h-32 w-32 group">
          {displayPictureUrl ? (
            <img
              src={displayPictureUrl}
              alt="Avatar"
              className="h-32 w-32 rounded-full object-cover border border-border shadow-xs"
            />
          ) : (
            <div className="flex h-32 w-32 items-center justify-center rounded-full bg-primary/10 text-primary border border-primary/20 text-3xl font-bold font-heading shadow-xs">
              {initials || <User className="h-12 w-12" />}
            </div>
          )}

          {/* Overlay camera trigger */}
          <button
            type="button"
            onClick={onTriggerUpload}
            disabled={isUploading || isSaving}
            className="absolute inset-0 flex items-center justify-center rounded-full bg-black/40 text-white opacity-0 group-hover:opacity-100 transition-opacity duration-200 cursor-pointer disabled:opacity-0 disabled:pointer-events-none"
          >
            <Camera className="h-6 w-6" />
          </button>

          {/* Uploading Spinner */}
          {isUploading && (
            <div className="absolute inset-0 flex items-center justify-center rounded-full bg-background/80 text-primary">
              <Loader2 className="h-8 w-8 animate-spin" />
            </div>
          )}
        </div>

        <input
          type="file"
          ref={fileInputRef}
          onChange={onPhotoUpload}
          accept="image/*"
          className="hidden"
        />

        <Button
          variant="outline"
          size="sm"
          onClick={onTriggerUpload}
          disabled={isUploading || isSaving}
          className="mt-6 w-full cursor-pointer"
        >
          {isUploading ? 'A enviar...' : 'Alterar Foto'}
        </Button>
        <p className="mt-2 text-xs text-muted-foreground">
          Formatos recomendados: JPG, PNG ou WEBP. Imagens quadradas funcionam melhor.
        </p>
      </div>

      {/* Account Role Badge */}
      <div className="mt-4 w-full rounded-xl border border-border bg-card px-6 py-4 shadow-xs flex items-center justify-between">
        <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">
          Tipo de Conta
        </span>
        <div className="flex items-center gap-1.5">
          {profile.isProfessional ? (
            <span className="inline-flex items-center rounded-full bg-primary/10 px-2.5 py-0.5 text-xs font-semibold text-primary">
              <Briefcase className="mr-1 h-3.5 w-3.5" />
              Profissional
            </span>
          ) : (
            <span className="inline-flex items-center rounded-full bg-secondary px-2.5 py-0.5 text-xs font-semibold text-secondary-foreground">
              <User className="mr-1 h-3.5 w-3.5" />
              Cliente
            </span>
          )}
        </div>
      </div>
    </div>
  )
}
