using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletCredentialsHistoryRepository
    {
        Task InsertHistoryRecord(IWalletCredentials oldWalletCredentials);
        Task<IEnumerable<string>> GetPrevMultisigsForUser(string clientId);
    }
}
