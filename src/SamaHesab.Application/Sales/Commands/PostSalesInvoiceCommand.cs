using MediatR;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Application.Common.Models;
using SamaHesab.Domain.Entities.Sales;
using SamaHesab.Domain.Interfaces.Repositories;

namespace SamaHesab.Application.Sales.Commands;

public record PostSalesInvoiceCommand(int InvoiceId) : IRequest<Result>;

public class PostSalesInvoiceCommandHandler : IRequestHandler<PostSalesInvoiceCommand, Result>
{
    private readonly IRepository<SalesInvoice> _invoiceRepository;
    private readonly IStockItemRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public PostSalesInvoiceCommandHandler(
        IRepository<SalesInvoice> invoiceRepository,
        IStockItemRepository stockRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _invoiceRepository = invoiceRepository;
        _stockRepository = stockRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(PostSalesInvoiceCommand request, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var invoice = await _invoiceRepository.FindSingleAsync(
                i => i.Id == request.InvoiceId, ct);

            if (invoice == null) return Result.Failure("فاکتور یافت نشد.");
            if (invoice.CompanyId != _currentUser.CompanyId)
                return Result.Failure("دسترسی غیرمجاز.");

            // Check and deduct stock for each item
            foreach (var item in invoice.Items)
            {
                var stockItem = await _stockRepository
                    .GetByProductAndWarehouseAsync(item.ProductId, invoice.WarehouseId, ct);

                if (stockItem == null || stockItem.Quantity < item.Quantity)
                    return Result.Failure($"موجودی کافی برای کالا {item.ProductId} وجود ندارد.");

                var cost = stockItem.RemoveStock(item.Quantity);
                item.SetCostAndProfit(cost);
                _stockRepository.Update(stockItem);
            }

            // Mark invoice as posted (voucherId=0 placeholder; real impl creates accounting voucher)
            invoice.Post(_currentUser.UserId!.Value, 0);
            _invoiceRepository.Update(invoice);

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return Result.Failure(ex.Message);
        }
    }
}
