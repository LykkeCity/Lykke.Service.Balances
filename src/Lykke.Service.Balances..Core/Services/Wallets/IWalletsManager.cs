using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.Core.Services.Wallets
{
    public interface IWalletsManager
    {
        Task<IReadOnlyList<IWallet>> GetAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task UpdateBalanceAsync(string walletId, IEnumerable<(string Asset, decimal Balance, decimal Reserved)> assetBalances);
        Task CacheItAsync(string walletId);
        Task<IReadOnlyList<IWallet>> GetTotalBalancesAsync();
        Task UpdateTotalBalancesAsync(List<Wallet> totalBalances);
    }
}
