using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Lykke.Service.Balances.AutorestClient
{
    /// <summary>
    /// Used to prevent memory leak in RetryPolicy
    /// </summary>
    public partial class BalancesAPI
    {
        public BalancesAPI(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();
            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}
