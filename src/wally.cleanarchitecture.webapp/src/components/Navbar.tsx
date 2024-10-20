"use client"

import Image from "next/image";
import {Menu} from "lucide-react";
import {Button} from "@/components/ui/button";
import { SidebarTrigger } from "@/components/ui/sidebar"
import {Avatar, AvatarFallback, AvatarImage} from "@/components/ui/avatar";
import {ThemeSwitch} from "@/components/ThemeSwitch";

// https://kit.shadcnui.com/components/layout/navbar
function Navbar() {
    return (
        <header>
            <nav className="flex items-center justify-between bg-neutral-200">
                <div className="flex">
                    <Button>
                        <Menu/>
                    </Button>
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
                    <ThemeSwitch/>
                    <Avatar className="mr-2">
                        <AvatarImage src="https://github.com/shadcn.png"/>
                        <AvatarFallback>CN</AvatarFallback>
                    </Avatar>
                    <SidebarTrigger className="-mr-1 ml-auto rotate-180" />
                </div>
            </nav>
        </header>
);
}

export {
    Navbar
}