namespace GlobalSuite.Core.Accounting.Models
{
    public class ProductDetailResponse:ProductResponse
    { 
        public string ModuleName { get; set; }
        public string GLAcct { get; set; }
        public string ProductClass { get; set; }
        public string DefaultProduct { get; set; }
        public int ProductType { get; set; } 
    }

    public class ProductResponse
    {
        public string ProductCode { get; set; } 
        public string ProductName { get; set; }
    }
}