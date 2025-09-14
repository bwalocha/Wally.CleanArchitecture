"use client"

import {
    Database,
    Plus,
    LucideProps
} from "lucide-react"
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

import {
    useGetListQuery,
    useGetQuery
} 
from "@/features/files/store/pathsApi"
import {useEffect, useState} from "react";
import * as react from "react";
import {useParams, usePathname, useRouter, useSearchParams} from "next/navigation";

type Item = {title: string, count: number, url: string, icon: react.ForwardRefExoticComponent<Omit<LucideProps, "ref"> & react.RefAttributes<SVGSVGElement>>}

function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {

    // const router = useRouter();
    // const pathname = usePathname()
    // const searchParams = useSearchParams()
    const params = useParams<{ slug?: string[] }>()


    // const {id} = React.use(params)
    // const { toggleSidebar } = useSidebar()

    const [ items, setItems ] = useState<Item[]>([]);

    const {data, error, isLoading} = useGetQuery({ id: params.slug?.[0] ?? "", odata: undefined});
    // const {data, error, isLoading} = useGetQuery({ id: "", odata: undefined});
    
    // const {data, error, isLoading} = useGetListQuery();
    // const {data, error, isLoading} = useGetQuery({ id: "93a0de1e-9fcf-4a63-8000-089f7ff5f7b5", odata: undefined});
    // const {data, error, isLoading} = useGetQuery({ id: "88bbedf3-255a-42d9-8000-089f7ff5f7b5", odata: undefined});
    
    useEffect(() => {
        if (data) {
            const items = data?.items.map((item: any) => {
                return {
                    title: item.name,
                    count: -123,
                    url: `/storage/${item.id}`,
                    icon: Database,
                }
            })
            
            setItems(items)
        }
    }, [data])
        
    /*return (
        <div>
            <pre>DATA: {JSON.stringify(data, null, 2)}</pre>
            <code>ERROR: {JSON.stringify(error, null, 2)}</code>
            <code>LOADING: {JSON.stringify(isLoading, null, 2)}</code>
        </div>
    )*/

    return (
        <Sidebar collapsible={"icon"} {...props}>
            <SidebarContent>

                {/*<pre>{JSON.stringify(router, null, 2)}</pre>
                <pre>{JSON.stringify(pathname, null, 2)}</pre>
                <pre>{JSON.stringify(searchParams, null, 2)}</pre>
                <pre>{JSON.stringify(params, null, 2)}</pre>*/}

                <SidebarGroup>
                    <SidebarGroupLabel>Storages</SidebarGroupLabel>
                    <SidebarGroupAction>
                        <Plus/> <span className="sr-only">Add Storage</span>
                    </SidebarGroupAction>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {items.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <a href={item.url}>
                                            <item.icon/>
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

            <SidebarRail/>
        </Sidebar>
    )
}

export default AppSidebar