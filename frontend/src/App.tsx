import {TanStackRouterDevtools} from '@tanstack/react-router-devtools'
import {router} from "./router.ts"
import {RouterProvider} from "@tanstack/react-router"
import "./index.css"

function App() {
    return (
        <>
            <RouterProvider router={router}/>
            <TanStackRouterDevtools router={router}/>
        </>
    )
}

export default App
