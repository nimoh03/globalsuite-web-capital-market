using System;
using System.Threading.Tasks;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<CustomerBalance> GetCustomerBalance(string customerId, string productId)
        {
            
            if (customerId == null) throw new ArgumentNullException(nameof(customerId));
            if (productId == null) throw new ArgumentNullException(nameof(productId));
            return await Task.Run(() =>
            {
                try
                {
                    var oAccountGl = new AcctGL
                    {
                        AccountID = customerId,
                        AcctRef = productId
                    };
                    var oCustomer = new Customer{CustAID =customerId};
                    oCustomer.GetCustomer();
                    var customerBalance = new CustomerBalance
                    {
                        Customer = Mapper.Map<PublicCustomer>(oCustomer),
                        Balance = oAccountGl.GetAccountBalanceByCustomer()
                    };
                    return customerBalance;
                }
                catch (Exception ex)
                {
                    return CustomerBalance.Default;
                }
            });

        }
    }
}