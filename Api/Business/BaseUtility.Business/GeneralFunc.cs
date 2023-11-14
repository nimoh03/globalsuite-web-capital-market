using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace BaseUtility.Business
{
    public class GeneralFunc
    {
        static IFormatProvider format = new CultureInfo("en-GB");
        public static string UserName;
        public static int CompanyNumber;
        public static string UserBranchNumber;
        public static string DefaultBranch;
        public static string EmptyControl = "CANBEEMPTY";
        public static DateTime ExpireDate = DateTime.ParseExact("01/09/9994","dd/MM/yyyy",format) ;
        public static string ReportName;
        public static string FormModuleName;
        
        public static List<AccountNumberWithDetail> lstAccountNumberWithDetail = new List<AccountNumberWithDetail>();

        public static string EncryptOld(string inp)
        {
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
            byte[] tBytes = Encoding.ASCII.GetBytes(inp.Trim());
            byte[] hBytes = hasher.ComputeHash(tBytes);

            StringBuilder sb = new StringBuilder();
            for (int c = 0; c < hBytes.Length; c++)
                sb.AppendFormat("{0:x2}", hBytes[c]);

            return (sb.ToString());
        }

        public static string Decrypt2(string inp)
        {

            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
            byte[] tBytesEncrypt = Encoding.ASCII.GetBytes("2");
            byte[] hBytesEncrypt = hasher.ComputeHash(tBytesEncrypt);

            StringBuilder sb = new StringBuilder();
            for (int c = 0; c < hBytesEncrypt.Length; c++)
            {
                string test = hBytesEncrypt[c].ToString();
                //sb.AppendFormat("{0:x2}", hBytesEncrypt[c]);
                sb.AppendFormat("{0}", hBytesEncrypt[c]);
            }

            string strEncryptValue =  sb.ToString();


            var hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyArray = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(strEncryptValue));
            





            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            byte[] byteBuff = Convert.FromBase64String(strEncryptValue);

            objDESCrypto.KeySize = 128;
            objDESCrypto.Key = keyArray;
            objDESCrypto.Mode = CipherMode.CBC;
            objDESCrypto.Padding = PaddingMode.None;
            //ICryptoTransform cTransform = objDESCrypto.CreateDecryptor();
            //byte[] resultArray = cTransform.TransformFinalBlock(byteBuff, 0, byteBuff.Length);
            string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            hashmd5.Clear();
            objDESCrypto.Clear();
            //string strDecrypted = Encoding.UTF8.GetString(resultArray);
            return strDecrypted;

            

            //TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            //var hashmd5 = new MD5CryptoServiceProvider();
            //byte[] tBytes = Encoding.ASCII.GetBytes(strEncryptValue.Trim());
            //byte[] hBytes = hashmd5.ComputeHash(tBytes);
            //objDESCrypto.Key = hBytes;
            //string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(tBytes, 0, tBytes.Length));
            //return strDecrypted;
        }

        public static string EncryptCustomer(string strToEncrypt)
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

            byte[] byteHash, byteBuff;
            string strTempKey = "KeySHPEnt";

            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

            byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }

        public static string DecryptOld(string strEncrypted)
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            var hashmd5 = new MD5CryptoServiceProvider();
            byte[] byteBuff = Convert.FromBase64String(strEncrypted);
            byte[] keyArray = hashmd5.ComputeHash(Encoding.ASCII.GetBytes(strEncrypted));
            hashmd5.Clear();
            objDESCrypto.Key = keyArray;
            objDESCrypto.Mode = CipherMode.CBC;
            //objDESCrypto.Padding = PaddingMode.ANSIX923;
            //ICryptoTransform cTransform = objDESCrypto.CreateDecryptor();
            //byte[] resultArray = cTransform.TransformFinalBlock(byteBuff, 0, byteBuff.Length);
            string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            objDESCrypto.Clear();
            //string strDecrypted = Encoding.UTF8.GetString(resultArray);
            return strDecrypted;
        }

        public static string ConvertUCase(string input)
        {
            return (input.ToUpper());
        }

        public static string GetMonthName(int input)
        {
            string strResult="";
            if (input == 1)
            {
                strResult = "January";
            }
            else if (input == 2)
            {
                strResult = "Febuary";
            }
            else if (input == 3)
            {
                strResult = "March";
            }
            else if (input == 4)
            {
                strResult = "April";
            }
            else if (input == 5)
            {
                strResult = "May";
            }
            else if (input == 6)
            {
                strResult = "June";
            }
            else if (input == 7)
            {
                strResult = "July";
            }
            else if (input == 8)
            {
                strResult = "August";
            }
            else if (input == 9)
            {
                strResult = "September";
            }
            else if (input == 10)
            {
                strResult = "October";
            }
            else if (input == 11)
            {
                strResult = "November";
            }
            else if (input == 12)
            {
                strResult = "December";
            }
            return strResult;
        }

        public static DateTime GetTodayDate()
        {
            IFormatProvider format = new CultureInfo("en-GB", true);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GetTodayDate") as SqlCommand;
            return DateTime.ParseExact(db.ExecuteScalar(oCommand).ToString(),"dd/MM/yyyy",format);
        }

        public static int GetTodayTimeHour()
        {
            IFormatProvider format = new CultureInfo("en-GB", true);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GetTodayTimeHour") as SqlCommand;
            return int.Parse(db.ExecuteScalar(oCommand).ToString());
        }

        public static string GetMonthLastDay(int input,int inputYear)
        {
            string strResult = "";
            if (input == 1)
            {
                strResult = "31";
            }
            else if (input == 2)
            {
                if (inputYear % 4 == 0)
                {
                    strResult = "29";
                }
                else
                {
                    strResult = "28";
                }
            }
            else if (input == 3)
            {
                strResult = "31";
            }
            else if (input == 4)
            {
                strResult = "30";
            }
            else if (input == 5)
            {
                strResult = "31";
            }
            else if (input == 6)
            {
                strResult = "30";
            }
            else if (input == 7)
            {
                strResult = "31";
            }
            else if (input == 8)
            {
                strResult = "31";
            }
            else if (input == 9)
            {
                strResult = "30";
            }
            else if (input == 10)
            {
                strResult = "31";
            }
            else if (input == 11)
            {
                strResult = "30";
            }
            else if (input == 12)
            {
                strResult = "31";
            }
            return strResult;
        }

        public DateTime AddBusinessDay(DateTime start, int count,IEnumerable<DateTime> holidays)
        {
            int daysToAdd = count + ((count / 6) * 2) + ((((int)start.DayOfWeek + (count % 6)) >= 6) ? 2 : 0);
            var end = start.AddDays(daysToAdd);
            foreach (var dt in holidays)
            {
                if (dt >= start && dt <= end)
                {
                    end = end.AddDays(1);
                    if (end.DayOfWeek == DayOfWeek.Saturday)
                    {
                        end = end.AddDays(2);
                    }
                    else if (end.DayOfWeek == DayOfWeek.Sunday)
                    {
                        end = end.AddDays(1);
                    }
                }
            }
            return end;
        }

        public DateTime MinusBusinessDay(DateTime start, int count, IEnumerable<DateTime> holidays)
        {
            var end = start.AddDays(-count);
            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                end = end.AddDays(-2);
            }
            foreach (var dt in holidays)
            {
                if (dt <= start && dt >= end)
                {
                    end = end.AddDays(-1);
                    if (end.DayOfWeek == DayOfWeek.Saturday)
                    {
                        end = end.AddDays(-1);
                    }
                    else if (end.DayOfWeek == DayOfWeek.Sunday)
                    {
                        end = end.AddDays(-2);
                    }
                }
            }
            return end;
        }

        public DateTime MinusBusinessDayForPriceList(DateTime start, int count, IEnumerable<DateTime> holidays, DateTime lastdate)
        {
            var end = start.AddDays(-count);
            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                end = end.AddDays(-2);
            }
            foreach (var dt in holidays)
            {
                if (dt <= start && dt >= lastdate)
                {
                    end = end.AddDays(-1);
                    if (end.DayOfWeek == DayOfWeek.Saturday)
                    {
                        end = end.AddDays(-1);
                    }
                    else if (end.DayOfWeek == DayOfWeek.Sunday)
                    {
                        end = end.AddDays(-2);
                    }
                }
            }
            return end;
        }

        public DateTime AddBusinessDays2(DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == DayOfWeek.Saturday)
                    dt = dt.AddDays(2);
            }
            return dt.AddDays(weeks * 7);
        }

        public DateTime MinusBusinessDays2(DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(-1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(-1);
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                    dt = dt.AddDays(-2);
            }
            return dt.AddDays(weeks * -7);
        }

        public bool ChkIsWeekend(DateTime dt)
        {
            bool blnStatus = false;
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            {
                blnStatus = true;
            }
            return blnStatus;
        }

        public static DataTable GetMonths()
        {
            DataTable dtMonth = new DataTable("ALLMONTH");
            DataColumn myDataColumn;
            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int16");
            myDataColumn.ColumnName = "MONTHNO";
            dtMonth.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "MONTHNAME";
            dtMonth.Columns.Add(myDataColumn);

            DataRow oRowMonth;
            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 1;
            oRowMonth["MONTHNAME"] = "JANUARY";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 2;
            oRowMonth["MONTHNAME"] = "FEBUARY";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 3;
            oRowMonth["MONTHNAME"] = "MARCH";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 4;
            oRowMonth["MONTHNAME"] = "APRIL";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 5;
            oRowMonth["MONTHNAME"] = "MAY";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 6;
            oRowMonth["MONTHNAME"] = "JUNE";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 7;
            oRowMonth["MONTHNAME"] = "JULY";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 8;
            oRowMonth["MONTHNAME"] = "AUGUST";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 9;
            oRowMonth["MONTHNAME"] = "SEPTEMBER";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 10;
            oRowMonth["MONTHNAME"] = "OCTOBER";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 11;
            oRowMonth["MONTHNAME"] = "NOVEMBER";
            dtMonth.Rows.Add(oRowMonth);

            oRowMonth = dtMonth.NewRow();
            oRowMonth["MONTHNO"] = 12;
            oRowMonth["MONTHNAME"] = "DECEMBER";
            dtMonth.Rows.Add(oRowMonth);

            return dtMonth;
        }

        #region Create Random Password
        public static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        #endregion

        public static object[,] ReadExcelFile(string FileName)
        {
            //Worksheet workSheet;
            Microsoft.Office.Interop.Excel.Application _excelApp = new Microsoft.Office.Interop.Excel.Application();
            object[,] valueArray = null;

            Microsoft.Office.Interop.Excel.Workbook workBook = _excelApp.Workbooks.Open(FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
                                                        "\t", false, false, 0, true, 1, 0);
            //int numSheets = workBook.Sheets.Count;

            //
            // Iterate through the sheets. They are indexed starting at 1.

            //This is Done Intensionally to Read one Sheet , Implement multiple in future
            for (int sheetNum = 1; sheetNum <= 1; sheetNum++)
            {
                Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Sheets[sheetNum];

                //
                // Take the used range of the sheet. Finally, get an object array of all
                // of the cells in the sheet (their values). You can do things with those
                // values. See notes about compatibility.
                //
                Microsoft.Office.Interop.Excel.Range excelRange = sheet.UsedRange;
                valueArray = (object[,])excelRange.get_Value(
                    Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);
                //TempArray.CopyTo(valueArray,0);
                ////
                //// Do something with the data in the array with a custom method.
                ////
                //ProcessObjects(valueArray);

            }
            return valueArray;
        }

        #region Customer Direct Cash Settlement And Do Not Charge Stamp Duty Update And Return Command
        public SqlCommand CustomerDirectCashSettlementAndNoStampDutyUpdateCommand(string strCustomerNumber,
            bool blnDirectCashStatus, bool blnDirectCashStatusNASD,
            bool blnDoNotChargeStampDuty,DateTime datDCSSetupDate, DateTime datDCSSetupDateNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommandCustomerDCS = new SqlCommand();
            oCommandCustomerDCS = db.GetStoredProcCommand("CustomerExtraInformationDirectCashSettleUpdate") as SqlCommand;
            db.AddInParameter(oCommandCustomerDCS, "CustomerNo", SqlDbType.VarChar, strCustomerNumber.Trim());
            db.AddInParameter(oCommandCustomerDCS, "DirectCashStatus", SqlDbType.Bit, blnDirectCashStatus);
            db.AddInParameter(oCommandCustomerDCS, "DirectCashStatusNASD", SqlDbType.Bit, blnDirectCashStatusNASD);
            db.AddInParameter(oCommandCustomerDCS, "DoNotChargeStampDuty", SqlDbType.Bit, blnDoNotChargeStampDuty);
            db.AddInParameter(oCommandCustomerDCS, "DCSSetupDate", SqlDbType.DateTime, datDCSSetupDate);
            db.AddInParameter(oCommandCustomerDCS, "DCSSetupDateNASD", SqlDbType.DateTime, datDCSSetupDateNASD);
            db.AddInParameter(oCommandCustomerDCS, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommandCustomerDCS;
        }
        #endregion

        #region Get All
        public static DataSet GetAll(string strTableName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelectAll") as SqlCommand;
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Posting
        public static DataSet GetAllPosting(string strTableName, DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("GenSelectAll") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Convert List To DataTable For The DataSet
        public static DataTable CreateTableFromList<T>(string name, IEnumerable<T> list)
        {
            PropertyInfo[] propInfo = typeof(T).GetProperties();
            DataTable table = Table<T>(name, list, propInfo);
            IEnumerator<T> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
                table.Rows.Add(CreateRow<T>(table.NewRow(), enumerator.Current, propInfo));
            return table;
        }
        #endregion
        private static DataRow CreateRow<T>(DataRow row, T listItem, PropertyInfo[] pi)
        {
            foreach (PropertyInfo p in pi)
            {
                if (p.Name.Trim() != "SaveType")
                {
                    row[p.Name.ToString()] = p.GetValue(listItem, null);
                }
            }
            return row;
        }
        private static DataTable Table<T>(string name, IEnumerable<T> list, PropertyInfo[] pi)
        {
            DataTable table = new DataTable(name);
            foreach (PropertyInfo p in pi)
            {
                if (p.Name.Trim() != "SaveType")
                {
                    table.Columns.Add(p.Name, p.PropertyType);
                }
            }
            return table;
        }

        public class AccountNumberWithDetail
        {
            public string AccountId { get; set; }
            public string AccountDetail { get; set; }
            public string IsParent { get; set; }
            public string IsInternalAccount { get; set; }
            public string AccountBranch { get; set; }
            public string ParentId { get; set; }

            #region Convert DataSet To List
            public List<AccountNumberWithDetail> ConvertDataSetToList(DataSet oDataSet)
            {
                List<AccountNumberWithDetail> lstAccountNumberWithDetail = oDataSet.Tables[0].AsEnumerable().Select(
                                oRow => new AccountNumberWithDetail
                                {
                                    AccountId = oRow["AccountID"].ToString(),
                                    AccountDetail = oRow["Description"].ToString() + " " + oRow["AccountID"].ToString(),
                                    IsParent = oRow["IsParent"].ToString(),
                                    IsInternalAccount = oRow["InternalAccount"].ToString(),
                                    AccountBranch = oRow["BranchId"].ToString(),
                                    ParentId = oRow["ParentId"].ToString(),
                                }).ToList();
                return lstAccountNumberWithDetail;
            }
            #endregion
        }

    }
}
