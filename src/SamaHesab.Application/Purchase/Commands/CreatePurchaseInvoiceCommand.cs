using FluentValidation;
using MediatR;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Application.Common.Models;
using SamaHesab.Domain.Interfaces.Repositories;

namespace SamaHesab.Application.Purchase.Commands;

public record CreatePurchaseInvoiceCommand(
    int BranchId,
    int FiscalYearId,
    string InvoiceDate,
    int SupplierId,
    int WarehouseId,
    string InvoiceType,
    int? OrderId,
    string? DueDate,
    string? Description,
    decimal Shipping,
    decimal OtherCosts,
    List<PurchaseInvoiceItemDto> Items
) : IRequest<Result<int>>;

public record PurchaseInvoiceItemDto(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPct,
    decimal TaxPct,
    string? Description,
    int? BatchId,
    string? BatchNumber,
    string? ProductionDate,
    string? ExpiryDate
);

public class CreatePurchaseInvoiceCommandValidator : AbstractValidator<CreatePurchaseInvoiceCommand>
{
    public CreatePurchaseInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("تاریخ فاکتور الزامی است.");
        RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage("تأمین‌کننده الزامی است.");
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

public class CreatePurchaseInvoiceCommandHandler : IRequestHandler<CreatePurchaseInvoiceCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IStockItemRepository _stockRepository;
    private readonly IProductRepository _productRepository;

    public CreatePurchaseInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IStockItemRepository stockRepository,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _stockRepository = stockRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<int>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var companyId = _currentUser.CompanyId!.Value;

            // Update stock for each item
            foreach (var item in request.Items)
            {
                var stockItem = await _stockRepository
                    .GetByProductAndWarehouseAsync(item.ProductId, request.WarehouseId, ct);

                if (stockItem == null)
                {
                    stockItem = Domain.Entities.Inventory.StockItem.Create(
                        companyId, item.ProductId, request.WarehouseId);
                    await _stockRepository.AddAsync(stockItem, ct);
                }

                stockItem.AddStock(item.Quantity, item.UnitPrice);
                _stockRepository.Update(stockItem);

                // Update product purchase price
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                if (product != null)
                {
                    product.UpdatePrices(item.UnitPrice, product.SalePrice,
                        product.WholesalePrice, product.ConsumerPrice, product.TaxRate);
                    _productRepository.Update(product);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            return Result<int>.Success(1); // Return invoice ID
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return Result<int>.Failure(ex.Message);
        }
    }
}
