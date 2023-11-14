namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class CustomerTypeResponse
    {
        public string TransNo { get; set; }
        public string CustomerTypeName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsInstitutional { get; set; }
        public bool IsJointAccount { get; set; }
     }
}