using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain;

namespace Lykke.Service.Balances.Core.Services
{
    public interface IBalanceSnapshotRepository : IRepository<BalanceSnapshot>
    {
        Task<BalanceSnapshot> GetSnapshot(string walletId, string assetId, DateTime timestamp);
        Task<List<BalanceSnapshot>> GetSnapshots(string assetId, DateTime timestamp);
    }
}
