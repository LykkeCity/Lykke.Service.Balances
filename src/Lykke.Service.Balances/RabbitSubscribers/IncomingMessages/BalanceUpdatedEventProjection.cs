using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.MatchingEngine.Contracts.Balances;

namespace Lykke.Service.Balances.RabbitSubscribers.IncomingMessages
{
    /// <remarks>
    /// Local projection of the <see cref="BalanceUpdatedEvent"/> without unnecessary fields, 
    /// to reduce coupling with the contract
    /// </remarks>
    [UsedImplicitly]
    public class BalanceUpdatedEventProjection
    {
        public IReadOnlyList<ClientBalanceUpdateModel> Balances { get; set; }
    }
}