"use client";

import { getSession } from "@/lib/api";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function HomePage() {
  const router = useRouter();
  useEffect(() => {
    const s = getSession();
    router.replace(s ? "/dashboard" : "/login");
  }, [router]);
  return (
    <div className="flex min-h-screen items-center justify-center bg-bg text-sm text-muted">
      正在进入系统…
    </div>
  );
}
