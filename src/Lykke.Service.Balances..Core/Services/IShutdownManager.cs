using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}