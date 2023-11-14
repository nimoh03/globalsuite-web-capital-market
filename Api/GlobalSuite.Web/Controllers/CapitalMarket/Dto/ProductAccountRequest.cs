namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class ProductAccountRequest
    {
        public string TransNo { get; set; }
        public string CustAID { get; set; }
        public string Branch { get; set; }
        public string BoxLoad { get; set; }
        public string AgentInd { get; set; }
        public float? Comm { get; set; }
        public float? SellCommission { get; set; }
        public string CsCsAcct { get; set; }
        public string CsCsReg { get; set; }
        public string NASDCsCsAcct { get; set; }
        public string NASDCsCsReg { get; set; }
        public string Agent { get; set; }
        public decimal AgentComm { get; set; }
        public string AccessCode { get; set; }
        public int Custodian { get; set; }
        public bool IsCustodian => Custodian > 0;
        public bool IsNASD { get; set; }
        public bool CustomerDirectCashSettlement { get; set; }
        public bool CustomerDirectCashSettlementNASD { get; set; }
        public bool DoNotChargeStampDuty { get; set; }
        public string DCSSetupDate { get; set; }
        public string DCSSetupDateNASD { get; set; }
    }
}