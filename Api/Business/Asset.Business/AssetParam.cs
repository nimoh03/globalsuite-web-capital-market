using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Asset.Business
{
    public class AssetParam
    {

        #region Declaration
        private string strPortfolioManagementProduct;
        private string strStatChargeAccount;
        private string strMutualFundAccount;
        #endregion

        #region Properties

        

        public string PortfolioManagementProduct
        {
            set { strPortfolioManagementProduct = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("AssetParameterSelectPortfolioManagementProduct") as SqlCommand;
                var varPortfolioManagementProduct = db.ExecuteScalar(dbCommand);
                strPortfolioManagementProduct = varPortfolioManagementProduct != null ? varPortfolioManagementProduct.ToString() : "";
                return strPortfolioManagementProduct;
            }

        }

        public string StatChargeAccount
        {
            set { strStatChargeAccount = value; }
            get { return strStatChargeAccount; }
        }

        public string MutualFundAccount
        {
            set { strMutualFundAccount = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("AssetParameterSelectMutualFundAccount") as SqlCommand;
                var varMutualFundAccount = db.ExecuteScalar(dbCommand);
                strMutualFundAccount = varMutualFundAccount != null ? varMutualFundAccount.ToString() : "";
                return strMutualFundAccount;
            }

        }
        #endregion

        #region Add Asset Parameter
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AssetParameterAdd") as SqlCommand;
            db.AddInParameter(oCommand, "PortfolioManagementProduct", SqlDbType.Char,strPortfolioManagementProduct);
            db.AddInParameter(oCommand, "StatChargeAccount", SqlDbType.VarChar, strStatChargeAccount);
            db.AddInParameter(oCommand, "MutualFundAccount", SqlDbType.VarChar, strMutualFundAccount);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get
        public bool GetAssetParameter()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AssetParemeterSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strPortfolioManagementProduct = thisRow[0]["PortfolioManagementProduct"] != null && thisRow[0]["PortfolioManagementProduct"].ToString().Trim() != "" ? thisRow[0]["PortfolioManagementProduct"].ToString() : "";
                strStatChargeAccount = thisRow[0]["StatChargeAccount"] != null && thisRow[0]["StatChargeAccount"].ToString().Trim() != "" ? thisRow[0]["StatChargeAccount"].ToString() : "";
                strMutualFundAccount = thisRow[0]["MutualFundAccount"] != null && thisRow[0]["MutualFundAccount"].ToString().Trim() != "" ? thisRow[0]["MutualFundAccount"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

    }
}
