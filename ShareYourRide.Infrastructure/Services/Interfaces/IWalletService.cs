using ShareYourRide.Application.DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IWalletService
    {
        Task TopUpAsync(Guid userId, TopUpDto dto);
        Task<WalletBalanceDto> GetBalanceAsync(Guid userId);
        Task<IReadOnlyList<WalletTransactionDto>> GetTransactionsAsync(Guid userId);

    }
}
