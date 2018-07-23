using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWalletCredentialsRepository
    {
        Task<IWalletCredentials> GetAsync(string clientId);
    }
}
