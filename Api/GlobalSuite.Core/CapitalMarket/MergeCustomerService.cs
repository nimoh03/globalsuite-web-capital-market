using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CapitalMarket.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> MergeCustomer(MergeCustomer oMergeCustomer, List<string> subCustomers)
        {

            return await Task.Run(() =>
            {
                try
                {
                         var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                        using (var connection = db.CreateConnection() as SqlConnection)
                        {
                connection.Open();
                 var transaction = connection.BeginTransaction();
                try
                {
                    oMergeCustomer.SaveType = Constants.SaveType.ADDS;
                     
                        if (oMergeCustomer.ChkNameExist())
                            return ResponseResult.Error("Cannot Add Merge Customer, Customer Account Already Merged");
                        var dbCommandMaster = oMergeCustomer.AddMasterCommand();
                        db.ExecuteNonQuery(dbCommandMaster, transaction);
                        if(subCustomers!=null)
                        foreach (var oCustomerAccountNo in subCustomers)
                        {
                            oMergeCustomer.SubID = oCustomerAccountNo;
                            var dbCommandSub = oMergeCustomer.AddSubCommand();
                            db.ExecuteNonQuery(dbCommandSub, transaction);
                        }
                        transaction.Commit();
                        return  ResponseResult.Success();


                        // else if (strMode.Trim() == "EDIT")
                    // {
                    //     oMergeCustomer.TransNo = txtCode.Text;
                    //     oMergeCustomer.AcctID = txtCustomer.Text;
                    //     if (!oMergeCustomer.ChkNameExist())
                    //     {
                    //         SqlCommand dbCommandMaster = oMergeCustomer.EditMasterCommand();
                    //         db.ExecuteNonQuery(dbCommandMaster, transaction);
                    //         SqlCommand dbCommandDeleteSubByMaster = oMergeCustomer.DeleteSubByMasterCommand();
                    //         db.ExecuteNonQuery(dbCommandDeleteSubByMaster, transaction);
                    //         foreach (Object oCustNo in listBox1.Items)
                    //         {
                    //             oMergeCustomer.TransNo = txtCode.Text;
                    //             oMergeCustomer.AcctID = txtCustomer.Text;
                    //             oMergeCustomer.SubID = oCustNo.ToString().Substring(0, oCustNo.ToString().Trim().IndexOf(" "));
                    //             SqlCommand dbCommandSub = oMergeCustomer.AddSubCommand();
                    //             db.ExecuteNonQuery(dbCommandSub, transaction);
                    //         }
                    //         transaction.Commit();
                    //         RefreshPage();
                    //     }
                    //     else
                    //     {
                    //         strErrMessage = "Cannot Edit Merge Customer, Customer Account Already Merged";
                    //         MessageBox.Show(strErrMessage, Application.ProductName + " Version " + Application.ProductVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //     }
                    // }
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    return ResponseResult.Error("Error In Merging " + ex.Message);
                }
                        }
                }
                catch (Exception ex)
                {
                    return ResponseResult.Error("Error In Merging " + ex.Message);
                }
            });
        }

        
    }
}