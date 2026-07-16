import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'
import { router } from "./router.ts"
import { RouterProvider } from "@tanstack/react-router"
import { AuthProvider } from "./lib/auth.tsx"
import "./index.css"

function App() {
    return (
        <AuthProvider>
            <RouterProvider router={router}/>
            <TanStackRouterDevtools router={router}/>
        </AuthProvider>
    )
}

export default App
