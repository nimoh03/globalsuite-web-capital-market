using System;
using System.Globalization;
using BaseUtility.Business;

namespace GL.Business
{
    public class DateFunction
    {
        #region Check Date Is In Future
        public static bool ChkDateIsFuture(DateTime datDate)
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            if (datDate > GeneralFunc.GetTodayDate())
            {
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
