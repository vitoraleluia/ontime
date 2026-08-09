import {useState} from 'react'
import {Link} from '@tanstack/react-router'
import {useAuth} from '@/lib/auth'
import {Button} from '@/components/ui/button'
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger
} from '@/components/ui/dropdown-menu'
import {CalendarRange, Menu, X, LogOut, User, Loader2, Store} from 'lucide-react'

export function Navbar() {
    const [isMenuOpen, setIsMenuOpen] = useState(false)

    return (
        <nav className="sticky top-0 z-50 w-full border-b border-border bg-background/85 backdrop-blur-md">
            <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                <div className="flex h-16 items-center justify-between">

                    {/* Logo */}
                    <div className="flex items-center">
                        <Link to="/"
                              className="flex items-center gap-2 text-xl font-bold tracking-tight text-foreground transition-all hover:opacity-90">
                            <div
                                className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary text-primary-foreground shadow-md shadow-primary/20">
                                <CalendarRange className="h-5 w-5"/>
                            </div>
                            <span className="font-heading">OnTime</span>
                        </Link>
                    </div>

                    {/* Desktop Navigation */}
                    <div className="hidden md:flex md:items-center md:gap-6">
                        <Link
                            to="/"
                            activeProps={{className: "text-primary"}}
                            inactiveProps={{className: "text-muted-foreground hover:text-foreground"}}
                            className="text-sm font-medium transition-colors"
                        >
                            Início
                        </Link>

                        {/* Auth Section */}
                        <div className="flex items-center gap-3 border-l border-border pl-6">
                            <DesktopAuthSection />
                        </div>
                    </div>

                    {/* Mobile menu button */}
                    <div className="flex md:hidden">
                        <button
                            onClick={() => setIsMenuOpen(!isMenuOpen)}
                            type="button"
                            className="inline-flex items-center justify-center rounded-md p-2 text-muted-foreground hover:bg-muted hover:text-foreground focus:outline-none"
                            aria-controls="mobile-menu"
                            aria-expanded={isMenuOpen}
                        >
                            <span className="sr-only">Abrir menu</span>
                            {isMenuOpen ? <X className="h-6 w-6"/> : <Menu className="h-6 w-6"/>}
                        </button>
                    </div>
                </div>
            </div>

            {/* Mobile Menu Panel */}
            {isMenuOpen && (
                <div className="md:hidden border-t border-border bg-background" id="mobile-menu">
                    <div className="space-y-1 px-2 pt-2 pb-3 sm:px-3">
                        <Link
                            to="/"
                            onClick={() => setIsMenuOpen(false)}
                            className="block rounded-md px-3 py-2 text-base font-medium text-foreground hover:bg-muted"
                        >
                            Início
                        </Link>
                    </div>
                    <div className="border-t border-border pt-4 pb-3 px-4">
                        <MobileAuthSection closeMenu={() => setIsMenuOpen(false)} />
                    </div>
                </div>
            )}
        </nav>
    )
}

function UserAvatar({ pictureUrl, initials, className = "h-4 w-4" }: { pictureUrl?: string | null; initials?: string; className?: string }) {
    if (pictureUrl) {
        return <img src={pictureUrl} alt="Avatar" className="h-full w-full object-cover" />
    }
    if (initials) {
        return <span className="text-xs font-bold text-foreground">{initials}</span>
    }
    return <User className={className} />
}

function DesktopAuthSection() {
    const { isAuthenticated, isLoading, logout, profile } = useAuth()

    if (isLoading) {
        return (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Loader2 className="h-4 w-4 animate-spin text-primary"/>
                <span>A carregar...</span>
            </div>
        )
    }

    if (isAuthenticated) {
        const userName = [profile?.firstName, profile?.lastName].filter(Boolean).join(' ') || 'A minha conta'
        const initials = `${profile?.firstName?.charAt(0) || ''}${profile?.lastName?.charAt(0) || ''}`.toUpperCase()

        return (
            <DropdownMenu>
                <DropdownMenuTrigger
                    className="group/button inline-flex shrink-0 items-center justify-center rounded-full border border-border bg-background size-9 cursor-pointer hover:bg-muted text-muted-foreground hover:text-foreground transition-colors outline-hidden focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 overflow-hidden">
                    <UserAvatar pictureUrl={profile?.profilePictureUrl} initials={initials} />
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-56">
                    <DropdownMenuGroup>
                        <DropdownMenuLabel className="font-normal">
                            <div className="flex flex-col space-y-1">
                                <p className="text-sm font-medium leading-none text-foreground">{userName}</p>
                                {profile?.email && (
                                    <p className="text-xs leading-none text-muted-foreground truncate">{profile.email}</p>
                                )}
                            </div>
                        </DropdownMenuLabel>
                        <DropdownMenuSeparator/>
                        <DropdownMenuItem className="p-0">
                            <Link to="/account" className="cursor-pointer flex w-full items-center px-2 py-1.5">
                                <User className="mr-2 h-4 w-4"/>
                                <span>Editar Perfil</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem className="p-0">
                            <Link to="/create-shop" className="cursor-pointer flex w-full items-center px-2 py-1.5">
                                <Store className="mr-2 h-4 w-4 text-primary"/>
                                <span>Criar Loja</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuSeparator/>
                        <DropdownMenuItem className="text-destructive cursor-pointer"
                                          onClick={() => logout()}>
                            <LogOut className="mr-2 h-4 w-4"/>
                            <span>Terminar Sessão</span>
                        </DropdownMenuItem>
                    </DropdownMenuGroup>
                </DropdownMenuContent>
            </DropdownMenu>
        )
    }

    return (
        <>
            <Link to="/register">
                <Button variant="ghost" size="sm" className="cursor-pointer">
                    Registar
                </Button>
            </Link>
            <Link to="/login">
                <Button variant="default" size="sm" className="cursor-pointer font-semibold shadow-xs">
                    Entrar
                </Button>
            </Link>
        </>
    )
}

function MobileAuthSection({ closeMenu }: { closeMenu: () => void }) {
    const { isAuthenticated, isLoading, logout, profile } = useAuth()

    if (isLoading) {
        return (
            <div className="flex items-center gap-2 py-2 text-muted-foreground">
                <Loader2 className="h-5 w-5 animate-spin text-primary"/>
                <span>A carregar sessão...</span>
            </div>
        )
    }

    if (isAuthenticated) {
        const userName = [profile?.firstName, profile?.lastName].filter(Boolean).join(' ') || 'Utilizador Autenticado'
        const initials = `${profile?.firstName?.charAt(0) || ''}${profile?.lastName?.charAt(0) || ''}`.toUpperCase()

        return (
            <div className="space-y-3">
                <div className="flex items-center gap-3">
                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-secondary text-secondary-foreground border border-border overflow-hidden">
                        <UserAvatar pictureUrl={profile?.profilePictureUrl} initials={initials} className="h-5 w-5" />
                    </div>
                    <div>
                        <p className="text-base font-semibold leading-none text-foreground">{userName}</p>
                        {profile?.email && <p className="text-xs text-muted-foreground mt-1">{profile.email}</p>}
                    </div>
                </div>
                <Link
                    to="/account"
                    onClick={closeMenu}
                    className="flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium text-foreground hover:bg-muted"
                >
                    <User className="h-5 w-5 text-muted-foreground"/>
                    <span>Editar Perfil</span>
                </Link>
                <Button
                    variant="outline"
                    onClick={() => {
                        closeMenu()
                        logout()
                    }}
                    className="w-full justify-center gap-1.5 text-destructive hover:bg-destructive/10 hover:text-destructive"
                >
                    <LogOut className="h-4 w-4"/>
                    Terminar Sessão
                </Button>
            </div>
        )
    }

    return (
        <div className="flex flex-col gap-2">
            <Link to="/register" onClick={closeMenu}>
                <Button variant="outline" className="w-full justify-center">
                    Registar
                </Button>
            </Link>
            <Link to="/login" onClick={closeMenu}>
                <Button variant="default" className="w-full justify-center">
                    Entrar
                </Button>
            </Link>
        </div>
    )
}

