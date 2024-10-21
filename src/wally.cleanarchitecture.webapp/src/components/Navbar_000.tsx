"use client"

import Image from "next/image";
import {Menu} from "lucide-react";
import {Button} from "@/components/ui/button";
import { SidebarTrigger } from "@/components/ui/sidebar"
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";
import {ThemeSwitch} from "@/components/ThemeSwitch";
import { useSidebar } from "@/components/ui/sidebar"

// https://kit.shadcnui.com/components/layout/navbar
function Navbar() {
    const { toggleSidebar } = useSidebar()
    
    return (
        <header className="sticky border-b-[1px] top-0 z-40 w-full bg-white dark:border-b-slate-700 dark:bg-background">
            <nav className="flex items-center justify-between bg-neutral-200">
                <div className="flex">
                    <Image
                        className="dark:invert"
                        src="/logo.svg"
                        alt="logo"
                        width={180}
                        height={38}
                        priority
                    />
                </div>
                <div className="flex">
                    <ThemeSwitch />
                    <Avatar className="mr-2">
                        <AvatarImage src="https://github.com/shadcn.png"/>
                        <AvatarFallback>CN</AvatarFallback>
                    </Avatar>
                    <Button onClick={toggleSidebar}>
                        {/*<SidebarTrigger className="ml-auto rotate-180" />*/}
                        <Menu/>
                    </Button>


                </div>
            </nav>
        </header>
    );
}

export {
    Navbar
}