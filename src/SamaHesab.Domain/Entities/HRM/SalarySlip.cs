using SamaHesab.Domain.Common;

namespace SamaHesab.Domain.Entities.HRM;

public class SalarySlip : BaseEntity
{
    public int EmployeeId { get; private set; }
    public string PeriodYear { get; private set; } = default!;
    public byte PeriodMonth { get; private set; }
    public decimal BaseSalary { get; private set; }
    public decimal OvertimePay { get; private set; }
    public decimal Allowances { get; private set; }
    public decimal Commission { get; private set; }
    public decimal Bonuses { get; private set; }
    public decimal GrossSalary { get; private set; }
    public decimal InsuranceDeduct { get; private set; }
    public decimal TaxDeduct { get; private set; }
    public decimal OtherDeductions { get; private set; }
    public decimal NetSalary { get; private set; }
    public bool IsPaid { get; private set; }
    public string? PaidDate { get; private set; }
    public int? VoucherId { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    private SalarySlip() { }

    public static SalarySlip Create(int employeeId, string periodYear, byte periodMonth,
        decimal baseSalary, decimal overtimePay = 0, decimal allowances = 0,
        decimal commission = 0, decimal bonuses = 0,
        decimal insuranceDeduct = 0, decimal taxDeduct = 0, decimal otherDeductions = 0,
        string? notes = null)
    {
        var gross = baseSalary + overtimePay + allowances + commission + bonuses;
        var net = gross - insuranceDeduct - taxDeduct - otherDeductions;

        return new SalarySlip
        {
            EmployeeId = employeeId,
            PeriodYear = periodYear,
            PeriodMonth = periodMonth,
            BaseSalary = baseSalary,
            OvertimePay = overtimePay,
            Allowances = allowances,
            Commission = commission,
            Bonuses = bonuses,
            GrossSalary = gross,
            InsuranceDeduct = insuranceDeduct,
            TaxDeduct = taxDeduct,
            OtherDeductions = otherDeductions,
            NetSalary = net,
            Notes = notes
        };
    }

    public void MarkAsPaid(string paidDate, int? voucherId = null)
    {
        IsPaid = true;
        PaidDate = paidDate;
        VoucherId = voucherId;
    }
}
