using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.Core.Services.Wallets
{
    public interface IWalletsManager
    {
        Task<IReadOnlyList<IWallet>> GetAsync(string clientId);
        Task<IWallet> GetAsync(string clientId, string assetId);
        Task UpdateBalanceAsync(string clientId, string asset, double balance, double reserved);
        Task CacheItAsync(string clientId);
    }
}