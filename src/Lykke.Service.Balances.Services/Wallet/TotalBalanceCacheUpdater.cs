using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services.Wallets;

namespace Lykke.Service.Balances.Services.Wallet
{
    public class TotalBalanceCacheUpdater : TimerPeriod, ITotalBalanceCacheUpdater
    {
        private readonly IWalletsRepository _walletsRepository;
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly IAssetsServiceWithCache _assetsService;

        private HashSet<string> _assetIds;

        public TotalBalanceCacheUpdater(
            IWalletsRepository walletsRepository,
            ICachedWalletsRepository cachedWalletsRepository,
            IAssetsServiceWithCache assetsService,
            ILogFactory logFactory)
            : base(TimeSpan.FromMinutes(30), logFactory)
        {
            _walletsRepository = walletsRepository;
            _cachedWalletsRepository = cachedWalletsRepository;
            _assetsService = assetsService;
        }

        public new void Start()
        {
            Task.Run(() => StartProcessingAsync().GetAwaiter().GetResult());
        }

        public override async Task Execute()
        {
            await _cachedWalletsRepository.UpdateTotalBalancesAsync(_assetIds);
        }

        private async Task StartProcessingAsync()
        {
            var assets = await _assetsService.GetAllAssetsAsync(true);
            _assetIds = assets.Select(a => a.Id).ToHashSet();

            await _walletsRepository.InitAssetsWalletsAsync(_assetIds);

            base.Start();
        }
    }
}
