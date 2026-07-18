import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'
import { router } from "./router.ts"
import { RouterProvider } from "@tanstack/react-router"
import { AuthProvider } from "./lib/auth.tsx"
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import "./index.css"

const queryClient = new QueryClient()

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <AuthProvider>
                <RouterProvider router={router}/>
                <TanStackRouterDevtools router={router}/>
            </AuthProvider>
        </QueryClientProvider>
    )
}

export default App
