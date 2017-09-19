using System;
using Common.Log;

namespace Lykke.Service.Wallets.Client
{
    public class WalletsClient : IWalletsClient, IDisposable
    {
        private readonly ILog _log;

        public WalletsClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
