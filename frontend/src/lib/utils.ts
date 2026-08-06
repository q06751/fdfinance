import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatMoney(value: string | number | null | undefined) {
  const n = typeof value === "number" ? value : parseFloat(String(value ?? "0"));
  if (Number.isNaN(n)) return "¥0.00";
  return new Intl.NumberFormat("zh-CN", {
    style: "currency",
    currency: "CNY",
    minimumFractionDigits: 2,
  }).format(n);
}

export function formatDate(value?: string | Date | null) {
  if (!value) return "—";
  const d = typeof value === "string" ? new Date(value) : value;
  if (Number.isNaN(d.getTime())) return "—";
  return d.toLocaleDateString("zh-CN", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
}

export function statusTone(status: string) {
  switch (status) {
    case "draft":
      return "bg-surface-2 text-muted";
    case "inapproval":
      return "bg-warning-soft text-warning";
    case "approved":
      return "bg-success-soft text-success";
    case "voided":
      return "bg-danger-soft text-danger";
    default:
      return "bg-surface-2 text-muted";
  }
}
