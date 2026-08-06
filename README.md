# 复大财务 FdFinance — 现代化重构

将经典 **ASP.NET WebForms + SQL Server** 财务报销系统，重构为：

| 层 | 技术 |
|---|---|
| API | **.NET 10** ASP.NET Core Web API · Clean Architecture · JWT |
| Web | **Next.js 15** App Router · Tailwind v4 · Recharts |
| Cache | **Redis**（不可用时自动回退内存缓存） |
| DB | SQLite（演示）/ 可切换 SQL Server，**表结构字段名保持原样** |

原始代码归档于 [`legacy/fdfinance`](./legacy/fdfinance)。

## 数据结构兼容

原表与主键 **完整保留**（含历史拼写 `F_LoginNmae` / `F_Typt` / `F_ClassTypt` / `F_Descripion`）：

- `T_Reimbursement` / `T_Abstract` / `T_Sign`
- `T_Admin` / `T_User` / `T_Category` / `T_Apply`
- `T_Autograph` / `T_Relation` / `T_Report`

### 扩展方式（不破坏原结构）

每张核心表增加可选列：

```text
ExtensionJson  TEXT  -- JSON 袋，存新增业务字段
```

实体基类 `ExtensibleEntity` 提供 `GetExtension` / `SetExtension`。

`T_Admin.F_Role` 为显式扩展列示例（admin / user / approver）。

---

## 接老库双跑（生产）

**必做三步：**

1. 在原财务库执行增量 SQL：[`backend/scripts/legacy-compatible-additions.sql`](./backend/scripts/legacy-compatible-additions.sql)  
2. 配置 **`Database:Provider=SqlServer`**，**`EnsureCreated=false`**，连接串指向原库  
3. 配置 **`Password:WriteMode=LegacyMd5`**（新系统改密后老 WebForms 仍能登录）

完整说明、环境变量、上线自检见：

**[`backend/scripts/connect-legacy-sqlserver.md`](./backend/scripts/connect-legacy-sqlserver.md)**

配置模板：[`backend/src/FdFinance.Api/appsettings.SqlServer.example.json`](./backend/src/FdFinance.Api/appsettings.SqlServer.example.json)

| 场景 | WriteMode |
|---|---|
| 新老同库双跑 | **`LegacyMd5`** |
| 仅新系统 | `Bcrypt` |
| 登录校验 | 始终支持老 MD5 + BCrypt |

---

## 安全模型

- JWT 鉴权；密码按 `WriteMode` 写入（演示默认 BCrypt）
- 登录可验：BCrypt · 无前缀 32 位 MD5 · `md5:` · 明文
- **部门数据隔离**：非管理员仅可访问本部门 `F_Depid` 单据
- **审批权限**：仅节点指定审批人可批（管理员可代批）
- 已提交单据禁止改审批人 / 重复提交；已提交不可软删（请先作废）
- 合并：同类型、同制单人、待提交；金额先汇总再写 host

## 本地启动（演示 Sqlite）

```bash
sh /workspace/startup.sh
```

## 演示账号

| 账号 | 密码 | 角色 |
|---|---|---|
| admin | admin123 | 管理员（全量数据） |
| 张三 | 12345 | 业务员（复大医院） |
| 李四 | 12345 | 业务员（中大医院） |
| 王主管 / 赵财务 / 陈总 | 12345 | 审批人 |

## API 概览

- `POST /api/auth/login` · `POST /api/auth/change-password`
- `GET/POST /api/reimbursements` · `PUT {id}` · 审批链 / 提交 / 作废 / 合并 / 打印
- `GET /api/reimbursements/dashboard` · `approvals`
- `GET /api/masterdata/*` · `api/admin/*`
- `GET /api/health` — 含 Redis、database、passwordWriteMode

## 工作流语义（与原系统一致）

| 字段 | 含义 |
|---|---|
| `F_IsSend=0` | 待提交 |
| `F_IsSend=1` | 已提交 |
| `F_IsStatus=1` | 已作废 |
| `T_Sign.F_IsN=0` | 排队等待 |
| `T_Sign.F_IsN=1` | 已审批 |
| `T_Sign.F_IsN=2` | 当前待批 |
| `T_Sign.F_Status` | A 待签 / D 我已签 / B 完成 / C 作废 |

## 自动化测试（TDD 先红后绿）

```bash
# 全量
dotnet test backend/tests/FdFinance.Tests/FdFinance.Tests.csproj

# 守卫：绿 → 故意破坏 MD5 必须红 → 恢复再绿（防幻觉）
bash backend/scripts/run-tests-tdd.sh
```

说明见 [`backend/tests/FdFinance.Tests/README.md`](./backend/tests/FdFinance.Tests/README.md)。

## 完整源码包

若个别大文件尚未全部展开到目录树，可用归档还原：

```bash
bash scripts/extract-full-source.sh
```

（需 `archive/c*.b64` 全部到位；或克隆后对照本仓库持续同步的源码目录。）
