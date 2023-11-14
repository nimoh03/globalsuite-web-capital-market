namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class CustomerTransferRequest 
    {
        
        public string TransNo { get; set; }
        public string RCustAID { get; set; }
        public string RProduct { get; set; }
        public string Ref { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TCustAID { get; set; }
        public string TProduct { get; set; }
        public string TransNoRev { get; set; } 
    }
}