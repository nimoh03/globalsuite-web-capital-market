using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Ninject;
using Serilog;

namespace GlobalSuite.Core
{
    public class BaseService
    {


        public readonly ILogger Logger = Log.Logger;
        [Inject]
        public  IMapper Mapper { get; set; }

        public async Task<ResponseResult> ReverseSelfBalance(string code)
        {
            return await Task.Run(() =>
            {
                var strJnumberNext = "";
                var oSelfBal = new SelfBal();
                var oGl = new AcctGL();
                var oGlGetRec = new AcctGL();
                oSelfBal.TransNo = code.Trim();
                if (oSelfBal.GetSelfBal(DataGeneral.PostStatus.Posted))
                {

                    var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();

                        try
                        {
                            oGlGetRec.SysRef = "SBL" + "-" + code.Trim();
                            if (oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows.Count >= 1)
                            {
                                var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                db.ExecuteNonQuery(oCommandJnumber, transaction);
                                strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                foreach (DataRow oRowView in oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows)
                                {
                                    oGl.EffectiveDate = oRowView["EffDate"].ToString().ToDate();
                                    oGl.MasterID = oRowView["MasterID"].ToString().Trim();
                                    oGl.AccountID = oRowView["AccountID"].ToString().Trim();
                                    if (oRowView["Debcred"].ToString().Trim() == "D")
                                    {
                                        oGl.Credit = decimal.Parse(oRowView["Debit"].ToString().Trim());
                                        oGl.Debit = 0;
                                        oGl.Debcred = "C";
                                    }
                                    else if (oRowView["Debcred"].ToString().Trim() == "C")
                                    {
                                        oGl.Credit = 0;
                                        oGl.Debit = decimal.Parse(oRowView["Credit"].ToString().Trim());
                                        oGl.Debcred = "D";
                                    }
                                    oGl.FeeType = oRowView["FeeType"].ToString().Trim();
                                    oGl.Desciption = "REVERSAL Of " + oRowView["Description"].ToString().Trim();
                                    oGl.TransType = oRowView["TransType"].ToString().Trim();
                                    oGl.SysRef = oRowView["SysRef"].ToString().Trim();
                                    oGl.Ref01 = oRowView["Ref01"].ToString().Trim();
                                    oGl.Ref02 = oRowView["Ref02"].ToString().Trim();
                                    oGl.AcctRef = oRowView["AcctRef"].ToString().Trim();
                                    oGl.AcctRefSecond = oRowView["AcctRefSecond"].ToString().Trim();
                                    oGl.RecAcctMaster = oRowView["RecAcctMas"].ToString().Trim();
                                    oGl.RecAcct = oRowView["RecAcctSub"].ToString().Trim();
                                    oGl.Reverse = "Y";
                                    oGl.Jnumber = strJnumberNext;
                                    oGl.InstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), oRowView["InstrumentType"].ToString().Trim(), false); 
                                    oGl.Chqno = oRowView["Chqno"].ToString().Trim();
                                    oGl.Branch = oRowView["Branch"].ToString().Trim();
                                    var dbCommandGlTrans = oGl.AddCommand();
                                    db.ExecuteNonQuery(dbCommandGlTrans, transaction);
                                }
                                var dbCommandGlTransUpdate = oGlGetRec.UpdateGLBySysRefReversalCommand();
                                db.ExecuteNonQuery(dbCommandGlTransUpdate, transaction);

                                oSelfBal.Reversed = "Y";
                                var dbCommandSelfBal = oSelfBal.UpDateRevCommand();
                                db.ExecuteNonQuery(dbCommandSelfBal, transaction);

                                transaction.Commit();
                                return ResponseResult.Success();
                            }
                        }
                        catch(Exception ex) 
                        {
                            Logger.Error(ex.Message, ex);
                            transaction.Rollback();
                            return ResponseResult.Error("Error Reversing Self Balancing Posting " + ex.Message.Trim());
                        }

                    }
                }
                return ResponseResult.Error("Self Balance cannot be posted.");
            });
        }
    }
}
