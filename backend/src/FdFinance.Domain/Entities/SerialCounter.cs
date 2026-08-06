namespace FdFinance.Domain.Entities;

/// <summary>Replaces legacy stored procedure GetSerialNo.</summary>
public class SerialCounter
{
    public string Code { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int Sequence { get; set; }
}
