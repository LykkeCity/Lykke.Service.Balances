using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.Core.Services.Wallets
{
    public interface ICachedWalletsRepository
    {
        Task<IReadOnlyList<IWallet>> GetAllAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task UpdateBalanceAsync(string walletId, string assetId, decimal balance, decimal reserved, long updateSequenceNumber);
        Task CacheItAsync(string walletId);
    }
}
