namespace Lykke.Service.Wallets.Client.Models
{
    public class WalletCredentialsModel : BaseModel
    {
        public string ClientId { get; set; }
        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string MultiSig { get; set; }
        public string ColoredMultiSig { get; set; }
        public bool? PreventTxDetection { get; set; }
        public string EncodedPrivateKey { get; set; }

        public string BtcConvertionWalletPrivateKey { get; set; }
        public string BtcConvertionWalletAddress { get; set; }

        public string EthConversionWalletAddress { get; set; }
        public string EthAddress { get; set; }
        public string EthPublicKey { get; set; }

        public static WalletCredentialsModel Create(AutorestClient.Models.IWalletCredentials src)
        {
            if (src != null)
            {
                return new WalletCredentialsModel
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
            else
            {
                return new WalletCredentialsModel();
            }
        }
    }
}
