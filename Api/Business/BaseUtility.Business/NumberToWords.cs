using System;

namespace BaseUtility.Business
{
    public class NumberToWords
    {
        string Number;
        string deciml;
        string _number;
        string _deciml;
        string[] US = new string[1003];
        string[] SNu = new string[20];
        string[] SNt = new string[10];


        #region Convert To Words
        public string ConvertToWord(string Number2)
        {
            string strNumberName = "";
            Initialize();

            string currency = "Naira";
            string _currency = "kobo Only";

            if (Number2 != "")
            {
                if (Convert.ToDouble(Number2) == 0)
                {
                    strNumberName = "";
                }

                if (Convert.ToDouble(Number2) < 0)
                {
                    strNumberName = "";
                }

                if (!Number2.Contains("."))
                {
                    Number2 = Number2 + ".00";
                }
                Number2 = Number2.Replace(",", "");
                string[] no = Number2.Split('.');
                if ((no[0] != null) && (no[1] != "00"))
                {
                    Number = no[0];
                    deciml = no[1];
                    _number = (NameOfNumber(Number));
                    _deciml = (NameOfNumber(deciml));
                    strNumberName = _number.Trim() + " " + currency + " and " + _deciml.Trim() + " " + _currency;
                }
                if ((no[0] != null) && (no[1] == "00"))
                {
                    Number = no[0];
                    _number = (NameOfNumber(Number));
                    strNumberName = _number + " " + currency + " Only";
                }
                if ((Convert.ToDouble(no[0]) == 0) && (no[1] != null))
                {
                    deciml = no[1];
                    _deciml = (NameOfNumber(deciml));
                    strNumberName = _deciml + " " + _currency + " Only";
                }
            }
            return strNumberName;
        }
        #endregion

        #region Convert To Words No Only
        public string ConvertToWordNoOnly(string Number2)
        {
            string strNumberName = "";
            Initialize();

            string currency = "";
            string _currency = "";

            if (Convert.ToDouble(Number2) == 0)
            {
                strNumberName = "";

            }
            if (Convert.ToDouble(Number2) < 0)
            {
                strNumberName = "";
            }

            if (!Number2.Contains("."))
            {
                Number2 = Number2 + ".00";
            }
            Number2 = Number2.Replace(",", "");
            string[] no = Number2.Split('.');
            if ((no[0] != null) && (no[1] != "00"))
            {
                Number = no[0];
                deciml = no[1];
                _number = (NameOfNumber(Number));
                _deciml = (NameOfNumber(deciml));
                strNumberName = _number.Trim() + " " + currency + " and " + _deciml.Trim() + " " + _currency;
            }
            if ((no[0] != null) && (no[1] == "00"))
            {
                Number = no[0];
                _number = (NameOfNumber(Number));
                strNumberName = _number + " " + currency + " Units Only";
            }
            if ((Convert.ToDouble(no[0]) == 0) && (no[1] != null))
            {
                deciml = no[1];
                _deciml = (NameOfNumber(deciml));
                strNumberName = _deciml + " " + _currency + " Units Only";
            }
            return strNumberName;
        }
        #endregion

        #region Name To Number
        private string NameOfNumber(string Number)
        {

            string GroupName = "";
            string OutPut = "";

            if ((Number.Length % 3) != 0)
            {
                Number = Number.PadLeft((Number.Length + (3 - (Number.Length % 3))), '0');
            }
            string[] Array = new string[Number.Length / 3];
            Int16 Element = -1;
            Int32 DisplayCount = -1;
            bool LimitGroupsShowAll = false;
            int LimitGroups = 0;
            bool GroupToWords = true;
            for (Int16 Count = 0; Count <= Number.Length - 3; Count += 3)
            {
                Element += 1;
                Array[Element] = Number.Substring(Count, 3);

            }
            if (LimitGroups == 0)
            {
                LimitGroupsShowAll = true;
            }
            for (Int16 Count = 0; (Count <= ((Number.Length / 3) - 1)); Count++)
            {
                DisplayCount++;
                if (((DisplayCount < LimitGroups) || LimitGroupsShowAll))
                {
                    if (Array[Count] == "000") continue;
                    {
                        GroupName = US[((Number.Length / 3) - 1) - Count + 1];
                    }


                    if ((GroupToWords == true))
                    {
                        OutPut += Group(Array[Count]).TrimEnd(' ') + " " + GroupName + " ";

                    }
                    else
                    {
                        OutPut += Array[Count].TrimStart('0') + " " + GroupName;

                    }
                }

            }
            Array = null;
            return OutPut;

        }

        #endregion

        #region Group Figures
        private string Group(string Argument)
        {
            string Hyphen = "";
            string OutPut = "";
            Int16 d1 = Convert.ToInt16(Argument.Substring(0, 1));
            Int16 d2 = Convert.ToInt16(Argument.Substring(1, 1));
            Int16 d3 = Convert.ToInt16(Argument.Substring(2, 1));
            if ((d1 >= 1))
            {
                if (d2 == 0 && d3 == 0)
                {
                    OutPut += SNu[d1] + " hundred ";
                }
                else
                {
                    OutPut += SNu[d1] + " hundred and ";
                }
            }
            if ((double.Parse(Argument.Substring(1, 2)) < 20))
            {
                OutPut += SNu[Convert.ToInt16(Argument.Substring(1, 2))];
            }
            if ((double.Parse(Argument.Substring(1, 2)) >= 20))
            {
                if (Convert.ToInt16(Argument.Substring(2, 1)) == 0)
                {
                    Hyphen += " ";
                }
                else
                {
                    Hyphen += " ";
                }
                OutPut += SNt[d2] + Hyphen + SNu[d3];
            }
            return OutPut;
        }


        #endregion

        #region Initialize
        private void Initialize()
        {

            SNu[0] = "";
            SNu[1] = "One";
            SNu[2] = "Two";
            SNu[3] = "Three";
            SNu[4] = "Four";
            SNu[5] = "Five";
            SNu[6] = "Six";
            SNu[7] = "Seven";
            SNu[8] = "Eight";
            SNu[9] = "Nine";
            SNu[10] = "Ten";
            SNu[11] = "Eleven";
            SNu[12] = "Twelve";
            SNu[13] = "Thirteen";
            SNu[14] = "Fourteen";
            SNu[15] = "Fifteen";
            SNu[16] = "Sixteen";
            SNu[17] = "Seventeen";
            SNu[18] = "Eighteen";
            SNu[19] = "Nineteen";
            SNt[2] = "Twenty";
            SNt[3] = "Thirty";
            SNt[4] = "Forty";
            SNt[5] = "Fifty";
            SNt[6] = "Sixty";
            SNt[7] = "Seventy";
            SNt[8] = "Eighty";
            SNt[9] = "Ninety";
            US[1] = "";
            US[2] = "Thousand";
            US[3] = "Million";
            US[4] = "Billion";
            US[5] = "Trillion";
            US[6] = "Quadrillion";
            US[7] = "Quintillion";
            US[8] = "Sextillion";
            US[9] = "Septillion";
            US[10] = "Octillion";
        }

        #endregion
    }
}
