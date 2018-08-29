using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Service.Balances.Core.Services;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Workflow.Handlers
{
    public class TotalBalanceCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ITotalBalancesService _totalBalancesService;

        public TotalBalanceCommandHandler(IChaosKitty chaosKitty, ITotalBalancesService totalBalancesService)
        {
            _chaosKitty = chaosKitty;
            _totalBalancesService = totalBalancesService;
        }

        public async Task<CommandHandlingResult> Handle(Commands.UpdateTotalBalanceCommand command)
        {
            await _totalBalancesService.ChangeTotalBalanceAsync(command.AssetId, command.DeltaBalance,
                command.SequenceNumber);

            _chaosKitty.Meow("Problem with Redis");

            return CommandHandlingResult.Ok();
        }
    }
}
