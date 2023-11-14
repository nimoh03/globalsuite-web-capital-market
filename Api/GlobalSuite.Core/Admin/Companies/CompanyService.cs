using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Admin
{
    public partial class AdminService 
    {
 
        public async Task<List<CompanyResponse>> GetAllCompanies()
        {
            var oCompany=new Company();
            return await Task.Run(() =>
            {
                return oCompany.GetAll().Tables[0].ToList<CompanyResponse>();
            });
        }
        public async Task<DataGeneral.SaveStatus> SaveCompany(Company company)
        {
           return await Task.Run(() =>
            {
                company.SaveType = Constants.SaveType.ADDS;
                var oBucket = new Bucket();
               company.TransNo =long.Parse(oBucket.GetNextBucketNo("COMPANY"));
                return  company.Save();
            });

        }
        /// <summary>
        /// Run End of Month
        /// </summary>
        /// <param name="runDate"></param>
        /// <returns></returns>
        public async Task<ResponseResult> RunEoM(DateTime runDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oEom = new EOMRun
            {
                RunDate = runDate,
                RunType = Constants.RunType.E
            };
            return await Task.Run(() =>
            {
 
                try
                {
                var oCompany = new Company();
                    if (oCompany.CheckOpenClosedPeriod())
                        return ResponseResult.Error("Cannot Run End Of Month.Closed Period Is Open. Please Close The Opened Period");
                    if (oCompany.CheckMonthIsClosed())
                        return ResponseResult.Error("Cannot Run End Of Month.Month Is Closed. Please Run Start Of Month");
                    if (oCompany.GetEOMRunDate() != DateTime.MinValue)
                        runDate = oCompany.GetEOMRunDate();
                    else
                    {
                      var err=  oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "" ?
                        oCompany.ErrMessageToReturn.Trim() : "Cannot Run End Of Month. Table Not Set Properly";
                        return ResponseResult.Error(err);
                    }
                    //Check YEAR
                    if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                    {
                        if (DateTime.ParseExact(oCompany.StartYear.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) 
                        <= runDate
                            && DateTime.ParseExact(oCompany.EndYear.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) >=
                            runDate)
                        { }
                        else
                        {
                            return ResponseResult.Error("End Of Month Must Be Within Financial Year");
                         }
                    }
                    var sqlCommand = oEom.AddCommand();
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();


                        var oGeneralFunc = new GeneralFunc();
                        var datMonthAddedDate = runDate.AddMonths(1);
                        var correctedEndOfMonthDatedatMonthAddedDate = DateTime.ParseExact(GeneralFunc.GetMonthLastDay(datMonthAddedDate.Month, datMonthAddedDate.Year) + "/" + datMonthAddedDate.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datMonthAddedDate.Year, "dd/MM/yyyy", format);

                        var oCommandUpdateEomDate = oCompany.UpdateEOMRunDateCommand(correctedEndOfMonthDatedatMonthAddedDate);
                        db.ExecuteNonQuery(oCommandUpdateEomDate, transaction);

                        var oCommandCloseEom = oCompany.CloseEOMCommand();
                        db.ExecuteNonQuery(oCommandCloseEom, transaction);

                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }
               
            });
        }
        /// <summary>
        /// Run Start of Month
        /// </summary>
        /// <param name="runDate"></param>
        /// <returns></returns>
        public async Task<ResponseResult> RunSoM(DateTime runDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oEom = new EOMRun
            {
                RunDate = runDate,
                RunType = Constants.RunType.S
            };
            return await Task.Run(() =>
            {
                try
                {
                    var oCompany = new Company();
                    if (oCompany.CheckYearIsClosed())
                        return ResponseResult.Error("Cannot Run Start Of Month.Year Is Closed,please Run Start Of Year");
                    if (oCompany.CheckOpenClosedPeriod())
                        return ResponseResult.Error("Cannot Run Start Of Month.Closed Period Is Open,please Close The Opened Period");
                    if (oCompany.CheckMonthIsClosed())
                        return ResponseResult.Error("Cannot Open A New Month.Previous Month Is Not Closed,please Run End Of Month");
                    if (oCompany.GetEOMRunDate() != DateTime.MinValue)
                        runDate = oCompany.GetEOMRunDate();
                    else
                    {
                        var err = oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "" ?
                          oCompany.ErrMessageToReturn.Trim() : "Cannot Run End Of Month. Table Not Set Properly";
                        return ResponseResult.Error(err);
                    }
                    //Check YEAR
                    if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                    {
                        if (DateTime.ParseExact(oCompany.StartYear.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format)
                        <= runDate
                            && DateTime.ParseExact(oCompany.EndYear.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) >=
                            runDate)
                        { }
                        else
                        {
                            return ResponseResult.Error("End Of Month Must Be Within Financial Year");
                        }
                    }
                    var sqlCommand = oEom.AddCommand();
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();


                        var oGeneralFunc = new GeneralFunc();
                        var datMonthAddedDate = runDate.AddMonths(1);
                        var correctedEndOfMonthDatedatMonthAddedDate = DateTime.ParseExact(GeneralFunc.GetMonthLastDay(datMonthAddedDate.Month, datMonthAddedDate.Year) + "/" + datMonthAddedDate.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datMonthAddedDate.Year, "dd/MM/yyyy", format);

                        var oCommandUpdateEomDate = oCompany.UpdateEOMRunDateCommand(correctedEndOfMonthDatedatMonthAddedDate);
                        db.ExecuteNonQuery(oCommandUpdateEomDate, transaction);

                        var oCommandCloseEom = oCompany.CloseEOMCommand();
                        db.ExecuteNonQuery(oCommandCloseEom, transaction);

                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> RunEoY()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();
            var now= DateTime.Now;
            var  startDate = now.FirstDayOfYear();
            var endDate=now.LastDayOfYear();

            var oEom = new EOYRun
            {
                RunStartDate = startDate,
                RunEndDate = endDate,
                RunType = Constants.RunType.E
            };
            return await Task.Run(() =>
            {
                try
                {
                    var oCompany = new Company();
                    if (oCompany.CheckOpenClosedPeriod())
                        return ResponseResult.Error("Cannot Run End Of Month.Closed Period Is Open. Please Close The Opened Period");
                    if (oCompany.CheckYearIsClosed())
                        return ResponseResult.Error("Cannot Run End Of Year. Year Is Closed. Please Run Start Of Month");
                    oGlParam.Type = "CHKMONTHPOST";
                    var strCheckMonthPostFlag = oGlParam.CheckParameter();
                   
                    if (strCheckMonthPostFlag.Trim() == "YES")
                    {
                        if (!oCompany.CheckMonthIsClosed())
                        {
                            return ResponseResult.Error("Cannot Run End Of Year.Month Must Be Closed,please Run End Of Month");
                        }
                    }
                    if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                    {
                         startDate = oCompany.StartYear; endDate = oCompany.EndYear;
                    }
                    else
                    {
                       return ResponseResult.Error(oCompany.ErrMessageToReturn.Trim());
                     }
                    //Check EOM IS MORE THAN END OF YEAR BY A MONTH
                    if (strCheckMonthPostFlag.Trim() == "YES")
                    {
                        if (oCompany.GetEOMRunDate() != DateTime.MinValue)
                        {
                            var dblDaysInterval = DateTime.ParseExact(oCompany.GetEOMRunDate().ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Subtract(endDate).TotalDays;
                            if (!((int)dblDaysInterval >= 28 && (int)dblDaysInterval <= 31))
                            {
                                return ResponseResult.Error("Cannot Run EOY. EOM Date Must Be One Month More Than The EOY Date");
                            }
                        }
                        else
                        {
                            var err = oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "" ? oCompany.ErrMessageToReturn.Trim() : "Cannot Run End Of Month.Table Not Set Properly";
                            return ResponseResult.Error(err);   
                        }
                    }

                    // Begin Run
                    if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                    {
                        return ResponseResult.Error("Cannot Run EOY. GL Parameter Table Not Set Or Reserve A/C Missing");
                    }
                    var paramResult=CheckEoYParamIsCorrect(startDate, endDate);
                    if (!paramResult.IsSuccess) return paramResult;

                   
                    var sqlCommand = oEom.AddCommand();
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        var datFormerEomDate = DateTime.MinValue;
                        oGlParam.Type = "CHKMONTHPOST";
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            datFormerEomDate = oCompany.GetEOMRunDate();
                            var oCommandUpdateEomDate =
                            oCompany.UpdateEOMRunDateCommand(endDate);
                            db.ExecuteNonQuery(oCommandUpdateEomDate, transaction);

                            var oCommandOpenEom = oCompany.OpenEOMCommand();
                            db.ExecuteNonQuery(oCommandOpenEom, transaction);
                        }
                        //Post Profit And Loss Balances
                        PostProfitLossBalance(db, transaction, startDate, endDate, oBranch, oGlParam);

                        //Save To Account Balances Table
                        SaveToAccountBalancesTable(db, transaction, endDate);

                        //Zerorise Stock Prop Holding
                        ZerorisePropHolding(db, transaction, endDate);

                        var oEoyRun = new EOYRun();
                        oEoyRun.RunStartDate = startDate;
                        oEoyRun.RunEndDate = endDate;
                        oEoyRun.RunType = "E";
                        var oCommandEoyRun = oEoyRun.AddCommand();
                        db.ExecuteNonQuery(oCommandEoyRun, transaction);

                        var datAddedStartYear = startDate.AddYears(1);
                        var datAddedEndYear = endDate.AddYears(1);
                        var oCommandUpdateEoyDate = oCompany.UpdateEOYDateCommand(datAddedStartYear, datAddedEndYear);
                        db.ExecuteNonQuery(oCommandUpdateEoyDate, transaction);

                        var oCommandCloseEoy = oCompany.CloseEOYCommand();
                        db.ExecuteNonQuery(oCommandCloseEoy, transaction);


                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            var oCommandUpdateEomDateAfterPosting = oCompany.UpdateEOMRunDateCommand(datFormerEomDate);
                            db.ExecuteNonQuery(oCommandUpdateEomDateAfterPosting, transaction);

                            var oCommandCloseEom = oCompany.CloseEOMCommand();
                            db.ExecuteNonQuery(oCommandCloseEom, transaction);
                        }
                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> RunSoY()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();
            var now = DateTime.Now;
            var startDate = now.FirstDayOfYear();
            var endDate = now.LastDayOfYear();

            
            return await Task.Run(() =>
            {
                try
                {
                    var oCompany = new Company();
                    if (oCompany.CheckOpenClosedPeriod())
                        return ResponseResult.Error("Cannot Run End Of Month.Closed Period Is Open. Please Close The Opened Period");
                    if (!oCompany.CheckYearIsClosed())
                        return ResponseResult.Error("Cannot Run End Of Year. Year Is Closed. Please Run Start Of Month");
                    else
                    {
                        if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                        {
                            startDate = oCompany.StartYear ;
                            endDate = oCompany.EndYear;
                        }
                        else
                        {
                            return ResponseResult.Error(oCompany.ErrMessageToReturn.Trim());

                        }
                    }

                    // Begin Run
                    if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                    {
                        return ResponseResult.Error("Cannot Run EOY. GL Parameter Table Not Set Or Reserve A/C Missing");
                    }
                    var paramResult = CheckEoYParamIsCorrect(startDate, endDate);
                    if (!paramResult.IsSuccess) return paramResult;
                    
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        ///Update  EOY Date Table
                        var oEoyRun = new EOYRun
                        {
                            RunStartDate = startDate,
                            RunEndDate = endDate,
                            RunType =Constants.RunType.S
                        };
                        var oCommandEoyRun = oEoyRun.AddCommand();
                        db.ExecuteNonQuery(oCommandEoyRun, transaction);

                       
                        var oCommandOpenEoy = oCompany.OpenEOYCommand();
                        db.ExecuteNonQuery(oCommandOpenEoy, transaction);
                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> OpenClosedPeriod(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();
 

            return await Task.Run(() =>
            {
                try
                {
                    if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                    {
                        return ResponseResult.Error("Cannot Open Closed Period. GL Parameter Table Not Set Or Reserve A/C Missing");
                    }
                    var paramResult = CheckOpenClosedPeriodParamIsCorrect(monthDate,yearStartDate, yearEndDate, oGlParam);
                    if (!paramResult.IsSuccess) return paramResult;


                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        DateTime datMonthRunDate;
                        oGlParam.Type = "CHKMONTHPOST";
                        var strCheckMonthPostFlag = oGlParam.CheckParameter();
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            datMonthRunDate = monthDate;
                        }
                        else
                        {
                            datMonthRunDate = DateTime.MinValue;
                        }
                        var oOpenCloseClosedPeriod = new OpenCloseClosedPeriod();
                        var oCompany = new Company();
                        oCompany.GetCompany(GeneralFunc.CompanyNumber);
                        var oBucket = new Bucket();
                         oOpenCloseClosedPeriod.TransNo = oBucket.GetNextBucketNo("OPENCLOSECLOSEDPERIOD");

                        oOpenCloseClosedPeriod.OpenOrClosePeriod = "O";
                        oOpenCloseClosedPeriod.PastOrCurrentYear = "P";
                        oOpenCloseClosedPeriod.YearStartDate = yearStartDate;
                        oOpenCloseClosedPeriod.YearEndDate = yearEndDate;
                        oOpenCloseClosedPeriod.MonthDate = datMonthRunDate;
                        oOpenCloseClosedPeriod.CurrentYearStartDate = oCompany.StartYear;
                        oOpenCloseClosedPeriod.CurrentYearEndDate = oCompany.EndYear;
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            oOpenCloseClosedPeriod.CurrentMonthDate = oCompany.EOMRunDate;
                        }
                        else
                        {
                            oOpenCloseClosedPeriod.CurrentMonthDate = DateTime.MinValue;
                        }
                        var oCommmandOpenClosePeriod = oOpenCloseClosedPeriod.AddCommand();
                        db.ExecuteNonQuery(oCommmandOpenClosePeriod, transaction);

                        var oCommandUpdateOpenClosedPeriod = oCompany.UpdateOpenClosedPeriodCommand
                                        (yearStartDate, yearEndDate,
                                        datMonthRunDate);
                        db.ExecuteNonQuery(oCommandUpdateOpenClosedPeriod, transaction);

                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> CloseClosedPeriod(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();

            return await Task.Run(() =>
            {
                try
                {
                    if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                    {
                        return ResponseResult.Error("Cannot Open Closed Period. GL Parameter Table Not Set Or Reserve A/C Missing");
                    }
                    var paramResult = CheckCloseClosedPeriodParamIsCorrect(monthDate, yearStartDate, yearEndDate, oGlParam);
                    if (!paramResult.IsSuccess) return paramResult;


                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        oGlParam.Type = "CHKMONTHPOST";
                        var strCheckMonthPostFlag = oGlParam.CheckParameter();

                        //Post Profit And Loss Balances
                        CloseClosedPostProfitLossBalance(db,transaction, yearStartDate,yearEndDate, oBranch, oGlParam);

                        //Save To Account Balances Table
                        CloseClosedSaveToAccountBalancesTable(db, transaction, yearEndDate);

                        var oOpenCloseClosedPeriod = new OpenCloseClosedPeriod();
                        var oCompany = new Company();
                        oCompany.GetCompany(GeneralFunc.CompanyNumber);
                        var oBucket = new Bucket();
                        oOpenCloseClosedPeriod.TransNo = oBucket.GetNextBucketNo("OPENCLOSECLOSEDPERIOD");
                        oOpenCloseClosedPeriod.OpenOrClosePeriod = "C";
                        oOpenCloseClosedPeriod.PastOrCurrentYear = "P";
                        oOpenCloseClosedPeriod.YearStartDate = oCompany.StartYear;
                        oOpenCloseClosedPeriod.YearEndDate = oCompany.EndYear;
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            oOpenCloseClosedPeriod.MonthDate = oCompany.EOMRunDate;
                            oOpenCloseClosedPeriod.CurrentMonthDate = oCompany.CurrentMonthDate;
                        }
                        else
                        {
                            oOpenCloseClosedPeriod.MonthDate = DateTime.MinValue;
                            oOpenCloseClosedPeriod.CurrentMonthDate = DateTime.MinValue;
                        }
                        oOpenCloseClosedPeriod.CurrentYearStartDate = oCompany.CurrentYearStartDate;
                        oOpenCloseClosedPeriod.CurrentYearEndDate = oCompany.CurrentYearEndDate;

                        var oCommmandOpenClosePeriod = oOpenCloseClosedPeriod.AddCommand();

                        var oCommandUpdateOpenClosedPeriod = oCompany.UpdateCloseClosedPeriodCommand();
                        db.ExecuteNonQuery(oCommandUpdateOpenClosedPeriod, transaction);


                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> OpenClosedPeriodCurrentYear(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();
            var oCompany = new Company();


            return await Task.Run(() =>
            {
                try
                {
                    if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                    {
                        return ResponseResult.Error("Cannot Open Closed Period. GL Parameter Table Not Set Or Reserve A/C Missing");
                    }
                    var paramResult = CheckOpenClosedPeriodParamIsCorrect(monthDate, yearStartDate, yearEndDate, oGlParam);
                    if (!paramResult.IsSuccess) return paramResult;


                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        oGlParam.Type = "CHKMONTHPOST";
                        var strCheckMonthPostFlag = oGlParam.CheckParameter();
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                            {
                                yearStartDate = oCompany.StartYear;
                                yearEndDate = oCompany.EndYear;
                            }
                            else
                            {
                                return ResponseResult.Error("Cannot Open Closed Month Period Within Current Period.Financial Year Period Does Not Exist");
                             }
                        }
                        var result = CheckOpenClosedPeriodParamIsCorrect(monthDate, yearStartDate, yearEndDate, oGlParam);
                        if (!result.IsSuccess) return result;
                        var oOpenCloseClosedPeriod = new OpenCloseClosedPeriod();
                         oCompany.GetCompany(GeneralFunc.CompanyNumber);
                        var oBucket = new Bucket();
                        oOpenCloseClosedPeriod.TransNo = oBucket.GetNextBucketNo("OPENCLOSECLOSEDPERIOD");
                        oOpenCloseClosedPeriod.OpenOrClosePeriod = "O";
                        oOpenCloseClosedPeriod.PastOrCurrentYear = "C";
                        oOpenCloseClosedPeriod.YearStartDate = yearStartDate;
                        oOpenCloseClosedPeriod.YearEndDate = yearEndDate;
                        oOpenCloseClosedPeriod.MonthDate =monthDate;
                        oOpenCloseClosedPeriod.CurrentYearStartDate = oCompany.StartYear;
                        oOpenCloseClosedPeriod.CurrentYearEndDate = oCompany.EndYear;
                        oOpenCloseClosedPeriod.CurrentMonthDate = oCompany.EOMRunDate;

                        var oCommmandOpenClosePeriod = oOpenCloseClosedPeriod.AddCommand();
                        db.ExecuteNonQuery(oCommmandOpenClosePeriod, transaction);

                        var oCommandUpdateOpenClosedMonthPeriod = oCompany.UpdateOpenClosedMonthPeriodCommand(monthDate);
                        db.ExecuteNonQuery(oCommandUpdateOpenClosedMonthPeriod, transaction);
                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        public async Task<ResponseResult> CloseClosedPeriodCurrentYear(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            SqlTransaction transaction = null;
            var oGlParam = new GLParam();
            var oBranch = new Branch();
            var oCompany = new Company();
            return await Task.Run(() =>
            {
                try
                {
                    if (!oCompany.CheckOpenClosedPeriod())
                         return ResponseResult.Error("Cannot Close Open Period.No Closed Period Is Open,please Open Closed Period");
                     oGlParam.Type = "CHKMONTHPOST";
                    var strCheckMonthPostFlag = oGlParam.CheckParameter();


                    var oOpenCloseClosedPeriod = new OpenCloseClosedPeriod();
                    oOpenCloseClosedPeriod.TransNo = oOpenCloseClosedPeriod.GetLastTransactionNumber().ToString();

                    if (oOpenCloseClosedPeriod.GetOpenCloseClosedPeriod())
                    {
                        oCompany.GetCompany(GeneralFunc.CompanyNumber);
                        if (oOpenCloseClosedPeriod.OpenOrClosePeriod.Trim() == "C")
                        {
                            return ResponseResult.Error("Cannot Close Opened Period.Last Opened Period Is Closed");
                         }
                        if (oOpenCloseClosedPeriod.PastOrCurrentYear.Trim() == "P")
                        {
                            return ResponseResult.Error("Cannot Close Opened Period.Last Opened Period Is Not Within Current Year Period.Use Close Period");
                        }
                        if ((oOpenCloseClosedPeriod.YearStartDate != oCompany.StartYear) || (oOpenCloseClosedPeriod.YearEndDate != oCompany.EndYear))
                        {
                            return ResponseResult.Error("Cannot Close Opened Period.Current Financial Year Period Not The Same With Last Open Period.Please Correct");
                        }
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            if (oOpenCloseClosedPeriod.MonthDate != oCompany.EOMRunDate)
                            {
                                return ResponseResult.Error("Cannot Close Opened Period.Current Financial Month Period Not The Same With Last Open Period.Please Correct");
                            }
                        }
                    }
                    else
                    {
                        return ResponseResult.Error("Error In Closing Opened Period.No Opened Period");
                    }

                    if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
                    {
                        yearStartDate = oCompany.StartYear;
                        yearEndDate = oCompany.EndYear;
                    }
                    else
                    {
                        return ResponseResult.Error( oCompany.ErrMessageToReturn.Trim());
                     }

                    if (oCompany.GetEOMRunDate() != DateTime.MinValue)
                    {
                        monthDate = oCompany.GetEOMRunDate();
                    }
                    else
                    {
                        var err = oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "" ?
                        oCompany.ErrMessageToReturn.Trim() : "Cannot Run End Of Month.Table Not Set Properly";
                        return ResponseResult.Error(err);
                    }

                    if (strCheckMonthPostFlag.Trim() == "NO")
                    {
                        monthDate = DateTime.Now;
                     }
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();

                        if ((!oGlParam.GetGLParam()) || oGlParam.ReserveAcct == null || oGlParam.ReserveAcct.Trim() == "")
                        {
                            return ResponseResult.Error("Cannot Cloase Opened Period. GL Parameter Table Not Set Or Reserve A/C Missing");
                         }
                        var result = CheckCloseClosedPeriodParamIsCorrect(monthDate, yearStartDate, yearEndDate, oGlParam);
                        if (!result.IsSuccess) return result;
                        var oBucket = new Bucket();
                        oGlParam.Type = "CHKMONTHPOST";
                        oCompany.GetCompany(GeneralFunc.CompanyNumber);

                        oOpenCloseClosedPeriod.TransNo = oBucket.GetNextBucketNo("OPENCLOSECLOSEDPERIOD");
                        oOpenCloseClosedPeriod.OpenOrClosePeriod = "C";
                        oOpenCloseClosedPeriod.PastOrCurrentYear = "C";
                        oOpenCloseClosedPeriod.YearStartDate = oCompany.StartYear;
                        oOpenCloseClosedPeriod.YearEndDate = oCompany.EndYear;
                        if (strCheckMonthPostFlag.Trim() == "YES")
                        {
                            oOpenCloseClosedPeriod.MonthDate = oCompany.EOMRunDate;
                            oOpenCloseClosedPeriod.CurrentMonthDate = oCompany.CurrentMonthDate;
                        }
                        else
                        {
                            oOpenCloseClosedPeriod.MonthDate = DateTime.MinValue;
                            oOpenCloseClosedPeriod.CurrentMonthDate = DateTime.MinValue;
                        }
                        oOpenCloseClosedPeriod.CurrentYearStartDate = oCompany.CurrentYearStartDate;
                        oOpenCloseClosedPeriod.CurrentYearEndDate = oCompany.CurrentYearEndDate;

                        var oCommmandOpenClosePeriod = oOpenCloseClosedPeriod.AddCommand();

                        var oCommandUpdateOpenClosedMonthPeriod = oCompany.UpdateCloseClosedMonthPeriodCommand();
                        db.ExecuteNonQuery(oCommandUpdateOpenClosedMonthPeriod, transaction);

                        transaction.Commit();
                        return ResponseResult.Success();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    transaction?.Rollback();
                    return ResponseResult.Error($"Error In Running End Of Month. {ex.Message}");
                }

            });
        }

        private ResponseResult CheckCloseClosedPeriodParamIsCorrect(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate, GLParam oGlParam)
        {
            var oAcctGl = new AcctGL();
             
            oGlParam.Type = "CHKMONTHPOST";
            var strCheckMonthPostFlag = oGlParam.CheckParameter();
            if (strCheckMonthPostFlag.Trim() == "YES")
            { 
                if (monthDate > GeneralFunc.GetTodayDate())
                {
                    return ResponseResult.Error("Cannot Open Closed Period, Month Run Date Is In The Future");
                 }
                if (!(yearStartDate <= monthDate
                    && yearEndDate >= monthDate))
                {
                    return ResponseResult.Error("Cannot Open Closed Period,Month Date Must Be A Date Within " +
                        "The Start Of Year And End Of Year Period");
                }
            }
            if (yearEndDate < yearStartDate)
            {
                return ResponseResult.Error("Cannot Open Closed Period,End Of Year Date Cannot Be Lass Than Start Of Year");
            }
            if (yearEndDate > GeneralFunc.GetTodayDate())
            {
                return ResponseResult.Error("Cannot Open Closed Period,End Of Year Date Is In The Future");
            }

            if (yearStartDate
                .AddYears(1).AddDays(-1) == yearEndDate)
            {
            }
            else
            {
                return ResponseResult.Error("Cannot Open Closed Period. Start And End Year Interval Must Be One Year");
            }
            var decTotalBal = Math.Round(oAcctGl.GetTotalBalanceForAllAccountByDateRange(yearStartDate,
                yearEndDate), 2);
            if (decTotalBal == 0)
            {
            }
            else
            {
                return ResponseResult.Error("Cannot Open Closed Period.Trial Balance Unbalance By " + decTotalBal.ToString("n") + " Please Contact Your Software Vendor");
            }
            return ResponseResult.Success();
        }

        private ResponseResult CheckOpenClosedPeriodParamIsCorrect(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate, GLParam oGlParam)
        {
            var oAcctGl = new AcctGL();
             
            oGlParam.Type = "CHKMONTHPOST";
            var strCheckMonthPostFlag = oGlParam.CheckParameter();
            if (strCheckMonthPostFlag.Trim() == "YES")
            {
                 
                if (monthDate > GeneralFunc.GetTodayDate())
                {
                    return ResponseResult.Error("Cannot Open Closed Period,Month Run Date Is In The Future");
                }
                if (!(yearStartDate <= monthDate
                    && yearEndDate >= monthDate))
                {
                    return ResponseResult.Error("Cannot Open Closed Period,Month Date Must Be A Date Within " +
                        "The Start Of Year And End Of Year Period");
                }
            }
            if (yearEndDate < yearStartDate)
            {
                return ResponseResult.Error("Cannot Open Closed Period,End Of Year Date Cannot Be Lass Than Start Of Year");
            }
            if (yearEndDate > GeneralFunc.GetTodayDate())
            {
                return ResponseResult.Error("Cannot Open Closed Period,End Of Year Date Is In The Future");
            }

            if (yearStartDate
                .AddYears(1).AddDays(-1) == yearEndDate)
            {
            }
            else
            {
                return ResponseResult.Error("Cannot Open Closed Period. Start And End Year Interval Must Be One Year");
            }
            var decTotalBal = Math.Round(oAcctGl.GetTotalBalanceForAllAccountByDateRange(yearStartDate,yearEndDate), 2);
            if (decTotalBal == 0)
            {
            }
            else
            {
                return ResponseResult.Error("Cannot Open Closed Period.Trial Balance Unbalance By " + 
                    decTotalBal.ToString("n") + " Please Contact Your Software Vendor");
            }
            return ResponseResult.Success();

        }

        #region Close Closed Period Post Profit Loss Balance
        private void CloseClosedPostProfitLossBalance(SqlDatabase db, SqlTransaction transaction,
            DateTime yearStartDate, DateTime yearEndDate, Branch oBranch, GLParam oGlParam)
        {
            var oGl = new AcctGL
            {
                ByPassYearAndMonthPeriodCheck = true
            };
            var  oDs = oGl.GetAllProfitLossBalance(yearStartDate, yearEndDate);
            var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
            db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(oCommandJnumber, transaction);
            var strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

            decimal decNetProfitLoss = 0;
            foreach (DataRow oRow in oDs.Tables[0].Rows)
            {
                if (decimal.Parse(oRow["NetAmount"].ToString()) < 0)
                {
                    decNetProfitLoss = decNetProfitLoss + Math.Round(decimal.Parse(oRow["NetAmount"].ToString()), 2);
                    oGl.EffectiveDate = yearEndDate;
                    oGl.AccountID = "";
                    oGl.MasterID = oRow["MasterId"].ToString();
                    oGl.AcctRef = "";
                    oGl.Credit = Math.Round(Math.Abs(decimal.Parse(oRow["NetAmount"].ToString())), 2);
                    oGl.Debit = 0;
                    oGl.Debcred = "C";
                    oGl.Desciption = "EOY Closing for " + yearEndDate.Year;
                    oGl.TransType = "PLYEARUN";
                    oGl.SysRef = "YRRUN" + "-" + yearEndDate.Day.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Month.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Year.ToString().PadLeft(4, char.Parse("0"));
                    oGl.Ref01 = "";
                    oGl.Ref02 = "";
                    oGl.Chqno = "";
                    oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGl.RecAcctMaster = oGl.MasterID;
                    oGl.RecAcct = "";
                    oGl.Reverse = "N";
                    oGl.Jnumber = strJnumberNext;
                    oGl.Branch = oBranch.DefaultBranch;
                    oGl.Reverse = "N";
                    oGl.FeeType = "PLACCT";
                    oGl.OverrideEOM = "Y";
                    var dbCommandCredit = oGl.AddCommand();
                    db.ExecuteNonQuery(dbCommandCredit, transaction);
                }
                else if (decimal.Parse(oRow["NetAmount"].ToString()) > 0)
                {
                    decNetProfitLoss = decNetProfitLoss + Math.Round(decimal.Parse(oRow["NetAmount"].ToString()), 2);
                    oGl.EffectiveDate = yearEndDate;
                    oGl.AccountID = "";
                    oGl.MasterID = oRow["MasterId"].ToString();
                    oGl.AcctRef = "";
                    oGl.Credit = 0;
                    oGl.Debit = Math.Round(Math.Abs(decimal.Parse(oRow["NetAmount"].ToString())), 2);
                    oGl.Debcred = "D";
                    oGl.Desciption = "EOY Closing for " + yearEndDate.Year;
                    oGl.TransType = "PLYEARUN";
                    oGl.SysRef = "YRRUN" + "-" + yearEndDate.Day.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Month.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Year.ToString().PadLeft(4, char.Parse("0"));
                    oGl.Ref01 = "";
                    oGl.Ref02 = "";
                    oGl.Chqno = "";
                    oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGl.RecAcctMaster = oGl.MasterID;
                    oGl.RecAcct = "";
                    oGl.AcctRefSecond = "";
                    oGl.Reverse = "N";
                    oGl.Jnumber = strJnumberNext;
                    oGl.Branch = oBranch.DefaultBranch;
                    oGl.Reverse = "N";
                    oGl.FeeType = "PLACCT";
                    oGl.OverrideEOM = "Y";
                    var dbCommandDebit = oGl.AddCommand();
                    db.ExecuteNonQuery(dbCommandDebit, transaction);
                }
             }
            decNetProfitLoss = Math.Round(decNetProfitLoss, 2);
            if (decNetProfitLoss != 0)
            {
                oGl.EffectiveDate = yearEndDate;
                oGl.AccountID = "";
                oGl.MasterID = oGlParam.ReserveAcct;
                oGl.AcctRef = "";
                if (decNetProfitLoss > 0)
                {
                    oGl.Credit = Math.Round(Math.Abs(decNetProfitLoss), 2);
                    oGl.Debit = 0;
                    oGl.Debcred = "C";
                }
                else
                {
                    oGl.Credit = 0;
                    oGl.Debit = Math.Round(Math.Abs(decNetProfitLoss), 2);
                    oGl.Debcred = "D";
                }
                oGl.Desciption = "EOY P/L Earning for " + yearEndDate.Year;
                oGl.TransType = "PLYEARUN";
                oGl.SysRef = "YRRUN" + "-" + yearEndDate.Day.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Month.ToString().PadLeft(2, char.Parse("0")) + yearEndDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGl.Ref01 = "";
                oGl.Ref02 = "";
                oGl.Chqno = "";
                oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGl.RecAcctMaster = oGl.MasterID;
                oGl.RecAcct = "";
                oGl.AcctRefSecond = "";
                oGl.Reverse = "N";
                oGl.Jnumber = strJnumberNext;
                oGl.Branch = oBranch.DefaultBranch;
                oGl.Reverse = "N";
                oGl.FeeType = "REVACCT";
                oGl.OverrideEOM = "Y";
                var dbCommandReserve = oGl.AddCommand();
                db.ExecuteNonQuery(dbCommandReserve, transaction);
            }
        }
        #endregion

        #region Close Closed Period Save To Account Balances Table
        private void CloseClosedSaveToAccountBalancesTable(SqlDatabase db, SqlTransaction transaction, DateTime yearEndDate)
        {
            //When Upgrading EOY Remove This and Only Update Only 
            //Zerorise Last Column, Update Current Column,Update Financial Date,Add New Record
            var oAccountBalance = new AccountBalance();
            var dbCommandDeleteAcctBal = oAccountBalance.DeleteCommand();
            db.ExecuteNonQuery(dbCommandDeleteAcctBal, transaction);

            var dbCommandAddGlAcctBal = oAccountBalance.AddAllGLAccountBalance(yearEndDate);
            db.ExecuteNonQuery(dbCommandAddGlAcctBal, transaction);

            var dbCommandAddCustomerAcctBal = oAccountBalance.AddAllCustomerAccountBalance(yearEndDate);
            db.ExecuteNonQuery(dbCommandAddCustomerAcctBal, transaction);
        }
        #endregion

        #region Save To Account Balances Table
        private void SaveToAccountBalancesTable(SqlDatabase db, SqlTransaction transaction, DateTime endDate)
        {
            //When Upgrading EOY Remove This and Only Update Only 
            //Zerorise Last Column, Update Current Column,Update Financial Date,Add New Record
            var oAccountBalance = new AccountBalance();
            var dbCommandDeleteAcctBal = oAccountBalance.DeleteCommand();
            db.ExecuteNonQuery(dbCommandDeleteAcctBal, transaction);

            var dbCommandAddGlAcctBal = oAccountBalance.AddAllGLAccountBalance(endDate);
            db.ExecuteNonQuery(dbCommandAddGlAcctBal, transaction);

            var dbCommandAddCustomerAcctBal = oAccountBalance.AddAllCustomerAccountBalance(endDate);
            db.ExecuteNonQuery(dbCommandAddCustomerAcctBal, transaction);

        }
        #endregion

        #region Post Profit Loss Balance
        private void PostProfitLossBalance(SqlDatabase db, SqlTransaction transaction, DateTime startDate, DateTime endDate,
            Branch oBranch, GLParam oGlParam)
        {
            var oGl = new AcctGL();
            var  oDs = oGl.GetAllProfitLossBalance(startDate, endDate);
            var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
            db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(oCommandJnumber, transaction);
            var strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

            decimal decNetProfitLoss = 0;
            oGl.ByPassYearAndMonthPeriodCheck = true;
            foreach (DataRow oRow in oDs.Tables[0].Rows)
            {
                if (decimal.Parse(oRow["NetAmount"].ToString()) < 0)
                {
                    decNetProfitLoss = decNetProfitLoss + Math.Round(decimal.Parse(oRow["NetAmount"].ToString()), 2);
                    //When Error Of Date Show Not In Fianacial Year ByPass Check By Putting a Value to be true
                    //Then If True it will bypass the check For Financial Year Range and Month Range.
                    oGl.EffectiveDate = endDate;
                    oGl.AccountID = "";
                    oGl.MasterID = oRow["MasterId"].ToString();
                    oGl.AcctRef = "";
                    oGl.Credit = Math.Round(Math.Abs(decimal.Parse(oRow["NetAmount"].ToString())), 2);
                    oGl.Debit = 0;
                    oGl.Debcred = "C";
                    oGl.Desciption = "EOY Closing for " + endDate.Year;
                    oGl.TransType = "PLYEARUN";
                    oGl.SysRef = "YRRUN" + "-" + endDate.Day.ToString().PadLeft(2, char.Parse("0")) + endDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                        endDate.Year.ToString().PadLeft(4, char.Parse("0"));
                    oGl.Ref01 = "";
                    oGl.Ref02 = "";
                    oGl.Chqno = "";
                    oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGl.RecAcctMaster = oGl.MasterID;
                    oGl.RecAcct = "";
                    oGl.Reverse = "N";
                    oGl.Jnumber = strJnumberNext;
                    oGl.Branch = oBranch.DefaultBranch;
                    oGl.Reverse = "N";
                    oGl.FeeType = "PLACCT";
                    oGl.OverrideEOM = "Y";
                    var dbCommandCredit = oGl.AddCommand();
                    db.ExecuteNonQuery(dbCommandCredit, transaction);
                }
                else if (decimal.Parse(oRow["NetAmount"].ToString()) > 0)
                {
                    decNetProfitLoss = decNetProfitLoss + Math.Round(decimal.Parse(oRow["NetAmount"].ToString()), 2);
                    oGl.EffectiveDate = endDate;
                    oGl.AccountID = "";
                    oGl.MasterID = oRow["MasterId"].ToString();
                    oGl.AcctRef = "";
                    oGl.Credit = 0;
                    oGl.Debit = Math.Round(Math.Abs(decimal.Parse(oRow["NetAmount"].ToString())), 2);
                    oGl.Debcred = "D";
                    oGl.Desciption = "EOY Closing for " + endDate.Year;
                    oGl.TransType = "PLYEARUN";
                    oGl.SysRef = "YRRUN" + "-" + endDate.Day.ToString().PadLeft(2, char.Parse("0")) + endDate.Month.ToString().PadLeft(2, char.Parse("0")) + endDate.Year.ToString().PadLeft(4, char.Parse("0"));
                    oGl.Ref01 = "";
                    oGl.Ref02 = "";
                    oGl.Chqno = "";
                    oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGl.RecAcctMaster = oGl.MasterID;
                    oGl.RecAcct = "";
                    oGl.AcctRefSecond = "";
                    oGl.Reverse = "N";
                    oGl.Jnumber = strJnumberNext;
                    oGl.Branch = oBranch.DefaultBranch;
                    oGl.Reverse = "N";
                    oGl.FeeType = "PLACCT";
                    oGl.OverrideEOM = "Y";
                    var dbCommandDebit = oGl.AddCommand();
                    db.ExecuteNonQuery(dbCommandDebit, transaction);
                }
            }
            decNetProfitLoss = Math.Round(decNetProfitLoss, 2);
            if (decNetProfitLoss != 0)
            {
                oGl.EffectiveDate = endDate;
                oGl.AccountID = "";
                oGl.MasterID = oGlParam.ReserveAcct;
                oGl.AcctRef = "";
                if (decNetProfitLoss > 0)
                {
                    oGl.Credit = Math.Round(Math.Abs(decNetProfitLoss), 2);
                    oGl.Debit = 0;
                    oGl.Debcred = "C";
                }
                else
                {
                    oGl.Credit = 0;
                    oGl.Debit = Math.Round(Math.Abs(decNetProfitLoss), 2);
                    oGl.Debcred = "D";
                }
                oGl.Desciption = "EOY P/L Earning for " + startDate.Year;
                oGl.TransType = "PLYEARUN";
                oGl.SysRef = "YRRUN" + "-" + endDate.Day.ToString().PadLeft(2, char.Parse("0")) + endDate.Month.ToString().PadLeft(2, char.Parse("0")) + endDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGl.Ref01 = "";
                oGl.Ref02 = "";
                oGl.Chqno = "";
                oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGl.RecAcctMaster = oGl.MasterID;
                oGl.RecAcct = "";
                oGl.AcctRefSecond = "";
                oGl.Reverse = "N";
                oGl.Jnumber = strJnumberNext;
                oGl.Branch = oBranch.DefaultBranch;
                oGl.Reverse = "N";
                oGl.FeeType = "REVACCT";
                oGl.OverrideEOM = "Y";
                var dbCommandReserve = oGl.AddCommand();
                db.ExecuteNonQuery(dbCommandReserve, transaction);
            }
        }
        #endregion

        #region Zerorise Prop Holding
        private void ZerorisePropHolding(SqlDatabase db, SqlTransaction transaction, DateTime endDate)
        {
            var oGlParam = new GLParam();
            oGlParam.Type = "ZERORISEPROPHOLDINGENDOFYEAR";
            if (oGlParam.CheckParameter().Trim() == "YES")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                IFormatProvider format = new CultureInfo("en-GB");
                var datYearDate = endDate;
                decimal decPrice;
                string strDescription;
                var oCompany = new Company();
                if (oCompany.UpdatePortfolioDate(datYearDate))
                {
                    SqlCommand dbCommandPortfolio = null;
                    dbCommandPortfolio = db.GetStoredProcCommand("PortfolioSelectByProductCode") as SqlCommand;
                    db.AddInParameter(dbCommandPortfolio, "ProductCode", SqlDbType.VarChar, GetProductInvestment(db).Trim());
                    var oDs = db.ExecuteDataSet(dbCommandPortfolio);

                    foreach (DataRow oRow in oDs.Tables[0].Rows)
                    {
                        if (oRow["Total Holding"].ToString().Trim() != "" && decimal.Parse(oRow["Total Holding"].ToString()) != 0)
                        {

                            decPrice = decimal.Parse(GetUnitCost(datYearDate, oRow["CustNo"].ToString().Trim(), oRow["Stock"].ToString().Trim()).ToString());
                            strDescription = "Closing Balance For Year " + datYearDate.Year + " of " + oRow["Stock"].ToString().Trim() + " @ " + decPrice.ToString();
                            var oCommand = db.GetStoredProcCommand("PortfolioAdd") as SqlCommand;
                            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datYearDate);
                            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, oRow["CustNo"].ToString().Trim());
                            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, oRow["Stock"].ToString().Trim());
                            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, long.Parse(oRow["Total Holding"].ToString()));
                            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, float.Parse(decPrice.ToString()));
                            db.AddInParameter(oCommand, "ActualUnitCost", SqlDbType.Float, float.Parse(decPrice.ToString()));
                            db.AddInParameter(oCommand, "TotalCost", SqlDbType.Decimal, decPrice * long.Parse(oRow["Total Holding"].ToString()));
                            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, "YRRUN" + "-" + endDate.Day.ToString().PadLeft(2, char.Parse("0")) + endDate.Month.ToString().PadLeft(2, char.Parse("0")) + endDate.Year.ToString().PadLeft(4, char.Parse("0")).Trim());
                            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, "YEAREND");
                            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, endDate.Day.ToString().PadLeft(2, char.Parse("0")) +endDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                                endDate.Year.ToString().PadLeft(4, char.Parse("0")).Trim());
                            db.AddInParameter(oCommand, "TransDesc", SqlDbType.VarChar, strDescription.Trim());
                            if (decimal.Parse(oRow["Total Holding"].ToString()) > 0)
                            {
                                db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, "D");
                            }
                            else
                            {
                                db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, "C");
                            }
                            db.AddInParameter(oCommand, "MarginCode", SqlDbType.VarChar, "");
                            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, "");
                            db.ExecuteNonQuery(oCommand, transaction);

                        }
                    }
                }
            }
        }
        #endregion

        #region Calculate Unit Cost For Investment Account Only
        public double GetUnitCost(DateTime datPurchaseDate, string strCustomerAcct, string strStockCode)
        {
            string strCalcNormalAvgPrice;
            var oGlParam = new GLParam();
            oGlParam.Type = "CALCNORMALAVGPRICE";
            strCalcNormalAvgPrice = oGlParam.CheckParameter();

            double decRealUnitCost = 0;
            long intHolding = 0;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("PortfolioTransactionHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            var thisTable = db.ExecuteDataSet(dbCommand).Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length >= 1)
            {
                foreach (DataRow oRow in thisTable.Rows)
                {
                    if (oRow["DebCred"].ToString().Trim() == "C")
                    {
                        decRealUnitCost = (((intHolding * decRealUnitCost)
                        + (long.Parse(oRow["Units"].ToString().Trim()) * float.Parse(oRow["Actual Unit Cost"].ToString().Trim())))
                        / (intHolding + long.Parse(oRow["Units"].ToString().Trim())));
                        decRealUnitCost = Math.Round(decRealUnitCost, 2);
                        intHolding = intHolding + long.Parse(oRow["Units"].ToString().Trim());
                    }
                    else if (oRow["DebCred"].ToString().Trim() == "D")
                    {
                        if (strCalcNormalAvgPrice.Trim() == "YES")
                        { }
                        else
                        {
                            intHolding = intHolding - long.Parse(oRow["Units"].ToString().Trim());
                            if (intHolding < 0)
                            {
                                intHolding = 0;
                            }
                        }
                    }
                }

            }
            return !double.IsNaN(decRealUnitCost) ? decRealUnitCost : 0;
        }
        #endregion

        public string GetProductInvestment(SqlDatabase db)
        {
            var dbCommand = db.GetStoredProcCommand("PAGentableSelectInvestment") as SqlCommand;
            return (string)db.ExecuteScalar(dbCommand).ToString();
        }


        #region Check Param Is Correct
        private ResponseResult CheckEoYParamIsCorrect(DateTime startDate, DateTime endDate)
        {
            var oAcctGl = new AcctGL();
            
            if (endDate> GeneralFunc.GetTodayDate())
            {
                return ResponseResult.Error( "Cannot Run End Of Year,End Of Year Date Is In The Future");
            }

            if (startDate
                .AddYears(1).AddDays(-1) == endDate)
            {
                
            }
            else
            {
                return ResponseResult.Error( "Cannot Run End Of Year. Start And End Year Interval Must Be One Year");
            }
            var decTotalBal = Math.Round(oAcctGl.GetTotalBalanceForAllAccountByDateRange(startDate,endDate), 2);
            if (decTotalBal == 0)
            {
                
            }
            else
            {
                return ResponseResult.Error("Cannot Run End Of Year.Trial Balance Unbalance By " +
                    decTotalBal.ToString("n") + " Please Contact Your Software Vendor");
            }

            var oEoyRun = new EOYRun();
            oEoyRun.RunStartDate = startDate;
            oEoyRun.RunEndDate = endDate;
            oEoyRun.RunType = "E";
            if (oEoyRun.CheckDateInEOYRun())
            {
                return ResponseResult.Error("Cannot Run End Of Year. End Of Year Already Run For This Period");
            }

            return ResponseResult.Success();
        }

        #endregion

    }
}
