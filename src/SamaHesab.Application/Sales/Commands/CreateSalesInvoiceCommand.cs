using FluentValidation;
using MediatR;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Application.Common.Models;
using SamaHesab.Domain.Entities.Sales;
using SamaHesab.Domain.Enums;
using SamaHesab.Domain.Interfaces.Repositories;

namespace SamaHesab.Application.Sales.Commands;

public record CreateSalesInvoiceCommand(
    int BranchId,
    int FiscalYearId,
    string InvoiceDate,
    int CustomerId,
    int WarehouseId,
    InvoiceType InvoiceType,
    string PriceLevel,
    int? SalesRepId,
    string? DueDate,
    string? Description,
    decimal Shipping,
    decimal OtherCosts,
    List<SalesInvoiceItemDto> Items
) : IRequest<Result<int>>;

public record SalesInvoiceItemDto(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPct,
    decimal TaxPct,
    string? Description,
    int? BatchId,
    int? SerialId
);

public class CreateSalesInvoiceCommandValidator : AbstractValidator<CreateSalesInvoiceCommand>
{
    public CreateSalesInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("تاریخ فاکتور الزامی است.");
        RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("مشتری الزامی است.");
        RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("انبار الزامی است.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("فاکتور باید حداقل یک ردیف داشته باشد.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0).WithMessage("کالا الزامی است.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("مقدار باید بزرگتر از صفر باشد.");
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("قیمت واحد نمی‌تواند منفی باشد.");
        });
    }
}

public class CreateSalesInvoiceCommandHandler : IRequestHandler<CreateSalesInvoiceCommand, Result<int>>
{
    private readonly IRepository<SalesInvoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IPersianCalendarService _calendar;

    public CreateSalesInvoiceCommandHandler(
        IRepository<SalesInvoice> invoiceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IPersianCalendarService calendar)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _calendar = calendar;
    }

    public async Task<Result<int>> Handle(CreateSalesInvoiceCommand request, CancellationToken ct)
    {
        try
        {
            var companyId = _currentUser.CompanyId!.Value;

            // Generate invoice number
            var invoiceNumber = await GenerateInvoiceNumberAsync(companyId, request.FiscalYearId, request.InvoiceType, ct);

            var invoice = SalesInvoice.Create(
                companyId, request.BranchId, request.FiscalYearId,
                invoiceNumber, request.InvoiceDate, request.CustomerId, request.WarehouseId,
                request.InvoiceType, request.PriceLevel, request.SalesRepId,
                request.DueDate, request.Description);

            for (int i = 0; i < request.Items.Count; i++)
            {
                var dto = request.Items[i];
                var item = SalesInvoiceItem.Create(
                    0, i + 1, dto.ProductId, dto.Quantity, dto.UnitPrice,
                    dto.DiscountPct, dto.TaxPct, dto.Description, dto.BatchId, dto.SerialId);
                invoice.AddItem(item);
            }

            invoice.SetShipping(request.Shipping, request.OtherCosts);

            await _invoiceRepository.AddAsync(invoice, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<int>.Success(invoice.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }

    private async Task<string> GenerateInvoiceNumberAsync(int companyId, int fiscalYearId,
        InvoiceType type, CancellationToken ct)
    {
        // Simplified - in real implementation call the SP
        var prefix = type switch
        {
            InvoiceType.Sale => "F",
            InvoiceType.SaleReturn => "BR",
            InvoiceType.Quotation => "PF",
            _ => "F"
        };
        var count = await _invoiceRepository.CountAsync(
            i => i.CompanyId == companyId && i.FiscalYearId == fiscalYearId && i.InvoiceType == type, ct);
        return $"{prefix}{(count + 1):D6}";
    }
}
