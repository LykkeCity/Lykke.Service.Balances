using Lykke.Service.Balances.Core.Domain.Wallets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Services
{
    public interface ITotalBalancesService
    {
        Task<IReadOnlyList<IWallet>> GetTotalBalancesAsync();
        Task<IWallet> GetTotalAssetBalanceAsync(string assetId);
        Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber);
    }
}
