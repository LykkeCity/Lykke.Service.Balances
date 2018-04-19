using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task<Dictionary<string, decimal>> GetTotalBalancesAsync();
        Task UpdateBalanceAsync(string walletId, List<IWallet> wallets);
        
    }
}
