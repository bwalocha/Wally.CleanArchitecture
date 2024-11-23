// https://redux-toolkit.js.org/rtk-query/usage/error-handling
import { isRejectedWithValue } from '@reduxjs/toolkit'
import type { MiddlewareAPI, Middleware } from '@reduxjs/toolkit'
import {toast} from "@/hooks/use-toast";

export const errorHandlerMiddleware: Middleware =
    (api: MiddlewareAPI) => (next) => (action) => {
        if (isRejectedWithValue(action)) {
            toast({
                variant: "destructive",
                title: JSON.stringify(action.payload),
                description: 'data' in action.error
                    ? (action.error.data as { message: string }).message
                    : action.error.message,
                // action: <ToastAction altText="Try again">Try again</ToastAction>,
                duration: 24 * 60 * 60 * 1000,
            })
        }

        return next(action)
    }