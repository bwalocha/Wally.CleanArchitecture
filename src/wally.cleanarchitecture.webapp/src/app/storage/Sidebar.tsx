"use client"

import {Blocks, Database, Calendar, ChevronDown, ChevronUp, Home, Inbox, Plus, Search, Settings, User2} from "lucide-react"
import {ChevronsLeft} from "lucide-react";
import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupAction,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuAction,
    SidebarMenuBadge,
    SidebarMenuButton,
    SidebarMenuItem,
    SidebarRail,
    SidebarSeparator, SidebarTrigger, useSidebar,
} from "@/components/ui/sidebar"
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger} from "@/components/ui/dropdown-menu";
import {Collapsible, CollapsibleContent, CollapsibleTrigger} from "@/components/ui/collapsible";
import * as React from "react";
import {Button} from "@/components/ui/button";

// Menu items.
const items = [
    {
        title: "Arcade",
        count: 9876,
        url: "/storage/1001",
        icon: Database,
    },
    {
        title: "Audio",
        count: 1001,
        url: "/storage/1002",
        icon: Database,
    },
    {
        title: "Book",
        count: 420,
        url: "/storage/1003",
        icon: Database,
    },
    {
        title: "Photo",
        count: 1200,
        url: "/storage/1004",
        icon: Database,
    },
    {
        title: "Stuff",
        count: 2100,
        url: "/storage/1005",
        icon: Database,
    },
    {
        title: "Video",
        count: 6100,
        url: "/storage/1006",
        icon: Database
    },
]

function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {

    const { toggleSidebar } = useSidebar();
    
    return (
        <Sidebar collapsible={"icon"} {...props}>
            <SidebarContent>
                <SidebarGroup>
                    <SidebarGroupLabel>Storages</SidebarGroupLabel>
                    <SidebarGroupAction>
                        <Plus /> <span className="sr-only">Add Storage</span>
                    </SidebarGroupAction>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {items.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <a href={item.url}>
                                            <item.icon />
                                            <span>{item.title}</span>
                                        </a>
                                    </SidebarMenuButton>
                                    <SidebarMenuBadge>{item.count}</SidebarMenuBadge>
                                </SidebarMenuItem>
                            ))}
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
                
            </SidebarContent>
            
            <SidebarRail />
        </Sidebar>
    )
}

export default AppSidebar