namespace Lykke.Service.Balances.Core.Wallets
{
    public interface IWalletCredentials
    {
        string ClientId { get; set; }
        string Address { get; set; }
        string PublicKey { get; set; }
        string PrivateKey { get; set; }
        string MultiSig { get; set; }
        string ColoredMultiSig { get; set; }
        bool PreventTxDetection { get; set; }
        string EncodedPrivateKey { get; set; }

        /// <summary>
        /// Conversion wallet is used for accepting BTC deposit and transfering needed LKK amount
        /// </summary>
        string BtcConvertionWalletPrivateKey { get; set; }
        string BtcConvertionWalletAddress { get; set; }

        /// <summary>
        /// Eth contract for user
        /// </summary>
        //ToDo: rename field to EthContract and change existing records
        string EthConversionWalletAddress { get; set; }
        string EthAddress { get; set; }
        string EthPublicKey { get; set; }

        string SolarCoinWalletAddress { get; set; }

        string ChronoBankContract { get; set; }

        string QuantaContract { get; set; }
    }
}
