﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.MatchingEngine.Connector.Models.Events;
using Lykke.Service.Balances.Client.Events;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Services;

namespace Lykke.Service.Balances.Workflow.Projections
{
    public class BalancesUpdateProjection
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly IChaosKitty _chaosKitty;

        public BalancesUpdateProjection(
            [NotNull] ICachedWalletsRepository cachedWalletsRepository,
            [NotNull] IChaosKitty chaosKitty)
        {
            _cachedWalletsRepository = cachedWalletsRepository ?? throw new ArgumentNullException(nameof(cachedWalletsRepository));
            _chaosKitty = chaosKitty ?? throw new ArgumentNullException(nameof(chaosKitty));
        }

        public async Task Handle(BalanceUpdatedEvent evt)
        {
            var operation = TelemetryHelper.InitTelemetryOperation($"Processing {nameof(BalanceUpdatedEvent)} message", evt.WalletId);
            await _cachedWalletsRepository.UpdateBalanceAsync(
                evt.WalletId,
                evt.AssetId,
                decimal.Parse(evt.Balance),
                decimal.Parse(evt.Reserved),
                evt.SequenceNumber,
                evt.Timestamp);

            _chaosKitty.Meow("Problem with Azure Table Storage");

            TelemetryHelper.SubmitOperationResult(operation);
        }
    }
}
