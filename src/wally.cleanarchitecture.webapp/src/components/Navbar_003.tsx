"use client"
import { useSession } from "next-auth/react"

import {SidebarTrigger} from "@/components/ui/sidebar";
import {Separator} from "@/components/ui/separator";
import {
    Breadcrumb,
    BreadcrumbItem,
    BreadcrumbLink,
    BreadcrumbList, BreadcrumbPage,
    BreadcrumbSeparator
} from "@/components/ui/breadcrumb";
import * as React from "react";
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger} from "@/components/ui/dropdown-menu";
import {Input} from "@/components/ui/input";

import {Bell} from "lucide-react";

import {ThemeSwitch} from "@/components/ThemeSwitch";
import {SignIn} from "@/components/SignIn";
import {SignOut} from "@/components/SignOut";

function Navbar() {
    const { data: session, status } = useSession()
    
    return (
        <header className="flex h-16 shrink-0 items-center gap-2 border-b px-4">
            <div className="flex flex-1 items-center gap-2 px-3">
                <SidebarTrigger className="-ml-1"/>
                <Separator orientation="vertical" className="mr-2 h-4"/>
                <Breadcrumb>
                    <BreadcrumbList>
                        <BreadcrumbItem className="hidden md:block">
                            <BreadcrumbLink href="#">components</BreadcrumbLink>
                        </BreadcrumbItem>
                        <BreadcrumbSeparator className="hidden md:block"/>
                        <BreadcrumbItem className="hidden md:block">
                            <BreadcrumbLink href="#">ui</BreadcrumbLink>
                        </BreadcrumbItem>
                        <BreadcrumbSeparator className="hidden md:block"/>
                        <BreadcrumbItem>
                            <BreadcrumbPage>button.tsx</BreadcrumbPage>
                        </BreadcrumbItem>
                    </BreadcrumbList>
                </Breadcrumb>
            </div>

            <div className="ml-auto flex gap-10 px-3">
                <Input type="search" placeholder="Search..." />

                <Bell size={32} />

                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                                <Avatar>
                                    {/*<AvatarImage src="https://gravatar.com/avatar/04ef6a0bd30e5ce6b2100506bc5e8c74?size=64"/>*/}
                                    <AvatarImage src={session?.user?.image ?? ""} alt="Profile" referrerPolicy={'no-referrer'} />
                                    <AvatarFallback>??</AvatarFallback>
                                </Avatar>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent
                            side="bottom"
                            className="w-[--radix-popper-anchor-width]"
                        >
                            <DropdownMenuItem>
                                <span>Account</span>
                            </DropdownMenuItem>
                            <DropdownMenuItem>
                                <ThemeSwitch />
                            </DropdownMenuItem>
                            
                            <Separator className="my-2" />
                            
                            <DropdownMenuItem>
                                {status === "authenticated" ? <SignOut /> : <SignIn />}                                
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
            </div>
        </header>
    )
}

export default Navbar;