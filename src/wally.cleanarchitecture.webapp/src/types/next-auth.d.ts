/* eslint-disable @typescript-eslint/no-unused-vars */
import NextAuth, { DefaultSession } from 'next-auth'

declare module 'next-auth' {
    interface Session extends DefaultSession {
        accessToken: string,
        user: {
            id: string,
            email: string,
            name: string,
            image: string
        }
    }
}