using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class KycDocTypeCustomerTypeRequest
    {
        [Required]
        public long KyDocTypeId { get; set; }
        [Required] 
        public int CustomerTypeId { get; set; }
        public bool IsOptional { get; set; }
    }
    public class CompulsoryCustomerKyc{
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public int DataType { get; set; }
    }
}