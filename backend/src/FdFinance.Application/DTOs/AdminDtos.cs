namespace FdFinance.Application.DTOs;

public record AdminAccountDto(
    string AdminId,
    string? LoginName,
    string? DepartmentName,
    int DepId,
    string? Role,
    int IsUp);

public record CreateAdminRequest(
    string LoginName,
    string Password,
    string? DepartmentName,
    int DepId,
    string Role = "user");

public record UpdateAdminRequest(
    string? DepartmentName,
    int? DepId,
    string? Role);

public record ResetPasswordRequest(string NewPassword);

public record ChangePasswordRequest(string OldPassword, string NewPassword);

public record UpsertDepartmentRequest(
    string Name,
    string ClassCode,
    int ParentId = 0);

public record UpsertCategoryRequest(
    string Name,
    string Code);

public record UpsertApproverRequest(
    string Name,
    string? Phone,
    string? Code);

public record AdminOverviewDto(
    int AccountCount,
    int DepartmentCount,
    int ApproverCount,
    int CategoryCount,
    int ReimbursementTotal,
    int PendingApprovalDocs,
    int DraftDocs,
    int VoidedDocs);
