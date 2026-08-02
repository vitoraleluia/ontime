import { useEffect } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useAuth } from '@/lib/auth'
import { $api } from '@/lib/api'
import { Loader2 } from 'lucide-react'
import { ClientUpgradePrompt } from '@/components/create-shop/ClientUpgradePrompt'
import { ShopCreationForm } from '@/components/create-shop/ShopCreationForm'

export const Route = createFileRoute('/create-shop')({
  component: CreateShopPage,
})

function CreateShopPage() {
  const { isAuthenticated, isLoading: isAuthLoading } = useAuth()
  const navigate = useNavigate()

  const { data: profile, isLoading: isProfileLoading, refetch } = $api.useQuery(
    'get',
    '/api/Account',
    {},
    { enabled: isAuthenticated }
  )

  useEffect(() => {
    if (!isAuthLoading && !isAuthenticated) {
      navigate({ to: '/login', search: { returnUrl: '/create-shop' } })
    }
  }, [isAuthLoading, isAuthenticated, navigate])

  if (isAuthLoading || isProfileLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  const isProfessional = profile?.isProfessional

  if (!isProfessional) {
    return <ClientUpgradePrompt refetchProfile={refetch} />
  }

  return <ShopCreationForm navigate={navigate} />
}
