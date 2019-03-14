using System;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain;

namespace Lykke.Service.Balances.Core.Services
{
    public interface IBalanceSnapshotRepository : IRepository<BalanceSnapshot>
    {
        Task<BalanceSnapshot> GetSnapshot(string walletId, string assetId, DateTime timestamp);
    }
}
