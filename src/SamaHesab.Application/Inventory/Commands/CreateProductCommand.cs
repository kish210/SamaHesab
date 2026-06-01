using FluentValidation;
using MediatR;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Application.Common.Models;
using SamaHesab.Domain.Entities.Inventory;
using SamaHesab.Domain.Enums;
using SamaHesab.Domain.Interfaces.Repositories;

namespace SamaHesab.Application.Inventory.Commands;

public record CreateProductCommand(
    string Code,
    string? Barcode,
    string Name,
    string? NameEn,
    int? GroupId,
    int? BrandId,
    int UnitId,
    ProductType ProductType,
    decimal PurchasePrice,
    decimal SalePrice,
    decimal WholesalePrice,
    decimal ConsumerPrice,
    decimal MinStock,
    decimal? MaxStock,
    bool HasSerial,
    bool HasBatch,
    bool HasExpiry,
    ValuationMethod ValuationMethod,
    decimal TaxRate,
    string? Description
) : IRequest<Result<int>>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("کد کالا الزامی است.")
            .MaximumLength(30).WithMessage("کد کالا حداکثر ۳۰ کاراکتر.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("نام کالا الزامی است.")
            .MaximumLength(300).WithMessage("نام کالا حداکثر ۳۰۰ کاراکتر.");
        RuleFor(x => x.UnitId).GreaterThan(0).WithMessage("واحد اندازه‌گیری الزامی است.");
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0).WithMessage("قیمت فروش نمی‌تواند منفی باشد.");
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage("قیمت خرید نمی‌تواند منفی باشد.");
        RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0).WithMessage("حداقل موجودی نمی‌تواند منفی باشد.");
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100).WithMessage("نرخ مالیات باید بین ۰ تا ۱۰۰ باشد.");
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        try
        {
            var companyId = _currentUser.CompanyId!.Value;

            var existing = await _productRepository.GetByCodeAsync(companyId, request.Code, ct);
            if (existing != null) return Result<int>.Failure("کد کالا تکراری است.");

            if (!string.IsNullOrWhiteSpace(request.Barcode))
            {
                var byBarcode = await _productRepository.GetByBarcodeAsync(companyId, request.Barcode, ct);
                if (byBarcode != null) return Result<int>.Failure("بارکد تکراری است.");
            }

            var product = Product.Create(companyId, request.Code, request.Name, request.UnitId,
                request.SalePrice, request.PurchasePrice, request.ProductType);

            product.UpdateDetails(request.Name, request.NameEn, request.GroupId, request.BrandId,
                request.Barcode, null, request.Description);
            product.UpdatePrices(request.PurchasePrice, request.SalePrice,
                request.WholesalePrice, request.ConsumerPrice, request.TaxRate);
            product.SetStockLimits(request.MinStock, request.MaxStock, null);
            product.SetTrackingOptions(request.HasSerial, request.HasBatch,
                request.HasExpiry, request.ValuationMethod);

            await _productRepository.AddAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<int>.Success(product.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex.Message);
        }
    }
}
