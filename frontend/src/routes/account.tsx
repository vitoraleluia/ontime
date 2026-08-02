import { useState, useEffect, useRef } from 'react'
import { createFileRoute, useNavigate, Link } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'
import { ProfilePhotoSection } from '@/components/account/ProfilePhotoSection'
import { PersonalDetailsForm } from '@/components/account/PersonalDetailsForm'
import { ProfessionalAccountSection } from '@/components/account/ProfessionalAccountSection'
import { CheckCircle, Loader2, AlertCircle } from 'lucide-react'

export const Route = createFileRoute('/account')({
  component: AccountPage,
})

function AccountPage() {
  const { isAuthenticated, isLoading: isAuthLoading } = useAuth()
  const navigate = useNavigate()

  // React Query - Profile fetching
  const { data: profile, isLoading: isProfileLoading, refetch } = $api.useQuery(
    'get',
    '/api/Account',
    {},
    { enabled: isAuthenticated }
  )

  const [profilePictureId, setProfilePictureId] = useState<string | null>(null)
  const [tempPictureUrl, setTempPictureUrl] = useState<string | null>(null)

  const [errorMsg, setErrorMsg] = useState<string | null>(null)
  const [successMsg, setSuccessMsg] = useState<string | null>(null)

  const fileInputRef = useRef<HTMLInputElement>(null)

  // Redirect if not authenticated
  useEffect(() => {
    if (!isAuthLoading && !isAuthenticated) {
      navigate({ to: '/login', search: { returnUrl: '/account' } })
    }
  }, [isAuthLoading, isAuthenticated, navigate])

  // Mutations
  const updateProfileMutation = $api.useMutation('put', '/api/Account', {
    onSuccess: () => {
      setSuccessMsg('Perfil atualizado com sucesso!')
      setProfilePictureId(null)
      setTempPictureUrl(null)
      refetch()
    },
    onError: (err: unknown) => {
      setErrorMsg(ErrorUtils.extractMessage(err, 'Falha ao atualizar dados.'))
    },
  })

  const uploadPhotoMutation = $api.useMutation('post', '/api/Images', {
    onSuccess: (data) => {
      if (data && data.id) {
        setProfilePictureId(data.id)
        setSuccessMsg('Foto carregada com sucesso! Clique em "Guardar Alterações" para aplicar.')
      }
    },
    onError: () => {
      setErrorMsg('Erro ao fazer upload da foto de perfil.')
    },
  })

  const assignProfessionalMutation = $api.useMutation('post', '/api/Account/assign-professional', {
    onSuccess: () => {
      setSuccessMsg('Parabéns! A sua conta profissional foi ativada com sucesso.')
      refetch()
    },
    onError: () => {
      setErrorMsg('Erro ao ativar conta profissional.')
    },
  })

  // Handlers
  const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

    if (!file.type.startsWith('image/')) {
      setErrorMsg('Por favor, selecione um ficheiro de imagem válido.')
      return
    }

    setErrorMsg(null)
    setSuccessMsg(null)

    const localUrl = URL.createObjectURL(file)
    setTempPictureUrl(localUrl)

    uploadPhotoMutation.mutate({
      params: { query: { format: 0 } },
      body: { file: file as unknown as string },
    })
  }

  const handleSaveProfile = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const formFirstName = (formData.get('firstName') as string)?.trim()
    const formLastName = (formData.get('lastName') as string)?.trim()
    const formPhoneNumber = (formData.get('phoneNumber') as string)?.trim()

    if (!formFirstName || !formLastName) {
      setErrorMsg('O nome e apelido são obrigatórios.')
      return
    }

    setErrorMsg(null)
    setSuccessMsg(null)

    updateProfileMutation.mutate({
      body: {
        firstName: formFirstName,
        lastName: formLastName,
        phoneNumber: formPhoneNumber || null,
        profilePictureId: profilePictureId || undefined,
      },
    })
  }

  const handleUpgradeAccount = () => {
    if (profile?.isProfessional) return

    setErrorMsg(null)
    setSuccessMsg(null)

    assignProfessionalMutation.mutate({})
  }

  const isSaving = updateProfileMutation.isPending
  const isUploading = uploadPhotoMutation.isPending
  const isUpgrading = assignProfessionalMutation.isPending
  const isLoading = isProfileLoading

  if (isAuthLoading || isLoading) {
    return (
      <div className="flex min-h-[70vh] flex-col items-center justify-center gap-4">
        <Loader2 className="h-10 w-10 animate-spin text-primary" />
        <p className="text-muted-foreground text-sm font-medium">A carregar dados da conta...</p>
      </div>
    )
  }

  if (!isAuthenticated || !profile) {
    return (
      <div className="flex min-h-[70vh] flex-col items-center justify-center gap-4 px-4 text-center">
        <AlertCircle className="h-12 w-12 text-muted-foreground" />
        <h2 className="text-xl font-bold">Acesso Restrito</h2>
        <p className="text-muted-foreground max-w-md text-sm">
          Por favor, inicie sessão para aceder às configurações do seu perfil.
        </p>
        <Link to="/login" search={{ returnUrl: '/account' }}>
          <Button className="mt-2 font-semibold">Iniciar Sessão</Button>
        </Link>
      </div>
    )
  }

  const displayPictureUrl = tempPictureUrl || profile.profilePictureUrl || null
  const initials = `${profile.firstName?.charAt(0) || ''}${profile.lastName?.charAt(0) || ''}`.toUpperCase()

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6 lg:px-8">
      {/* Header */}
      <div className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
            A Minha Conta
          </h1>
          <p className="mt-2 text-sm text-muted-foreground">
            Altere os seus dados pessoais, foto de perfil e tipo de conta.
          </p>
        </div>
      </div>

      {/* Messages */}
      {errorMsg && (
        <div className="mb-6 flex items-start gap-3 rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive animate-fade-in">
          <AlertCircle className="h-5 w-5 shrink-0 mt-0.5" />
          <div className="text-sm font-medium">{errorMsg}</div>
        </div>
      )}
      {successMsg && (
        <div className="mb-6 flex items-start gap-3 rounded-lg border border-emerald-500/20 bg-emerald-500/10 p-4 text-emerald-600 dark:text-emerald-400 animate-fade-in">
          <CheckCircle className="h-5 w-5 shrink-0 mt-0.5" />
          <div className="text-sm font-medium">{successMsg}</div>
        </div>
      )}

      <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
        {/* Left Column: Photo upload */}
        <ProfilePhotoSection
          profile={profile}
          displayPictureUrl={displayPictureUrl}
          initials={initials}
          isUploading={isUploading}
          isSaving={isSaving}
          fileInputRef={fileInputRef}
          onPhotoUpload={handlePhotoUpload}
          onTriggerUpload={() => fileInputRef.current?.click()}
        />

        {/* Right Column: Personal details form & professional account upgrade */}
        <div className="md:col-span-2 space-y-6">
          <PersonalDetailsForm
            profile={profile}
            isSaving={isSaving}
            isUploading={isUploading}
            onSubmit={handleSaveProfile}
          />

          <ProfessionalAccountSection
            profile={profile}
            isUpgrading={isUpgrading}
            onUpgrade={handleUpgradeAccount}
          />
        </div>
      </div>
    </div>
  )
}
