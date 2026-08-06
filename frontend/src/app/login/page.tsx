"use client";

import { Button, Card, Input, Label } from "@/components/ui";
import { api, getSession, setSession } from "@/lib/api";
import { FileText, ShieldCheck } from "lucide-react";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";

export default function LoginPage() {
  const router = useRouter();
  const [loginName, setLoginName] = useState("admin");
  const [password, setPassword] = useState("admin123");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (getSession()) router.replace("/dashboard");
  }, [router]);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");
    const res = await api.login(loginName.trim(), password);
    setLoading(false);
    if (!res.success || !res.data) {
      setError(res.message || "登录失败");
      return;
    }
    setSession(res.data);
    router.replace("/dashboard");
  }

  return (
    <div className="relative min-h-screen overflow-hidden bg-bg">
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_right,rgba(13,148,136,0.12),transparent_40%),radial-gradient(circle_at_bottom_left,rgba(15,118,110,0.08),transparent_35%)]" />
      <div className="relative mx-auto grid min-h-screen max-w-6xl items-center gap-10 px-4 py-10 lg:grid-cols-2 lg:px-8">
        <div className="hidden lg:block">
          <div className="inline-flex items-center gap-2 rounded-full bg-primary-soft px-3 py-1 text-xs font-semibold text-accent">
            <ShieldCheck className="size-3.5" />
            企业财务
          </div>
          <h1 className="font-display mt-5 text-4xl font-semibold tracking-tight text-fg">
            复大财务报销系统
          </h1>
          <p className="mt-4 max-w-md text-base leading-relaxed text-muted">
            在线发起费用报销、设置审批流程、跟踪签字进度，支持多院区与多部门协同。
          </p>
          <ul className="mt-8 space-y-3 text-sm text-muted">
            <li className="flex items-center gap-2">
              <span className="size-1.5 rounded-full bg-primary" /> 草稿提交、逐级审批、作废全流程
            </li>
            <li className="flex items-center gap-2">
              <span className="size-1.5 rounded-full bg-primary" /> 待办签字与进度一目了然
            </li>
            <li className="flex items-center gap-2">
              <span className="size-1.5 rounded-full bg-primary" /> 部门数据隔离，权限清晰
            </li>
          </ul>
        </div>

        <Card className="mx-auto w-full max-w-md p-6 sm:p-8">
          <div className="mb-6 flex items-center gap-3">
            <div className="flex size-11 items-center justify-center rounded-xl bg-primary text-primary-fg">
              <FileText className="size-5" />
            </div>
            <div>
              <div className="font-display text-lg font-semibold">登录</div>
              <div className="text-sm text-muted">请输入账号与密码</div>
            </div>
          </div>

          <form onSubmit={onSubmit} className="space-y-4">
            <div>
              <Label>账号</Label>
              <Input
                value={loginName}
                onChange={(e) => setLoginName(e.target.value)}
                placeholder="请输入账号"
                autoComplete="username"
                required
              />
            </div>
            <div>
              <Label>密码</Label>
              <Input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="请输入密码"
                autoComplete="current-password"
                required
              />
            </div>
            {error && (
              <div className="rounded-md bg-danger-soft px-3 py-2 text-sm text-danger">{error}</div>
            )}
            <Button type="submit" className="w-full" loading={loading}>
              进入系统
            </Button>
          </form>

          <div className="mt-6 rounded-md bg-surface-2 p-3 text-xs leading-relaxed text-muted">
            <div className="mb-1 font-medium text-fg">演示账号</div>
            <div>管理员：admin / admin123</div>
            <div>业务：张三、李四 / 12345</div>
            <div>审批：王主管、赵财务、陈总 / 12345</div>
          </div>
        </Card>
      </div>
    </div>
  );
}
