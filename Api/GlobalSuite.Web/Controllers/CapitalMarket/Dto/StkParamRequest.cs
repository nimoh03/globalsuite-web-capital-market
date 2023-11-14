namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class StkParamRequest
    {
        // Fees
        public decimal Bsec { get; set; }
        public decimal Bcncomm { get; set; }
        public decimal Bnse { get; set; }
        public decimal Bvat { get; set; }
        public decimal Bstamp { get; set; }
        public decimal BCsCs { get; set; }
        public decimal SMSAlertCSCSB { get; set; }
        public decimal BcscsVat { get; set; }
        public decimal SMSAlertCSCSVATB { get; set; }
        public decimal BsecVat { get; set; }
        public decimal BnseVat { get; set; }
        public decimal NASDBSec { get; set; }
        public decimal NASDBComm { get; set; }
        public decimal NASDBNASD { get; set; }
        public decimal NASDBCSCS { get; set; }
        public decimal NASDBSECVAT { get; set; }
        public decimal NASDBCommVAT { get; set; }
        public decimal NASDBNASDVAT { get; set; }
        public decimal NASDBCSCSVAT { get; set; }
        public decimal NASDBAlertVAT { get; set; }
        public decimal NASDBStamp { get; set; }
        public decimal NASDBSMSCharge { get; set; }
        public decimal BondSECBuy { get; set; }
        public decimal BondCommissionBuy { get; set; }
        public decimal BondNSEBuy { get; set; }
        public decimal BondCSCSBuy { get; set; }
        public decimal BondContractStampBuy { get; set; }
        public decimal BondSMSAlertBuy { get; set; }
        public decimal BondSMSAlertVATBuy { get; set; }
        
        
        public decimal Ssec { get; set; }
        public decimal Scncomm { get; set; }
        public decimal Snse { get; set; }
        public decimal Svat { get; set; }
        public decimal Sstamp { get; set; }
        public decimal Scscs { get; set; }
        public decimal SMSAlertCSCSS { get; set; }
        public decimal ScscsVat { get; set; }
        public decimal SMSAlertCSCSVATS { get; set; }
        public decimal SsecVat { get; set; }
        public decimal SnseVat { get; set; }
        public decimal NASDSSec { get; set; }
        public decimal NASDSComm { get; set; }
        public decimal NASDSNASD { get; set; }
        public decimal NASDSCSCS { get; set; }
        public decimal NASDSSECVAT { get; set; }
        public decimal NASDSCommVAT { get; set; }
        public decimal NASDSNASDVAT { get; set; }
        public decimal NASDSCSCSVAT { get; set; }
        public decimal NASDSAlertVAT { get; set; }
        public decimal NASDSStamp { get; set; }
        public decimal NASDSSMSCharge { get; set; }
        public decimal BondSECSell { get; set; }
        public decimal BondCommissionSell { get; set; }
        public decimal BondNSESell { get; set; }
        public decimal BondCSCSSell { get; set; }
        public decimal BondContractStampSell { get; set; }
        public decimal BondSMSAlertSell { get; set; }
        public decimal BondSMSAlertVATSell { get; set; }
        
        
        //Minimum Amount
        public decimal MinSecB { get; set; }
        public decimal MinCscsB { get; set; }
        public decimal MinStampB { get; set; }
        public decimal MinNSceB { get; set; }
        public decimal MinCommB { get; set; }
        public decimal MinVatB { get; set; }
        public decimal MinSecVatB { get; set; }
        public decimal MinNseVatB { get; set; }
        public decimal MinCscsVATB { get; set; }
        public decimal NASDBCommMin { get; set; }
        
        public decimal MinSecS { get; set; }
        public decimal MinCscsS { get; set; }
        public decimal MinStampS { get; set; }
        public decimal MinSceS { get; set; }
        public decimal MinCommS { get; set; }
        public decimal MinVatS { get; set; }
        public decimal MinSecVatS { get; set; }
        public decimal MinNseVatS { get; set; }
        public decimal MinCscsVatS { get; set; }
        public decimal NASDSCommMin { get; set; }
        
        public int DropJobOrderDayNumber { get; set; }
        public decimal OrderPriceMarkup { get; set; }
        public string NASDMemberCode { get; set; }
        public string CommType { get; set; }
     }
}