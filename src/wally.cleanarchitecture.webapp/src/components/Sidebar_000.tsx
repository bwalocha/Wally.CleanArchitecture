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
        title: "Home",
        url: "/home",
        icon: Home,
    },
    {
        title: "Inbox",
        url: "/",
        icon: Inbox,
    },
    {
        title: "Calendar",
        url: "#",
        icon: Calendar,
    },
    {
        title: "Paths",
        // url: "/storage/00000000-0000-0000-0000-000000000000",
        url: "/paths",
        icon: Database,
    },
    {
        title: "Storages",
        // url: "/storage/00000000-0000-0000-0000-000000000000",
        url: "/storages",
        icon: Database,
    },
    {
        title: "Plugins",
        url: "/plugins",
        icon: Blocks
    },
    {
        title: "Search",
        url: "/search",
        icon: Search,
    },
]

function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {

    const { toggleSidebar } = useSidebar();
    
    return (
        <Sidebar collapsible={"icon"} {...props}>
            <SidebarHeader className="group-data-[collapsible=icon]:hidden">
                <SidebarMenu>
                    <SidebarMenuItem>
                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <SidebarMenuButton>
                                    Select Workspace
                                    <ChevronDown className="ml-auto" />
                                </SidebarMenuButton>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent className="w-[--radix-popper-anchor-width]">
                                <DropdownMenuItem>
                                    <span>Acme Inc</span>
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                    <span>Acme Corp.</span>
                                </DropdownMenuItem>
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </SidebarMenuItem>
                </SidebarMenu>
            </SidebarHeader>

            <SidebarSeparator className="group-data-[collapsible=icon]:hidden" />
            
            <SidebarContent>
                <Collapsible defaultOpen className="group/collapsible">
                    <SidebarGroup className="group-data-[collapsible=icon]:hidden">
                        <SidebarGroupLabel asChild>
                            <CollapsibleTrigger>
                                Help
                                <ChevronDown className="ml-auto transition-transform group-data-[state=open]/collapsible:rotate-180" />
                            </CollapsibleTrigger>
                        </SidebarGroupLabel>
                        <CollapsibleContent>
                            <SidebarGroupContent />
                        </CollapsibleContent>
                    </SidebarGroup>
                </Collapsible>

                <SidebarSeparator className="group-data-[collapsible=icon]:hidden" />
                
                <SidebarGroup className="group-data-[collapsible=icon]:hidden">
                    <SidebarGroupLabel asChild>Projects</SidebarGroupLabel>
                    <SidebarGroupAction title="Add Project">
                        <Plus /> <span className="sr-only">Add Project</span>
                    </SidebarGroupAction>
                    <SidebarGroupContent />
                </SidebarGroup>

                <SidebarSeparator className="group-data-[collapsible=icon]:hidden" />
                
                <SidebarGroup>
                    <SidebarGroupLabel>Application</SidebarGroupLabel>
                    <SidebarGroupAction>
                        <Plus /> <span className="sr-only">Add Project</span>
                    </SidebarGroupAction>
                    <SidebarGroupContent></SidebarGroupContent>
                </SidebarGroup>

                <SidebarSeparator className="group-data-[collapsible=icon]:hidden" />
                
                <SidebarGroup>
                    <SidebarGroupLabel>Application</SidebarGroupLabel>
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
                                    <SidebarMenuBadge>24</SidebarMenuBadge>
                                </SidebarMenuItem>
                            ))}
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
                
            </SidebarContent>

            <SidebarSeparator />

            <SidebarFooter>
                <SidebarMenu>
                    <SidebarMenu>
                            <SidebarMenuItem>
                                <SidebarMenuButton asChild>
                                    <a href={"settings"}>
                                        <Settings />
                                        <span>Settings</span>
                                    </a>
                                </SidebarMenuButton>
                                <SidebarMenuAction onClick={() => toggleSidebar()}>
                                    <ChevronsLeft />
                                    {/*<SidebarTrigger className="-ml-1"/>*/}
                                </SidebarMenuAction>
                            </SidebarMenuItem>
                    </SidebarMenu>
                </SidebarMenu>
            </SidebarFooter>

            <SidebarRail />
        </Sidebar>
    )
}

export default AppSidebar