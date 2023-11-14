using CustomerManagement.Business;
using GlobalSuite.Core.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using Admin.Business;
using BaseUtility.Business;
using GlobalSuite.Core.Customers.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Customers
{
    public partial class CustomerService :BaseService, ICustomerService
    {

        public async Task<List<Customer>> GetCustomers(CustomerFilter filter)
        {
            return await Task.Run(() =>
            {
                var oCustomer = new Customer();
                filter = filter ?? new CustomerFilter();
                var ds = oCustomer.GetAll(filter.Branch);
                return ds.Tables[0].ToList<Customer>();
            });
        }
        public async Task<ResponseResult> CreateCustomer(Customer customer, List<CustomerBank> customerBanks, CustomerNextOfKin nextOfKin,
            CustomerEmployer employer,CustomerExtraInformation extraInformation )
        {
            var oBucket = new Bucket();
            customer.CustAID= oBucket.GetNextBucketNoNonIdentity("CUSTOMERORIGINAL").PadLeft(7, char.Parse("0"));
            customer.SaveType = Constants.SaveType.ADDS;
           

            return  await Task.Run(() =>
            {
                var result = Validate(customer);
                if(!result.IsSuccess) return result;

                var factory = new DatabaseProviderFactory();
                var db = factory.Create("GlobalSuitedb") as SqlDatabase;

                using (var connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    var dbCommandCustomer = customer.SaveCommand();
                    db.ExecuteNonQuery(dbCommandCustomer, transaction);
                   
                        // nextOfKin.CustAID = db.GetParameterValue(dbCommandCustomer, "CustAID").ToString();
                        // employer.CustAID = db.GetParameterValue(dbCommandCustomer, "CustAID").ToString();
                        // extraInformation.CustAID = db.GetParameterValue(dbCommandCustomer, "CustAID").ToString();

                    //Next of kin
                    if (nextOfKin != null)
                    {
                        nextOfKin.CustAID = customer.CustAID;
                   nextOfKin.SaveType = Constants.SaveType.ADDS;
                                       var dbCommandCustomerNextOfKin = nextOfKin.SaveCommand();
                                       db.ExecuteNonQuery(dbCommandCustomerNextOfKin, transaction);
                    }
                    
                    
                    //Employer
                    if (employer != null)
                    {
                        employer.CustAID = customer.CustAID;
                         employer.SaveType = Constants.SaveType.ADDS;
                                                var dbCommandCustomerEmployer = employer.SaveCommand();
                                                db.ExecuteNonQuery(dbCommandCustomerEmployer, transaction);
                    }
                   
 
                    // Customer Extra Information
                    if (extraInformation != null)
                    {
                        extraInformation.CustAID = customer.CustAID;
                    extraInformation.SaveType = Constants.SaveType.ADDS;
                        var dbCommandCustomerExtraInformation = extraInformation.SaveCommand();
                        db.ExecuteNonQuery(dbCommandCustomerExtraInformation, transaction);
                    }

                    // Customer Banks
                    if (customerBanks.Any())
                    {
                         foreach (var oCustomerBankObj in customerBanks)
                         {
                             oCustomerBankObj.SaveType = Constants.SaveType.ADDS;
                             oCustomerBankObj.CustAID = db.GetParameterValue(dbCommandCustomer, "CustAID").ToString();
                             var dbCommandCustomerBankObj = oCustomerBankObj.SaveCommand();
                             db.ExecuteNonQuery(dbCommandCustomerBankObj, transaction);
                         }
                    }
                    transaction.Commit();
                    return ResponseResult.Success();
 
                }

            });
        }

        private static ResponseResult Validate(Customer customer)
        {
            var oUserProfile = new UserProfile
            {
                UserName = GeneralFunc.UserName
            };
            oUserProfile.GetUserProfile();
            if (!oUserProfile.CustRecAcc && customer.SaveType ==Constants.SaveType.ADDS)
                 return ResponseResult.Error( "Cannot Save! You Do Not Have Access To Add New Customer Record");
            if (!oUserProfile.CustomerModify && customer.SaveType == Constants.SaveType.EDIT)
                 return ResponseResult.Error("Cannot Save! You Do Not Have Access Modify Customer Record");

            return ResponseResult.Success();
         }
        
    }
}
