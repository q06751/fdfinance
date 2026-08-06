# 对接老 SQL Server（双跑）

> 目标：新系统与老 ASP.NET WebForms **共用同一财务库**；老程序不改代码也能继续跑。  
> 原则：只加可空列/辅助表，**不改、不删** 原 `F_*` 列与业务语义。

---

## 必做三步（清单）

| 步骤 | 动作 | 说明 |
|---|---|---|
| **1** | 在**原财务库**执行增量 SQL | [`legacy-compatible-additions.sql`](./legacy-compatible-additions.sql) |
| **2** | 配置 `Provider=SqlServer` | `EnsureCreated=false`，连接串指向原库 |
| **3** | 配置 `Password:WriteMode=LegacyMd5` | 双跑改密后老系统仍能登录 |

示例（`backend/src/FdFinance.Api/appsettings.json` 或环境变量覆盖）：

```json
{
  "Database": {
    "Provider": "SqlServer",
    "EnsureCreated": false
  },
  "ConnectionStrings": {
    "FinanceDb": "Server=.;Database=你的财务库;User Id=sa;Password=***;TrustServerCertificate=True;Encrypt=False"
  },
  "Password": {
    "WriteMode": "LegacyMd5"
  },
  "Jwt": {
    "Key": "生产环境用足够长的随机密钥，或设环境变量 FDFINANCE_JWT_KEY"
  },
  "App": {
    "SignatureDir": "D:\\data\\fdfinance\\signatures"
  },
  "Cors": {
    "Origins": [ "https://你的前端域名" ]
  },
  "Swagger": {
    "Enabled": false
  },
  "DingTalk": {
    "Enabled": false,
    "Webhook": "",
    "Secret": "",
    "AppBaseUrl": "https://你的前端域名"
  }
}
```

也可复制模板：[`appsettings.SqlServer.example.json`](../src/FdFinance.Api/appsettings.SqlServer.example.json)。

### 环境变量（可选）

| 变量 | 用途 |
|---|---|
| `FDFINANCE_JWT_KEY` | 覆盖 JWT 签名密钥 |
| `FDFINANCE_SIGNATURE_DIR` | 覆盖手写签名落盘目录 |
| `ConnectionStrings__FinanceDb` | ASP.NET Core 标准连接串覆盖 |
| `Database__Provider` | 设为 `SqlServer` |
| `Password__WriteMode` | 设为 `LegacyMd5` |

---

## 1. 增量 SQL 做了什么

脚本：`backend/scripts/legacy-compatible-additions.sql`

- 业务表增加可空 **`ExtensionJson`**
- `T_Admin` 可空 **`F_Role`**
- 可选新建 **`SerialCounter`**（流水号兜底）
- 可选新建 **`Department`**（组织编码/主数据；钉钉部门可另同步灌入）
- **不**动 `F_IsSend` / `F_IsStatus` / `F_IsN` / `F_Status` / `F_LoginNmae` 等
- **不**改 `T_Report`（打印次数与老 `Peradd` 共用）

老 WebForms 不写新列即可继续 INSERT/UPDATE。

---

## 2. 配置语义

| 项 | 值 | 说明 |
|---|---|---|
| `Database:Provider` | **`SqlServer`** | 接老库 |
| `Database:EnsureCreated` | **`false`** | 禁止自动建库改结构 |
| Seed 演示数据 | 自动跳过 | SqlServer 下不灌 demo |
| `Password:WriteMode` | **`LegacyMd5`** | 写入 32 位大写 MD5（对齐 `DEncrypt.GetMD5Encript` / FormsAuthentication） |
| 密码校验 | 始终兼容 | BCrypt · **无前缀 32 位 MD5** · `md5:` 前缀 · 明文 |
| 管理员 | `F_IsUP=1` 或 `F_Role=admin` | 与老字段兼容 |
| 打印 | `T_Report` | 新老共用 |
| 流水号 | `SerialCounter` 或扫 `F_Code` | 老存储过程可继续用 |

---

## 3. 密码双跑策略

| 场景 | WriteMode | 说明 |
|---|---|---|
| **新老同库双跑** | **`LegacyMd5`** | 新系统创建/改密后，老 WebForms 仍能验密 |
| 仅新系统（切完库） | `Bcrypt` | 更安全；老端无法验 BCrypt |
| 登录读库 | 任意 | 登录**始终**能验老 MD5 与 BCrypt |

切库完成后若不再跑老程序，可改为 `Bcrypt`（仅影响**新写入**的密码；存量 MD5 仍可登录）。

---

## 4. 上线前自检

1. 增量 SQL 在目标库执行成功（可重复执行）  
2. API 启动后 `GET /api/health` 显示 `"database":"SqlServer"`、`"passwordWriteMode":"LegacyMd5"`  
3. 用**老账号**登录新前端成功  
4. 新系统改密后，老 WebForms 仍能登录（WriteMode=LegacyMd5 时）  
5. 新建一张报销草稿 → 设审批链 → 提交 → 签字，库内 `F_IsSend` / `F_IsN` / `F_Status` 与老逻辑一致  
6. 打印一次，`T_Report.Count` 递增  
7. 生产：关 Swagger、收紧 CORS、注入 JWT 密钥、签名目录可写  

---

## 5. 钉钉（可选）

`DingTalk:Enabled=true` + 群机器人 Webhook。  
加签机器人填 `Secret`。  
`AppBaseUrl` 用于拼待办链接；未配时只打日志，业务不中断。

---

## 6. 与演示库（Sqlite）的区别

| | Sqlite 演示 | SqlServer 双跑 |
|---|---|---|
| Provider | `Sqlite` | `SqlServer` |
| EnsureCreated | `true` | **`false`** |
| Seed | 写入 demo 账号/单据 | **不写入** |
| WriteMode | `Bcrypt` | **`LegacyMd5`** |
| 数据 | `/workspace/data/fdfinance.db` | 原财务库连接串 |
