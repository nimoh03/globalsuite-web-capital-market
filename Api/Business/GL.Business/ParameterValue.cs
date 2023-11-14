using System;
using System.Data;
using BaseUtility.Business;

namespace GL.Business
{
    public class ParameterValue
    {
        #region Properties
        public Int64 ParameterValueId { get; set; }
        public Int32 PeriodInterval { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Get
        public bool GetParameterValue()
        {
            bool blnStatus = false;
            DataSet oDS = GeneralFunc.GetAll("GenParameterValue");
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ParameterValueId = Convert.ToInt64(thisRow[0]["ParameterValueId"]);
                PeriodInterval = Convert.ToInt32(thisRow[0]["PeriodInterval"]);
            }
            return blnStatus;
        }
        #endregion

    }
}
