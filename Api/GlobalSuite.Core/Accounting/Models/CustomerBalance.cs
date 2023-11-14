using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class CustomerBalance
    {
        public PublicCustomer Customer { get; set; }
        public decimal Balance { get; set; }
        public static CustomerBalance Default => new CustomerBalance();
    }
}