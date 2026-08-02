import { Loader2, CheckCircle2, XCircle } from 'lucide-react'

interface SlugStatusIconProps {
  isChecking: boolean
  checkResult?: { isAvailable?: boolean } | null
}

export function SlugStatusIcon({ isChecking, checkResult }: SlugStatusIconProps) {
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
