using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task<IWallet> GetTotalBalanceAsync(string assetId);
        Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet);
        //TODO remove this in next version
        Task InitAssetsWalletsAsync(HashSet<string> assetIds);
    }
}
