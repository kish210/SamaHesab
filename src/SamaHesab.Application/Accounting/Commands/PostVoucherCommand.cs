using MediatR;
using SamaHesab.Application.Common.Interfaces;
using SamaHesab.Application.Common.Models;
using SamaHesab.Domain.Interfaces.Repositories;

namespace SamaHesab.Application.Accounting.Commands;

public record PostVoucherCommand(int VoucherId) : IRequest<Result>;

public class PostVoucherCommandHandler : IRequestHandler<PostVoucherCommand, Result>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public PostVoucherCommandHandler(IVoucherRepository voucherRepository,
        IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(PostVoucherCommand request, CancellationToken ct)
    {
        try
        {
            var voucher = await _voucherRepository.GetWithItemsAsync(request.VoucherId, ct);
            if (voucher == null) return Result.Failure("سند یافت نشد.");
            if (voucher.CompanyId != _currentUser.CompanyId)
                return Result.Failure("دسترسی غیرمجاز.");

            voucher.Post(_currentUser.UserId!.Value);
            _voucherRepository.Update(voucher);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
