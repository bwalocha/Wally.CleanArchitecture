import * as React from "react";

export default function Page() {
    return (
        <main className="flex flex-1 flex-col gap-4 p-4">
            <div className="grid auto-rows-min gap-4 md:grid-cols-3">
                <div className="aspect-video rounded-xl bg-muted/50"/>
                <div className="aspect-video rounded-xl bg-muted/50"/>
                <div className="aspect-video rounded-xl bg-muted/50"/>
            </div>
            <p>HOME</p>
            <div className="min-h-[100vh] flex-1 rounded-xl bg-muted/50 md:min-h-min">
                <p>VER: v.{process.env.NEXT_PUBLIC_VERSION}</p>
                <p>URL: {process.env.NEXT_PUBLIC_API_BASE_URL}</p>                
            </div>
        </main>
    );
}
