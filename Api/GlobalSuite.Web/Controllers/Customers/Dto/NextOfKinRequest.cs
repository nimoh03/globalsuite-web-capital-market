using System.ComponentModel.DataAnnotations;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class NextOfKinRequest{
        public string CustAID { get; set; }
        public string NextKinName { get; set; } = "";
        public string NextKinPhone { get; set; } = "";
        public string NextKinAddress { get; set; } = "";
        public string NextKinRelationship { get; set; } = "";
       [DataType(DataType.EmailAddress)]
       public string EmailAddress { get; set; }  = "";
    }
}