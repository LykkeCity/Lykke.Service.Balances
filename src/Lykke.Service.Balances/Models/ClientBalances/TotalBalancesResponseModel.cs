using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lykke.Service.Balances.Models.ClientBalances
{
    public class  TotalBalancesResponseModel
    {
        public IReadOnlyCollection<TotalAssetBalance> Balances { get; set; }

        public TotalBalancesResponseModel(Dictionary<string, decimal> balances)
        {
            Balances = new ReadOnlyCollection<TotalAssetBalance>(balances.Select(item =>
                new TotalAssetBalance {AssetId = item.Key, Balance = item.Value}).ToList());
        }
    }
}
