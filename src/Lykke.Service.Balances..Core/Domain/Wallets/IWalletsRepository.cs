using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string clientId);
        Task<Dictionary<string, double>> GetTotalBalancesAsync();

        Task GetWalletsByChunkAsync(Func<IEnumerable<KeyValuePair<string, IEnumerable<IWallet>>>, Task> chunk);
    }
}
