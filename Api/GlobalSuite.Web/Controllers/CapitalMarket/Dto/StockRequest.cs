namespace GlobalSuite.Web.Controllers.CapitalMarket.Dto
{
    public class StockRequest
    {
        public string KeepStockCode { get; set; }
        public string SecName { get; set; }
        public string SecCode { get; set; }
        public string Industry { get; set; }
        public string Registrar { get; set; }
        public string InstruType { get; set; }
        public string CscsCode { get; set; }
        public decimal NominalValue { get; set; }
        public long IntialOutShare { get; set; }
        public bool DelistedByNSE { get; set; } 
    }
}