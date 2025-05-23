﻿import NextAuth, { /*User,*/ NextAuthConfig } from "next-auth";
import Google from "next-auth/providers/google"

// const BASE_PATH = "/api/auth";
const authOptions: NextAuthConfig = {
    providers: [
        Google({
            authorization: {
                params: {
                    prompt: "consent",
                    access_type: "offline",
                    response_type: "code",
                },
            },
        }),
    ],
    // basePath: BASE_PATH,
    // secret: process.env.AUTH_SECRET,
    callbacks: {
        async jwt({ token, account/*, user, profile, trigger, session*/ }) {
                // console.log('token:', token)
                // console.log('user:', user)
                // console.log('account:', account)
                // console.log('profile:', profile) 
                // console.log('trigger:', trigger) 
                // console.log('session:', session)
                
            if (account) {
                token.sub = account.providerAccountId
                token.accessToken = account.access_token
            }
            
            return token;
        },
        async session({ session, token }) {
            // console.log(token)
            
            return { ...session, accessToken: token.accessToken, user: { ...session.user, id: token.sub } }
        },
        async signIn({ account, profile }): Promise<string | false | true> {
            // console.log(account, profile)
            
            if (account?.provider === "google") {
                return profile?.email_verified === true // && profile?.email.endsWith("@example.com")
            }

            return true // Do different verification for other providers that don't have `email_verified`
        },
        authorized: async ({ auth }) => {
            // Logged in users are authenticated, otherwise redirect to login page
            return !!auth
        },
    },
};

export const { handlers, auth } = NextAuth(authOptions);
