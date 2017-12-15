﻿using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.AzureRepositories.Wallets
{
    public class WalletCredentials : IWalletCredentials
    {
        public string ClientId { get; set; }
        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string MultiSig { get; set; }
        public string ColoredMultiSig { get; set; }
        public bool PreventTxDetection { get; set; }
        public string EncodedPrivateKey { get; set; }

        /// <summary>
        /// Conversion wallet is used for accepting BTC deposit and transfering needed LKK amount
        /// </summary>
        public string BtcConvertionWalletPrivateKey { get; set; }
        public string BtcConvertionWalletAddress { get; set; }

        //EthContract in fact. ToDo: rename
        public string EthConversionWalletAddress { get; set; }
        public string EthAddress { get; set; }
        public string EthPublicKey { get; set; }

        public string SolarCoinWalletAddress { get; set; }
        public string ChronoBankContract { get; set; }
        public string QuantaContract { get; set; }

        public static WalletCredentials Create(string clientId, string address, string privateKey,
            string multisig, string coloredMultiSig, string btcConvertionWalletPrivateKey,
            string btcConvertionWalletAddress, bool preventTxDetection = false,
            string encodedPk = "", string pubKey = "")
        {
            return new WalletCredentials
            {
                ClientId = clientId,
                Address = address,
                PublicKey = pubKey,
                PrivateKey = privateKey,
                MultiSig = multisig,
                ColoredMultiSig = coloredMultiSig,
                PreventTxDetection = preventTxDetection,
                EncodedPrivateKey = encodedPk,
                BtcConvertionWalletPrivateKey = btcConvertionWalletPrivateKey,
                BtcConvertionWalletAddress = btcConvertionWalletAddress
            };
        }

        public static WalletCredentials Create(IWalletCredentials src)
        {
            return new WalletCredentials
            {
                ClientId = src.ClientId,
                Address = src.Address,
                PrivateKey = src.PrivateKey,
                MultiSig = src.MultiSig,
                ColoredMultiSig = src.ColoredMultiSig,
                PreventTxDetection = src.PreventTxDetection,
                EncodedPrivateKey = src.EncodedPrivateKey,
                PublicKey = src.PublicKey,
                BtcConvertionWalletPrivateKey = src.BtcConvertionWalletPrivateKey,
                BtcConvertionWalletAddress = src.BtcConvertionWalletAddress,
                EthConversionWalletAddress = src.EthConversionWalletAddress,
                EthAddress = src.EthAddress,
                EthPublicKey = src.EthPublicKey
            };
        }
    }

}
