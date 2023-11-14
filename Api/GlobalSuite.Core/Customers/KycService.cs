using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Customers
{
    

    public partial class CustomerService
    {
        public async Task<List<KYCDocType>> GetKycDocTypes()
        {
            var kyc = new KYCDocType();
            return await Task.Run(() => kyc.GetAll().Tables[0].ToList<KYCDocType>());
        }

        public async Task<ResponseResult> SetKycDocTypeForCustomerType(long kycTransNo, int customerTypeTransNo, bool isOptional=false)
        {
            var oKycDocTypeForCustomerType = new KYCDocTypeForCustomerType
            {
                KYCDocTypeId =kycTransNo,
                CustomerTypeId = customerTypeTransNo,
                IsOptional = isOptional,
                SaveType = Constants.SaveType.ADDS
            };
          var result=  await Task.Run(() => oKycDocTypeForCustomerType.SaveCommand());
          return result <= 0 ? ResponseResult.Error("Operation unsuccessful") : ResponseResult.Success(result);
        }

        public async Task<List<CustomerFieldData.KYCCompulsoryCustomer>> GetKycCompulsoryCustomer()
        {
            var oKycCustomer = new CustomerFieldData.KYCCompulsoryCustomer();
            return await Task.Run(() =>
                oKycCustomer.GetAll().Tables[0].ToList<CustomerFieldData.KYCCompulsoryCustomer>());
        }

        public async Task<ResponseResult> SetCompulsoryKyc(List<(string customerFieldId, string customerFieldName, int customerFieldDataType)> compusoryKycs)
        {
            SqlTransaction transaction = null;
            try
            {
                await Task.Run(() =>
                {
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    var oKycCompulsoryCustomer = new CustomerFieldData.KYCCompulsoryCustomer();
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();
                        using (var dbCommandKycCompulsoryCustomerDelete = oKycCompulsoryCustomer.DeleteAllCommand())
                        {
                            db.ExecuteNonQuery(dbCommandKycCompulsoryCustomerDelete, transaction);
                        }
                        foreach (var dbCommandKycCompulsoryCustomerObj in 
                                 compusoryKycs.Select(item => new CustomerFieldData.KYCCompulsoryCustomer
                                 {
                                     CustomerFieldId = item.customerFieldId,
                                     CustomerFieldName = item.customerFieldName,
                                     CustomerFieldDataType = item.customerFieldDataType,
                                 }).Select(oKycCompulsoryCustomerObj => oKycCompulsoryCustomerObj.SaveCommand()))
                        {
                            db.ExecuteNonQuery(dbCommandKycCompulsoryCustomerObj, transaction);
                        }
                        transaction.Commit();
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                transaction?.Rollback();
                return ResponseResult.Error("Error In Saving Compulsory Customer KYC  " + ex.Message.Trim());
            }
             

            return ResponseResult.Success();
        }
    }
}