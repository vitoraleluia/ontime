import { useState } from 'react'
import { createFileRoute, useSearch, Link } from '@tanstack/react-router'
import { CalendarRange } from 'lucide-react'
import { $api } from '@/lib/api'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'
import { ResetPasswordCardContent } from '@/components/auth/ResetPasswordComponents'

export const Route = createFileRoute('/reset-password')({
  component: ResetPasswordPage,
})

function ResetPasswordPage() {
  const search = useSearch({ from: '/reset-password' }) as { email?: string; token?: string }
  const email = search?.email ?? ''
  const token = search?.token ?? ''

  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const isInvalidLink = !email || !token

  const resetPasswordMutation = $api.useMutation('post', '/api/Auth/reset-password', {
    onError: (err: unknown) => {
      const msg = ErrorUtils.extractMessage(err, 'Falha ao redefinir a palavra-passe. O link pode ter expirado.')
      setErrorMsg(msg)
    },
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!newPassword || !confirmPassword) {
      setErrorMsg('Por favor, preencha todos os campos.')
      return
    }

    if (newPassword !== confirmPassword) {
      setErrorMsg('As palavras-passe não coincidem.')
      return
    }

    if (newPassword.length < 6) {
      setErrorMsg('A palavra-passe deve ter pelo menos 6 caracteres.')
      return
    }

    setErrorMsg(null)
    resetPasswordMutation.mutate({
      body: {
        email,
        token,
        newPassword,
      },
    })
  }

  return (
    <div className="flex min-h-[80vh] flex-col items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
      <div className="w-full max-w-md space-y-8">
        {/* Header */}
        <div className="text-center">
          <Link to="/" className="inline-flex items-center gap-2 text-2xl font-bold tracking-tight text-foreground">
            <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground shadow-md shadow-primary/20">
              <CalendarRange className="h-6 w-6" />
            </div>
          </Link>
          <h2 className="mt-6 text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            Redefinir Palavra-passe
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Introduza a sua nova palavra-passe nos campos abaixo.
          </p>
        </div>

        {/* Card */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8">
          <ResetPasswordCardContent
            isInvalidLink={isInvalidLink}
            isSuccess={resetPasswordMutation.isSuccess}
            newPassword={newPassword}
            setNewPassword={setNewPassword}
            confirmPassword={confirmPassword}
            setConfirmPassword={setConfirmPassword}
            error={errorMsg}
            isLoading={resetPasswordMutation.isPending}
            onSubmit={handleSubmit}
          />
        </div>
      </div>
    </div>
  )
}
