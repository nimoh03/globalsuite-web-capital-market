namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class PGenTableRequest
    {
        // Fees Account
        public string Bcscs { get; set; }
        public string Scscs { get; set; }
        public string Bnse { get; set; }
        public string Snse { get; set; }
        public string Bstamp { get; set; }
        public string Sstamp { get; set; }
        public string Bvat { get; set; }
        public string Svat { get; set; }
        public string Bcncomm { get; set; }
        public string Scncomm { get; set; }
        public string Bsec { get; set; }
        public string Ssec { get; set; }
        
        public string NASDBSec { get; set; }
        public string NASDBNASD { get; set; }
        public string NASDBComm { get; set; }
        public string NASDBCSCS { get; set; }
        public string NASDBStamp { get; set; }
        public string NASDBVAT { get; set; }
        
        public string NASDSSec { get; set; }
        public string NASDSNASD { get; set; }
        public string NASDSComm { get; set; }
        public string NASDSCSCS { get; set; }
        public string NASDSStamp { get; set; }
        public string NASDSVAT { get; set; }
        
        // Trading Account
        public string TradeBankBank { get; set; }
        public string Verification { get; set; }
        public string CapGainContra { get; set; }
        public string TradeBank { get; set; }
        public string PropTradeBank { get; set; }
        public string NASDTradingBank { get; set; }
        public string DirectCashSettleAcct { get; set; }
        public string CapGain { get; set; }
        public string Offer { get; set; }
        public string ShInv { get; set; }
        public string OffBank { get; set; }
        public string BondOffer { get; set; }
        public string strStkAcct { get; set; }
        public string strBrokPay { get; set; }
        public string strInvestment { get; set; }
        public string strNASDAccount { get; set; }
        public string strInvestmentNASDAC { get; set; }
        public string strInvestmentBondAC { get; set; }
        public string OverDrawn { get; set; }
        public string StatutoryControl { get; set; }
        public string AgComm { get; set; } 
    }
}