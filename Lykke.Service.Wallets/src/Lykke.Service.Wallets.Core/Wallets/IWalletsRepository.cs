using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Wallets.Core.Wallets
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string clientId);
        Task<IWallet> GetAsync(string clientId, string assetId);
        Task UpdateBalanceAsync(string clientId, string assetId, double balance);
        Task<Dictionary<string, double>> GetTotalBalancesAsync();

        Task GetWalletsByChunkAsync(Func<IEnumerable<KeyValuePair<string, IEnumerable<IWallet>>>, Task> chunk);
    }
}
