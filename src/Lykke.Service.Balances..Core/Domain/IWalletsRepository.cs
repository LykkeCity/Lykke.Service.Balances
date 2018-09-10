using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain
{
    public interface IWalletsRepository
    {
        Task<IEnumerable<IWallet>> GetAsync(string walletId);
        Task<IWallet> GetAsync(string walletId, string assetId);
        Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet, long updateSequenceNumber);
    }
}
