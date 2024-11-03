import * as React from "react";
import Sidebar from "./Sidebar";

export default function Layout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
    return (
        <div className="flex flex-1">
            <Sidebar collapsible="none" className="sticky hidden md:flex h-full" />
            {children}
        </div>
    );
}
