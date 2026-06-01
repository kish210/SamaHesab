using SamaHesab.Domain.Common;
using SamaHesab.Domain.Enums;

namespace SamaHesab.Domain.Entities.Sales;

public class SalesPayment : BaseEntity
{
    public int InvoiceId { get; private set; }
    public string PaymentDate { get; private set; } = default!;
    public PaymentType PaymentType { get; private set; }
    public decimal Amount { get; private set; }
    public int? BankAccountId { get; private set; }
    public int? ChequeId { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? Description { get; private set; }
    public int? VoucherId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    private SalesPayment() { }

    public static SalesPayment Create(int invoiceId, string paymentDate, PaymentType paymentType,
        decimal amount, int? bankAccountId = null, int? chequeId = null,
        string? referenceNumber = null, string? description = null)
    {
        if (amount <= 0) throw new ArgumentException("مبلغ پرداخت باید بزرگتر از صفر باشد.");

        return new SalesPayment
        {
            InvoiceId = invoiceId,
            PaymentDate = paymentDate,
            PaymentType = paymentType,
            Amount = amount,
            BankAccountId = bankAccountId,
            ChequeId = chequeId,
            ReferenceNumber = referenceNumber,
            Description = description
        };
    }

    public void SetVoucher(int voucherId) => VoucherId = voucherId;
}
