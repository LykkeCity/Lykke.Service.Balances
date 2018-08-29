using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services;

namespace Lykke.Service.Balances.Services
{
    public class TotalBalancesService : ITotalBalancesService
    {
        private static string GetTotalBalancesCacheKey() => ":totalBalances";
        public Task<IReadOnlyList<IWallet>> GetTotalBalancesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IWallet> GetTotalAssetBalanceAsync(string assetId)
        {
            throw new NotImplementedException();
        }

        public Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber)
        {
            // todo: save delta to Redis
            throw new NotImplementedException();
        }
    }
}
