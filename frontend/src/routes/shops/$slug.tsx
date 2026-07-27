import { createFileRoute, Link } from '@tanstack/react-router'
import { $api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import {
  Store,
  MapPin,
  Phone,
  Clock,
  CalendarCheck,
  Loader2,
  ArrowLeft,
  CheckCircle2,
  Share2
} from 'lucide-react'

export const Route = createFileRoute('/shops/$slug')({
  component: PublicShopPage,
})

function PublicShopPage() {
  const { slug } = Route.useParams()

  const { data: shop, isLoading, isError } = $api.useQuery(
    'get',
    '/api/Shops/{slug}',
    {
      params: { path: { slug } }
    }
  )

  if (isLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  if (isError || !shop) {
    return (
      <div className="mx-auto max-w-2xl px-4 py-20 text-center">
        <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-destructive/10 text-destructive">
          <Store className="h-8 w-8" />
        </div>
        <h1 className="font-heading text-2xl font-bold text-foreground sm:text-3xl">
          Estabelecimento Não Encontrado
        </h1>
        <p className="mt-2 text-muted-foreground">
          O estabelecimento com o slug "{slug}" não existe ou pode ter sido removido.
        </p>
        <div className="mt-6">
          <Link to="/">
            <Button variant="outline" className="gap-2">
              <ArrowLeft className="h-4 w-4" />
              Voltar ao Início
            </Button>
          </Link>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Banner / Cover Header */}
      <div className="relative h-64 w-full bg-gradient-to-r from-primary/20 via-primary/10 to-background sm:h-80">
        {shop.imageUrl ? (
          <img
            src={shop.imageUrl}
            alt={shop.name}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center bg-muted/40 text-muted-foreground/30">
            <Store className="h-24 w-24 opacity-30" />
          </div>
        )}
        <div className="absolute inset-0 bg-gradient-to-t from-background via-background/40 to-transparent" />
      </div>

      {/* Main Content Container */}
      <div className="relative mx-auto -mt-20 max-w-5xl px-4 pb-16 sm:px-6 lg:px-8">
        <div className="rounded-2xl border border-border bg-card p-6 shadow-xl backdrop-blur-md sm:p-8">
          <div className="flex flex-col justify-between gap-6 md:flex-row md:items-start">
            <div>
              <div className="inline-flex items-center gap-2 rounded-full bg-emerald-500/10 px-3 py-1 text-xs font-semibold text-emerald-600 dark:text-emerald-400">
                <CheckCircle2 className="h-3.5 w-3.5" />
                Estabelecimento Verificado
              </div>

              <h1 className="mt-3 font-heading text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
                {shop.name}
              </h1>

              <p className="mt-2 text-sm text-muted-foreground font-mono">
                ontime.pt/shops/{shop.slug}
              </p>

              {shop.description && (
                <p className="mt-4 max-w-2xl text-base text-muted-foreground leading-relaxed">
                  {shop.description}
                </p>
              )}
            </div>

            <div className="flex shrink-0 gap-3">
              <Button
                variant="outline"
                size="icon"
                onClick={() => {
                  navigator.clipboard.writeText(window.location.href)
                  alert('Link copiado para a área de transferência!')
                }}
                className="cursor-pointer"
                title="Partilhar link"
              >
                <Share2 className="h-4 w-4" />
              </Button>

              <Button size="lg" className="cursor-pointer gap-2 font-semibold shadow-md">
                <CalendarCheck className="h-5 w-5" />
                Reservar Marcação
              </Button>
            </div>
          </div>

          <hr className="my-6 border-border" />

          {/* Details Grid */}
          <div className="grid gap-6 sm:grid-cols-3">
            {shop.address && (
              <div className="flex items-start gap-3 rounded-xl border border-border/50 bg-muted/20 p-4">
                <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
                  <MapPin className="h-5 w-5" />
                </div>
                <div>
                  <p className="text-xs font-medium text-muted-foreground">Endereço</p>
                  <p className="mt-0.5 text-sm font-semibold text-foreground">{shop.address}</p>
                </div>
              </div>
            )}

            {shop.phoneNumber && (
              <div className="flex items-start gap-3 rounded-xl border border-border/50 bg-muted/20 p-4">
                <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
                  <Phone className="h-5 w-5" />
                </div>
                <div>
                  <p className="text-xs font-medium text-muted-foreground">Telefone</p>
                  <p className="mt-0.5 text-sm font-semibold text-foreground">{shop.phoneNumber}</p>
                </div>
              </div>
            )}

            <div className="flex items-start gap-3 rounded-xl border border-border/50 bg-muted/20 p-4">
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
                <Clock className="h-5 w-5" />
              </div>
              <div>
                <p className="text-xs font-medium text-muted-foreground">Duração da Marcação</p>
                <p className="mt-0.5 text-sm font-semibold text-foreground">
                  {shop.slotDurationMinutes} minutos por slot
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
