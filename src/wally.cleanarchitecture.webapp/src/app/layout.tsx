import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import {SidebarInset, SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"

import { ThemeProvider } from "@/components/providers/theme-provider";

import Navbar from "@/components/Navbar_003";
import Sidebar from "@/components/Sidebar_000";
import {Separator} from "@/components/ui/separator";
import {
    Breadcrumb,
    BreadcrumbItem,
    BreadcrumbLink,
    BreadcrumbList, BreadcrumbPage,
    BreadcrumbSeparator
} from "@/components/ui/breadcrumb";
import * as React from "react";

export const metadata: Metadata = {
    metadataBase: new URL(
        process.env.APP_URL
            ? `${process.env.APP_URL}`
            : process.env.VERCEL_URL
                ? `https://${process.env.VERCEL_URL}`
                : `http://localhost:${process.env.PORT || 3000}`
    ),
    title: "shadcn/ui sidebar",
    description:
        "A stunning and functional retractable sidebar for Next.js built on top of shadcn/ui complete with desktop and mobile responsiveness.",
    alternates: {
        canonical: "/"
    },
    openGraph: {
        url: "/",
        title: "shadcn/ui sidebar",
        description:
            "A stunning and functional retractable sidebar for Next.js built on top of shadcn/ui complete with desktop and mobile responsiveness.",
        type: "website"
    },
    twitter: {
        card: "summary_large_image",
        title: "shadcn/ui sidebar",
        description:
            "A stunning and functional retractable sidebar for Next.js built on top of shadcn/ui complete with desktop and mobile responsiveness."
    }
};


const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
});
const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
});

/*https://github.com/salimi-my/shadcn-ui-sidebar*/
// https://shadcn-ui-sidebar.salimi.my/dashboard
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={`${geistSans.variable} ${geistMono.variable} antialiased`}>
      <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
          <SidebarProvider>
              <Sidebar/>
              <SidebarInset>
                  <Navbar />
                  {children}
              </SidebarInset>
          </SidebarProvider>
      </ThemeProvider>
      </body>
    </html>
  );
}
