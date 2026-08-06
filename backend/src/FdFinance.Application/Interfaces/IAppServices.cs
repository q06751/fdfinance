using FdFinance.Application.Common;
using FdFinance.Application.DTOs;
using FdFinance.Domain.Entities;

namespace FdFinance.Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
    bool IsRedisConnected { get; }
}

public interface IAuthService
{
    Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<T_Admin?> GetAdminAsync(string adminId, CancellationToken ct = default);
    Task<ApiResult<bool>> ChangePasswordAsync(string adminId, ChangePasswordRequest req, CancellationToken ct = default);
}

public interface IReimbursementService
{
    Task<PagedResult<ReimbursementListItemDto>> ListAsync(
        string? keyword, string? status, int? category, int page, int pageSize,
        ActorContext actor, int? typt = 1, CancellationToken ct = default);

    Task<ReimbursementDetailDto?> GetAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<Guid>> CreateAsync(CreateReimbursementRequest req, T_Admin actor, CancellationToken ct = default);
    Task<ApiResult<bool>> UpdateAsync(Guid id, UpdateReimbursementRequest req, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> SoftDeleteAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> VoidAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> AddApproverAsync(Guid id, AddApproverRequest req, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> SetApproverChainAsync(Guid id, SetApproverChainRequest req, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> ClearApproversAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> SubmitAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> ApproveAsync(Guid reimbursementId, ActorContext actor, ApproveSignRequest req, CancellationToken ct = default);
    Task<ApiResult<bool>> ApproveOneAsync(Guid signId, ActorContext actor, ApproveSignRequest? req, CancellationToken ct = default);
    Task<ApiResult<bool>> MergeAsync(MergeReimbursementsRequest req, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> SplitAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<bool>> RejectAsync(Guid id, ActorContext actor, RejectRequest req, CancellationToken ct = default);
    Task<ApprovalWorkspaceDto?> GetApprovalWorkspaceAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<ApiResult<int>> RecordPrintAsync(Guid id, ActorContext actor, CancellationToken ct = default);
    Task<DashboardStatsDto> GetDashboardAsync(ActorContext actor, CancellationToken ct = default);
    Task<IReadOnlyList<ApprovalTaskDto>> ListApprovalsAsync(ActorContext actor, string tab, CancellationToken ct = default);
    Task<ApprovalCountsDto> GetApprovalCountsAsync(ActorContext actor, CancellationToken ct = default);
}

public interface IMasterDataService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(string? code = null, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ApplyDto>> GetAppliesAsync(CancellationToken ct = default);
    Task<ApiResult<Guid>> CreateApplyAsync(CreateApplyRequest req, CancellationToken ct = default);
}
