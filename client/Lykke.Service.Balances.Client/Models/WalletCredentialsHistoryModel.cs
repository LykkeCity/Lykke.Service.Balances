using System.Collections.Generic;

namespace Lykke.Service.Balances.Client.Models
{
    public class WalletCredentialsHistoryModel : BaseModel
    {
        public IEnumerable<string> WalletsCredentialHistory { get; set; }
    }
}
