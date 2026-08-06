using FdFinance.Application.Common;
using FdFinance.Application.DTOs;

namespace FdFinance.Application.Interfaces;

public interface IAdminService
{
    Task<AdminOverviewDto> GetOverviewAsync(CancellationToken ct = default);

    Task<IReadOnlyList<AdminAccountDto>> ListAccountsAsync(CancellationToken ct = default);
    Task<ApiResult<string>> CreateAccountAsync(CreateAdminRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> UpdateAccountAsync(string adminId, UpdateAdminRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> ResetPasswordAsync(string adminId, ResetPasswordRequest req, CancellationToken ct = default);

    Task<ApiResult<int>> CreateDepartmentAsync(UpsertDepartmentRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> UpdateDepartmentAsync(int depId, UpsertDepartmentRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> DeleteDepartmentAsync(int depId, CancellationToken ct = default);

    Task<ApiResult<int>> CreateCategoryAsync(UpsertCategoryRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> UpdateCategoryAsync(int id, UpsertCategoryRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> DeleteCategoryAsync(int id, CancellationToken ct = default);

    Task<ApiResult<Guid>> CreateApproverAsync(UpsertApproverRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> UpdateApproverAsync(Guid userId, UpsertApproverRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> DeleteApproverAsync(Guid userId, CancellationToken ct = default);
}
