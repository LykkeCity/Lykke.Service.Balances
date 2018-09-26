using Lykke.Service.Balances.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Services
{
    public interface ITotalBalancesService
    {
        Task InitAsync();
        Task<IReadOnlyList<TotalAssetBalance>> GetTotalBalancesAsync();
        Task<TotalAssetBalance> GetTotalAssetBalanceAsync(string assetId);
        Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber);
    }
}
