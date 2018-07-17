using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task<IEnumerable<IWallet>> GetTotalBalancesAsync();
        Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet);
    }
}
