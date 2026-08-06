"use client";

import { DocListPage } from "@/components/DocListPage";

export default function ReceiptsPage() {
  return (
    <DocListPage
      typt={3}
      description="收款单据独立管理（F_Typt=3），审批流与打印与报销共用"
    />
  );
}
