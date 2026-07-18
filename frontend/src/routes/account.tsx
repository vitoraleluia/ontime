import { useState, useEffect, useRef } from 'react'
import { createFileRoute } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { useMutation } from '@tanstack/react-query'
import { LocalStoreKeys } from '@/domain/constants/localStoreKeys'
import type { StoredTokens } from '@/domain/auth'
import { 
  User, 
  Phone, 
  Mail, 
  Briefcase, 
  CheckCircle, 
  Loader2, 
  Camera,
  AlertCircle
} from 'lucide-react'

export const Route = createFileRoute('/account')({
  component: AccountPage,
})

function AccountPage() {
  const { isAuthenticated, isLoading: isAuthLoading, login } = useAuth()
  
  // React Query - Profile fetching (types fully inferred)
  const { data: profile, isLoading: isProfileLoading, refetch } = $api.useQuery(
    'get',
    '/api/Account',
    {},
    {
      enabled: isAuthenticated
    }
  )

  // Minimal state: only holds the newly uploaded image ID and preview URL override
  const [profilePictureId, setProfilePictureId] = useState<string | null>(null)
  const [tempPictureUrl, setTempPictureUrl] = useState<string | null>(null)

  const [errorMsg, setErrorMsg] = useState<string | null>(null)
  const [successMsg, setSuccessMsg] = useState<string | null>(null)
  
  const fileInputRef = useRef<HTMLInputElement>(null)

  // Redirect if not authenticated
  useEffect(() => {
    if (!isAuthLoading && !isAuthenticated) {
      login('/account')
    }
  }, [isAuthLoading, isAuthenticated, login])

  // Mutations (Errors and Responses are fully typed)
  const updateProfileMutation = $api.useMutation('put', '/api/Account', {
    onSuccess: () => {
      setSuccessMsg('Perfil atualizado com sucesso!')
      setProfilePictureId(null) // Reset temporary ID
      setTempPictureUrl(null)   // Clear preview override
      refetch()
    },
    onError: (err) => {
      let msg = 'Falha ao atualizar dados.'
      if (err) {
        if (err.errors) {
          const messages = Object.values(err.errors).flat()
          if (messages.length > 0) {
            msg = messages.join(' ')
          }
        } else if (err.detail) {
          msg = err.detail
        } else if (err.title) {
          msg = err.title
        }
      }
      setErrorMsg(msg)
    }
  })

  const uploadPhotoMutation = useMutation({
    mutationFn: async (file: File) => {
      const tokensStr = localStorage.getItem(LocalStoreKeys.AuthTokens)
      if (!tokensStr) throw new Error('Sessão expirada. Inicie sessão novamente.')
      const tokens = JSON.parse(tokensStr) as StoredTokens

      const formData = new FormData()
      formData.append('file', file)

      const response = await fetch('/api/Images?format=Square', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${tokens.token}`
        },
        body: formData
      })

      if (!response.ok) {
        const errText = await response.text()
        throw new Error(errText || 'Falha ao enviar imagem.')
      }

      const result: { id: string } = await response.json()
      return result
    },
    onSuccess: (data) => {
      if (data && data.id) {
        setProfilePictureId(data.id)
        setSuccessMsg('Foto carregada com sucesso! Clique em "Guardar Alterações" para aplicar.')
      }
    },
    onError: () => {
      setErrorMsg('Erro ao fazer upload da foto de perfil.')
    }
  })

  const assignProfessionalMutation = $api.useMutation('post', '/api/Account/assign-professional', {
    onSuccess: () => {
      setSuccessMsg('Parabéns! A sua conta profissional foi ativada com sucesso.')
      refetch()
    },
    onError: () => {
      setErrorMsg('Erro ao ativar conta profissional.')
    }
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

    // Show temporary local preview
    const localUrl = URL.createObjectURL(file)
    setTempPictureUrl(localUrl)

    uploadPhotoMutation.mutate(file)
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
        profilePictureId: profilePictureId || undefined
      }
    })
  }

  const handleUpgradeAccount = () => {
    if (profile?.role === 1) return

    setErrorMsg(null)
    setSuccessMsg(null)

    assignProfessionalMutation.mutate({})
  }

  // Simplify pending loading flags
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
          Por favor, inicie sessão com a sua conta Keycloak para aceder às configurações do seu perfil.
        </p>
        <Button onClick={() => login('/account')} className="mt-2 font-semibold">
          Iniciar Sessão
        </Button>
      </div>
    )
  }

  // Declarative derived UI states
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
                onClick={() => fileInputRef.current?.click()}
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
              onChange={handlePhotoUpload} 
              accept="image/*" 
              className="hidden" 
            />

            <Button
              variant="outline"
              size="sm"
              onClick={() => fileInputRef.current?.click()}
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
            <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Tipo de Conta</span>
            <div className="flex items-center gap-1.5">
              {profile.role === 1 ? (
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

        {/* Right Column: Personal details form & professional account upgrade */}
        <div className="md:col-span-2 space-y-6">
          
          {/* Details Form Card */}
          <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
            <h3 className="text-lg font-bold text-foreground border-b border-border pb-3 mb-5">
              Dados Pessoais
            </h3>

            <form onSubmit={handleSaveProfile} className="space-y-4">
              
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
                <p className="mt-1.5 text-xs text-muted-foreground">
                  O email é gerido pela sua conta de autenticação Keycloak e não pode ser editado.
                </p>
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

          {/* Professional Account Upgrade Section */}
          <div className="rounded-xl border border-border bg-card p-6 shadow-xs">
            <h3 className="text-lg font-bold text-foreground border-b border-border pb-3 mb-5">
              Conta Profissional
            </h3>

            {profile.role === 1 ? (
              <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 bg-primary/5 rounded-lg border border-primary/10 p-5">
                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <CheckCircle className="h-6 w-6" />
                </div>
                <div>
                  <h4 className="text-sm font-semibold text-foreground">A sua conta profissional está ativa</h4>
                  <p className="mt-1 text-xs text-muted-foreground max-w-lg">
                    Agora já pode ser associado a salões ou lojas, gerir o seu horário de trabalho e organizar as suas marcações de serviços no OnTime.
                  </p>
                </div>
              </div>
            ) : (
              <div className="space-y-4">
                <p className="text-sm text-muted-foreground">
                  Quer prestar serviços na nossa plataforma? Ao atualizar para uma conta profissional, poderá gerir a sua própria agenda, receber marcações de clientes e definir os seus serviços.
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
                    onClick={handleUpgradeAccount}
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

        </div>

      </div>
    </div>
  )
}
