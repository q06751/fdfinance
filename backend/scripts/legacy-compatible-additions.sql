-- ============================================================
-- 新系统对接老库：仅「增量」变更，不删不改原列/原表语义
-- 老 WebForms 继续可跑：不写这些新列/新表即可
-- SQL Server
-- ============================================================

-- 1) 业务表可空扩展列（老 INSERT 不写也能成功）
IF COL_LENGTH('dbo.T_Reimbursement', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Reimbursement ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Abstract', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Abstract ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Sign', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Sign ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Admin', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Admin ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Admin', 'F_Role') IS NULL
  ALTER TABLE dbo.T_Admin ADD F_Role nvarchar(32) NULL;

IF COL_LENGTH('dbo.T_User', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_User ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Category', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Category ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Apply', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Apply ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Autograph', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Autograph ADD ExtensionJson nvarchar(max) NULL;

IF COL_LENGTH('dbo.T_Relation', 'ExtensionJson') IS NULL
  ALTER TABLE dbo.T_Relation ADD ExtensionJson nvarchar(max) NULL;

-- 2) 打印次数：沿用原表 T_Report（ReportId / F_ReimbursementId / Count）
--    勿改；新系统与旧 Peradd 共用

-- 3) 可选：新系统流水号兜底表（老系统仍可用 GetSerialNo 存储过程）
IF OBJECT_ID(N'dbo.SerialCounter', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.SerialCounter (
    Code   nvarchar(16) NOT NULL,
    [Year] int NOT NULL,
    [Month] int NOT NULL,
    Sequence int NOT NULL CONSTRAINT DF_SerialCounter_Seq DEFAULT(0),
    CONSTRAINT PK_SerialCounter PRIMARY KEY (Code, [Year], [Month])
  );
END

-- 4) 组织表 Department（新系统主数据/流水号前缀用）
--    若老环境部门在钉钉库，可不同库同步或手工 INSERT；本表不替代钉钉源
IF OBJECT_ID(N'dbo.Department', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.Department (
    DepId int NOT NULL,
    DName nvarchar(256) NULL,
    ClassCode nvarchar(16) NULL,
    ParentId int NOT NULL CONSTRAINT DF_Department_Parent DEFAULT(0),
    IsDelete int NOT NULL CONSTRAINT DF_Department_Del DEFAULT(0),
    ExtensionJson nvarchar(max) NULL,
    CONSTRAINT PK_Department PRIMARY KEY (DepId)
  );
END

-- 说明：
-- * 不修改 F_IsSend / F_IsStatus / F_IsN / F_Status / F_LoginNmae 等原字段
-- * 不删除、不重命名任何老列
-- * F_IsUP 仍是老管理员标志；新系统写 F_Role 时同步 F_IsUP
-- * 密码：老库多为 32 位 MD5 大写 hex；新系统登录可验 BCrypt / 无前缀 MD5 / 明文
-- * 双跑期改密请 Password:WriteMode=LegacyMd5，否则老系统无法验新密码
