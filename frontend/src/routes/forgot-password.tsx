import { useState } from 'react'
import { createFileRoute, Link } from '@tanstack/react-router'
import { CalendarRange } from 'lucide-react'
import { $api } from '@/lib/api'
import { ErrorUtils } from '@/domain/utils/ErrorUtils'
import {
  ForgotPasswordSuccessState,
  ForgotPasswordForm,
  ForgotPasswordFormFields,
} from '@/components/auth/ForgotPasswordComponents'

export const Route = createFileRoute('/forgot-password')({
  component: ForgotPasswordPage,
})

function ForgotPasswordPage() {
  const [submittedEmail, setSubmittedEmail] = useState<string | null>(null)
  const [errorMsg, setErrorMsg] = useState<string | null>(null)

  const forgotPasswordMutation = $api.useMutation('post', '/api/Auth/forgot-password', {
    onError: (err: unknown) => {
      const msg = ErrorUtils.extractMessage(err, 'Falha ao enviar o pedido de recuperação. Tente novamente.')
      setErrorMsg(msg)
    },
  })

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const email = (formData.get(ForgotPasswordFormFields.EMAIL) as string)?.trim()

    if (!email) {
      setErrorMsg('Por favor, introduza o seu endereço de email.')
      return
    }

    setErrorMsg(null)
    setSubmittedEmail(email)
    forgotPasswordMutation.mutate({
      body: { email },
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
            Recuperar Palavra-passe
          </h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Introduza o seu endereço de email para receber as instruções de recuperação.
          </p>
        </div>

        {/* Card */}
        <div className="rounded-2xl border border-border bg-card p-6 shadow-sm sm:p-8">
          {forgotPasswordMutation.isSuccess && submittedEmail ? (
            <ForgotPasswordSuccessState email={submittedEmail} />
          ) : (
            <ForgotPasswordForm
              error={errorMsg}
              isLoading={forgotPasswordMutation.isPending}
              onSubmit={handleSubmit}
            />
          )}
        </div>
      </div>
    </div>
  )
}
