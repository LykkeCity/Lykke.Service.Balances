using System.Collections.Generic;

namespace Lykke.Service.Balances.Models.ClientBalances
{
    public class TotalBalancesResponseModel
    {
        public Dictionary<string, decimal> Balances { get; set; }
    }
}
