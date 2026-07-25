using ShareYourRide.Application.DTOs.Wallet;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task TopUpAsync(Guid userId, TopUpDto dto)
        {
            var wallet = await GetWalletAsync(userId);

            wallet.Balance += dto.Amount;
            _unitOfWork.Wallets.Update(wallet);

            await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = dto.Amount,
                Type = TransactionType.TopUp
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<WalletBalanceDto> GetBalanceAsync(Guid userId)
        {
            var wallet = await GetWalletAsync(userId);

            return new WalletBalanceDto { Balance = wallet.Balance };
        }

        public async Task<IReadOnlyList<WalletTransactionDto>> GetTransactionsAsync(Guid userId)
        {
            var wallet = await GetWalletAsync(userId);
            var transactions = await _unitOfWork.WalletTransactions.FindAsync(t => t.WalletId == wallet.Id);

            return transactions
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new WalletTransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type,
                    CreatedAt = t.CreatedAt
                }).ToList();
        }

        private async Task<Wallet> GetWalletAsync(Guid userId)
        {
            return await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.UserId == userId)
                ?? throw new InvalidOperationException("Balans hesabı tapılmadı.");
        }
    }
}
