import { Fragment } from 'react'
import { Outlet, createRootRoute } from '@tanstack/react-router'
import { Navbar } from '@/components/Navbar'

export const Route = createRootRoute({
  component: RootComponent,
})

function RootComponent() {
  return (
    <Fragment>
      <div className="min-h-screen bg-background">
        <Navbar />
        <main>
          <Outlet />
        </main>
      </div>
    </Fragment>
  )
}
