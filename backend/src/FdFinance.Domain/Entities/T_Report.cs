namespace FdFinance.Domain.Entities;

/// <summary>
/// 原表 T_Report — 打印次数。新老系统共用，勿改字段语义。
/// </summary>
public class T_Report
{
    public string ReportId { get; set; } = string.Empty;
    public string? F_ReimbursementId { get; set; }
    public int Count { get; set; }
}
