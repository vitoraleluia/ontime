import { useState, useEffect } from 'react'
import type { FormEvent, ChangeEvent } from 'react'
import type { useNavigate } from '@tanstack/react-router'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { StringUtils } from '@/domain/utils/StringUtils'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'
import { SlugStatusIcon } from '@/components/create-shop/SlugStatusIcon'
import {
  Store,
  Building2,
  MapPin,
  Clock,
  Loader2,
  Image as ImageIcon,
  ShieldAlert,
  ArrowRight,
} from 'lucide-react'

export class ShopCreationFormFields {
  public static readonly NAME = 'name'
  public static readonly SLUG = 'slug'
  public static readonly DESCRIPTION = 'description'
  public static readonly ADDRESS = 'address'
  public static readonly PHONE_NUMBER = 'phoneNumber'
  public static readonly SLOT_DURATION = 'slotDuration'
  public static readonly ALLOW_CANCELLATION = 'allowCancellation'
  public static readonly CANCELLATION_DEADLINE = 'cancellationDeadline'
}

interface ShopCreationFormProps {
  navigate: ReturnType<typeof useNavigate>
}

export function ShopCreationForm({ navigate }: ShopCreationFormProps) {
  const [name, setName] = useState('')
  const [slug, setSlug] = useState('')
  const [isCustomSlug, setIsCustomSlug] = useState(false)
  const [imageId, setImageId] = useState<string | null>(null)
  const [imagePreviewUrl, setImagePreviewUrl] = useState<string | null>(null)

  const [debouncedSlug, setDebouncedSlug] = useState('')
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  // Auto-generate slug when name changes unless custom slug was modified
  const handleNameChange = (val: string) => {
    setName(val)
    if (!isCustomSlug) {
      setSlug(StringUtils.buildFriendlyUrl(val))
    }
  }

  const handleSlugChange = (val: string) => {
    setIsCustomSlug(true)
    setSlug(StringUtils.buildFriendlyUrl(val))
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
      params: { query: { slug: debouncedSlug } },
    },
    {
      enabled: Boolean(debouncedSlug && debouncedSlug.length > 0),
    }
  )

  const uploadPhotoMutation = $api.useMutation('post', '/api/Images', {
    onSuccess: (data) => {
      if (data && data.id) {
        setImageId(data.id)
      }
    },
  })

  const createShopMutation = $api.useMutation('post', '/api/Shops', {
    onSuccess: (shop) => {
      if (shop?.slug) {
        navigate({ to: '/shops/$slug', params: { slug: shop.slug } })
      } else {
        navigate({ to: '/' })
      }
    },
    onError: (err: unknown) => {
      setErrorMsg(ErrorUtils.extractMessage(err, 'Falha ao criar o estabelecimento.'))
    },
  })

  const isFormValid = !debouncedSlug || slugCheck?.isAvailable === true

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    setErrorMsg(null)

    const formData = new FormData(e.currentTarget)
    const nameValue = name.trim()
    const descriptionValue = (formData.get(ShopCreationFormFields.DESCRIPTION) as string)?.trim() || ''
    const addressValue = (formData.get(ShopCreationFormFields.ADDRESS) as string)?.trim() || undefined
    const phoneValue = (formData.get(ShopCreationFormFields.PHONE_NUMBER) as string)?.trim() || undefined
    const slotDuration = Number(formData.get(ShopCreationFormFields.SLOT_DURATION)) || 30
    const allowCancellation = formData.get(ShopCreationFormFields.ALLOW_CANCELLATION) === 'yes'
    const cancellationDeadline = Number(formData.get(ShopCreationFormFields.CANCELLATION_DEADLINE)) || 24

    if (!nameValue) {
      setErrorMsg('Por favor introduza o nome do estabelecimento.')
      return
    }

    if (!isFormValid) {
      setErrorMsg('Por favor defina um slug único e disponível.')
      return
    }

    createShopMutation.mutate({
      body: {
        name: nameValue,
        slug: slug.trim() || undefined,
        description: descriptionValue,
        address: addressValue,
        phoneNumber: phoneValue,
        slotDurationMinutes: slotDuration,
        allowCancellation,
        cancellationDeadlineHours: cancellationDeadline,
        imageId: imageId || undefined,
      },
    })
  }

  const handleImageFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      setImagePreviewUrl(URL.createObjectURL(file))
      uploadPhotoMutation.mutate({
        params: { query: { format: 1 } },
        body: { file: file as unknown as string },
      })
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
                name={ShopCreationFormFields.NAME}
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
                  name={ShopCreationFormFields.SLUG}
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
                name={ShopCreationFormFields.DESCRIPTION}
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
                name={ShopCreationFormFields.ADDRESS}
                type="text"
                maxLength={200}
                placeholder="Ex: Rua Augusta 123, Lisboa"
                className="mt-1.5 w-full rounded-lg border border-input bg-background px-3.5 py-2 text-sm text-foreground focus:border-ring focus:ring-2 focus:ring-ring/20 focus:outline-none"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-foreground">Telefone de Contacto</label>
              <input
                name={ShopCreationFormFields.PHONE_NUMBER}
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
                name={ShopCreationFormFields.SLOT_DURATION}
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
                name={ShopCreationFormFields.ALLOW_CANCELLATION}
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
                name={ShopCreationFormFields.CANCELLATION_DEADLINE}
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
