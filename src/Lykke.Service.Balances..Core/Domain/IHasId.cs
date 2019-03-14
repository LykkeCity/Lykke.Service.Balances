using System;

namespace Lykke.Service.Balances.Core.Domain
{
    public interface IHasId
    {
        Guid Id { get; set; }
    }
}
