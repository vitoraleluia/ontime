import { useState, useEffect, useRef } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { useMutation } from '@tanstack/react-query'
import { LocalStoreKeys } from '@/domain/constants/localStoreKeys'
import type { StoredTokens } from '@/domain/auth'
import {
  Store,
  Building2,
  MapPin,
  Clock,
  CheckCircle2,
  XCircle,
  Loader2,
  Image as ImageIcon,
  Sparkles,
  ShieldAlert,
  ArrowRight
} from 'lucide-react'

export const Route = createFileRoute('/create-shop')({
  component: CreateShopPage,
})

function SlugStatusIcon({ isChecking, checkResult }: { isChecking: boolean; checkResult?: { isAvailable: boolean } | null }) {
  if (isChecking) {
    return <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
  }
  if (!checkResult) {
    return null
  }
  if (checkResult.isAvailable) {
    return <CheckCircle2 className="h-4 w-4 text-emerald-500" />
  }
  return <XCircle className="h-4 w-4 text-destructive" />
}

function buildFriendlyUrl(text: string, maxLength = 50): string {
  if (!text) return ''
  let clean = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase()
  clean = clean.replace(/[^a-z0-9\s-]/g, '')
  clean = clean.replace(/[\s-]+/g, '-').replace(/^-+|-+$/g, '')
  return clean.slice(0, maxLength).replace(/-+$/g, '')
}

function CreateShopPage() {
  const { isAuthenticated, isLoading: isAuthLoading, login } = useAuth()
  const navigate = useNavigate()

  const { data: profile, isLoading: isProfileLoading, refetch } = $api.useQuery(
    'get',
    '/api/Account',
    {},
    { enabled: isAuthenticated }
  )

  useEffect(() => {
    if (!isAuthLoading && !isAuthenticated) {
      login('/create-shop')
    }
  }, [isAuthLoading, isAuthenticated, login])

  if (isAuthLoading || isProfileLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  const isProfessional = profile?.role === 1 // UserRole.Professional = 1

  if (!isProfessional) {
    return <ClientUpgradePrompt refetchProfile={refetch} />
  }

  return <ShopCreationForm navigate={navigate} />
}

function ClientUpgradePrompt({ refetchProfile }: { refetchProfile: () => void }) {
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const upgradeMutation = $api.useMutation('post', '/api/Account/assign-professional', {
    onSuccess: () => {
      refetchProfile()
    },
    onError: (err) => {
      setErrorMsg(err?.detail || err?.title || 'Falha ao atualizar conta para profissional.')
    }
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

function ShopCreationForm({ navigate }: { navigate: ReturnType<typeof useNavigate> }) {
  // Reactive states for live slug auto-generation & availability badges
  const [name, setName] = useState('')
  const [slug, setSlug] = useState('')
  const [isCustomSlug, setIsCustomSlug] = useState(false)
  const [imageId, setImageId] = useState<string | null>(null)
  const [imagePreviewUrl, setImagePreviewUrl] = useState<string | null>(null)

  // useRef hooks for static form fields (zero re-renders when typing)
  const descriptionRef = useRef<HTMLTextAreaElement>(null)
  const addressRef = useRef<HTMLInputElement>(null)
  const phoneNumberRef = useRef<HTMLInputElement>(null)
  const slotDurationRef = useRef<HTMLSelectElement>(null)
  const allowCancellationRef = useRef<HTMLSelectElement>(null)
  const cancellationDeadlineRef = useRef<HTMLInputElement>(null)

  const [debouncedSlug, setDebouncedSlug] = useState('')
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  // Auto-generate slug when name changes unless custom slug was modified
  const handleNameChange = (val: string) => {
    setName(val)
    if (!isCustomSlug) {
      setSlug(buildFriendlyUrl(val))
    }
  }

  const handleSlugChange = (val: string) => {
    setIsCustomSlug(true)
    setSlug(buildFriendlyUrl(val))
  }

  // Debounce slug for real-time validation check
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSlug(slug)
    }, 300)
    return () => clearTimeout(timer)
  }, [slug])

  // Real-time slug availability query
  const { data: slugCheck, isLoading: isCheckingSlug } = $api.useQuery(
    'get',
    '/api/Shops/check-slug',
    {
      params: { query: { slug: debouncedSlug } }
    },
    {
      enabled: Boolean(debouncedSlug && debouncedSlug.length > 0)
    }
  )

  // Upload image mutation
  const uploadPhotoMutation = useMutation({
    mutationFn: async (file: File) => {
      const tokensStr = localStorage.getItem(LocalStoreKeys.AuthTokens)
      if (!tokensStr) throw new Error('Sessão expirada. Inicie sessão novamente.')
      const tokens = JSON.parse(tokensStr) as StoredTokens

      const formData = new FormData()
      formData.append('file', file)

      const response = await fetch('/api/Images?format=Landscape', {
        method: 'POST',
        headers: { Authorization: `Bearer ${tokens.token}` },
        body: formData
      })

      if (!response.ok) {
        throw new Error('Falha no upload da imagem da loja.')
      }

      return await response.json()
    },
    onSuccess: (data: { id: string }) => {
      setImageId(data.id)
    }
  })

  // Create shop mutation
  const createShopMutation = $api.useMutation('post', '/api/Shops', {
    onSuccess: (shop) => {
      if (shop?.slug) {
        navigate({ to: '/shops/$slug', params: { slug: shop.slug } })
      } else {
        navigate({ to: '/' })
      }
    },
    onError: (err) => {
      let msg = 'Falha ao criar o estabelecimento.'
      if (err) {
        if (typeof err === 'string') {
          msg = err
        } else if (err.errors) {
          const messages = Object.values(err.errors).flat()
          if (messages.length > 0) msg = messages.join(' ')
        } else if (err.detail) {
          msg = err.detail
        } else if (err.title) {
          msg = err.title
        }
      }
      setErrorMsg(msg)
    }
  })

  const isSlugValid = !debouncedSlug || slugCheck?.isAvailable === true

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMsg(null)

    if (!name.trim()) {
      setErrorMsg('Por favor introduza o nome do estabelecimento.')
      return
    }

    if (!isSlugValid) {
      setErrorMsg('Por favor defina um slug único e disponível.')
      return
    }

    createShopMutation.mutate({
      body: {
        name: name.trim(),
        slug: slug.trim() || undefined,
        description: descriptionRef.current?.value.trim() || '',
        address: addressRef.current?.value.trim() || undefined,
        phoneNumber: phoneNumberRef.current?.value.trim() || undefined,
        slotDurationMinutes: Number(slotDurationRef.current?.value) || 30,
        allowCancellation: allowCancellationRef.current?.value === 'yes',
        cancellationDeadlineHours: Number(cancellationDeadlineRef.current?.value) || 24,
        imageId: imageId || undefined
      }
    })
  }

  const handleImageFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      setImagePreviewUrl(URL.createObjectURL(file))
      uploadPhotoMutation.mutate(file)
    }
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-10 sm:px-6 lg:px-8">
      <div className="mb-8">
        <div className="flex items-center gap-3">
          <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground shadow-md">
            <Store className="h-6 w-6" />
          </div>
          <div>
            <h1 className="font-heading text-3xl font-bold tracking-tight text-foreground">
              Criar Novo Estabelecimento
            </h1>
            <p className="text-sm text-muted-foreground">
              Configure a sua barbearia ou espaço para começar a receber agendamentos.
            </p>
          </div>
        </div>
      </div>

      {errorMsg && (
        <div className="mb-6 flex items-center gap-2 rounded-xl bg-destructive/10 p-4 text-sm font-medium text-destructive">
          <ShieldAlert className="h-5 w-5 shrink-0" />
          <span>{errorMsg}</span>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-8">
        {/* Basic Details */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-foreground flex items-center gap-2">
            <Building2 className="h-5 w-5 text-primary" />
            Informações Gerais
          </h2>

          <div className="space-y-5">
            <div>
              <label className="block text-sm font-medium text-foreground">
                Nome do Estabelecimento <span className="text-destructive">*</span>
              </label>
              <input
                type="text"
                required
                maxLength={100}
                placeholder="Ex: Barbearia Martinéz"
                value={name}
                onChange={(e) => handleNameChange(e.target.value)}
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>

            {/* Slug Field with Real-Time Indicator */}
            <div>
              <label className="block text-sm font-medium text-foreground">
                URL / Slug Personalizado <span className="text-destructive">*</span> (Máx 50 caracteres)
              </label>
              <div className="relative mt-1.5">
                <input
                  type="text"
                  required
                  maxLength={50}
                  placeholder="barbearia-martinez"
                  value={slug}
                  onChange={(e) => handleSlugChange(e.target.value)}
                  className="w-full rounded-lg border border-input bg-background px-3.5 py-2 pr-10 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none font-mono"
                />
                <div className="absolute inset-y-0 right-0 flex items-center pr-3">
                  <SlugStatusIcon isChecking={isCheckingSlug} checkResult={slugCheck} />
                </div>
              </div>

              {/* Slug status message */}
              <div className="mt-1.5 flex items-center justify-between text-xs">
                <span className="text-muted-foreground font-mono">
                  Link: ontime.pt/shops/{slug || 'slug'}
                </span>
                {slugCheck && (
                  <span
                    className={
                      slugCheck.isAvailable
                        ? 'font-medium text-emerald-600 dark:text-emerald-400'
                        : 'font-medium text-destructive'
                    }
                  >
                    {slugCheck.message}
                  </span>
                )}
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground">Descrição</label>
              <textarea
                ref={descriptionRef}
                rows={3}
                maxLength={500}
                placeholder="Descreva os serviços, atmosfera e diferenciais do seu espaço..."
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>
          </div>
        </div>

        {/* Location & Contact */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-foreground flex items-center gap-2">
            <MapPin className="h-5 w-5 text-primary" />
            Localização e Contacto
          </h2>

          <div className="grid gap-5 sm:grid-cols-2">
            <div>
              <label className="block text-sm font-medium text-foreground">Endereço</label>
              <input
                ref={addressRef}
                type="text"
                maxLength={200}
                placeholder="Ex: Rua Augusta 123, Lisboa"
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground">Telefone de Contacto</label>
              <input
                ref={phoneNumberRef}
                type="tel"
                maxLength={20}
                placeholder="Ex: +351 912 345 678"
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>
          </div>
        </div>

        {/* Schedule & Cancellation Rules */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-foreground flex items-center gap-2">
            <Clock className="h-5 w-5 text-primary" />
            Regras de Agendamento
          </h2>

          <div className="grid gap-5 sm:grid-cols-3">
            <div>
              <label className="block text-sm font-medium text-foreground">
                Duração Padrão do Slot (Minutos)
              </label>
              <select
                ref={slotDurationRef}
                defaultValue={30}
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              >
                <option value={15}>15 minutos</option>
                <option value={30}>30 minutos</option>
                <option value={45}>45 minutos</option>
                <option value={60}>60 minutos</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground">Permitir Cancelamento</label>
              <select
                ref={allowCancellationRef}
                defaultValue="yes"
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              >
                <option value="yes">Sim, permitir cancelamento</option>
                <option value="no">Não permitir</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground">
                Limite para Cancelar (Horas antes)
              </label>
              <input
                ref={cancellationDeadlineRef}
                type="number"
                min={0}
                max={168}
                defaultValue={24}
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>
          </div>
        </div>

        {/* Cover Image Upload */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-foreground flex items-center gap-2">
            <ImageIcon className="h-5 w-5 text-primary" />
            Imagem de Capa do Estabelecimento
          </h2>

          <div className="flex flex-col items-center justify-center rounded-xl border-2 border-dashed border-border p-6 text-center hover:border-primary/50 transition-colors">
            {imagePreviewUrl ? (
              <div className="relative mb-4 overflow-hidden rounded-xl">
                <img src={imagePreviewUrl} alt="Capa da loja" className="h-48 w-full object-cover" />
                {uploadPhotoMutation.isPending && (
                  <div className="absolute inset-0 flex items-center justify-center bg-black/50 text-white">
                    <Loader2 className="h-6 w-6 animate-spin" />
                  </div>
                )}
              </div>
            ) : (
              <div className="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-muted text-muted-foreground">
                <ImageIcon className="h-6 w-6" />
              </div>
            )}

            <label className="cursor-pointer rounded-lg bg-secondary px-4 py-2 text-sm font-medium text-secondary-foreground hover:bg-secondary/80 transition-colors">
              <span>{imagePreviewUrl ? 'Alterar Imagem' : 'Carregar Imagem'}</span>
              <input
                type="file"
                accept="image/*"
                onChange={handleImageFileChange}
                className="hidden"
              />
            </label>
            <p className="mt-2 text-xs text-muted-foreground">PNG, JPG ou WEBP até 5MB.</p>
          </div>
        </div>

        {/* Submit */}
        <div className="flex justify-end gap-4">
          <Button
            type="submit"
            size="lg"
            disabled={createShopMutation.isPending || (Boolean(slugCheck) && !slugCheck?.isAvailable)}
            className="cursor-pointer gap-2 font-semibold shadow-md px-8"
          >
            {createShopMutation.isPending ? (
              <>
                <Loader2 className="h-5 w-5 animate-spin" />
                A criar estabelecimento...
              </>
            ) : (
              <>
                Criar Estabelecimento
                <ArrowRight className="h-5 w-5" />
              </>
            )}
          </Button>
        </div>
      </form>
    </div>
  )
}
