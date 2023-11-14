using System.ComponentModel;

namespace BaseUtility.Business
{
    public class DataGeneral
    {
        #region Enum
        public enum SaveStatus
        {
            Nothing, NotExist, Saved, NotSaved, NameExistAdd, NameExistEdit,
            AccountIdExistAdd, AccountIdExistEdit, IsCustomerAccount, DuplicateRef,
            HeadOfficeExistAdd, HeadOfficeExistEdit, DateNotEqual, FutureDate,
            ProductRecordNotExist, ProductNotExist, QuantityNotEnough
        }
        public enum SaveStatusBranch { Nothing, NotExist, Saved, NameExistAdd, NameExistEdit, HeadOfficeExistAdd, HeadOfficeExistEdit }

        public enum DeleteAccountStatus
        {
            Nothing, NotExist, Deleted, GLExist
        }

        public enum TransFormType
        {
            AllExtra, All, AllExceptCode, Normal, Post, Reverse, PostOnly, ReverseOnly, SaveOnly, ViewOnly
        }
        #endregion

        #region Enum
        public enum PostStatus
        {
            Nothing, Posted, UnPosted, Reversed, All, PostedAsc, UnPostedAsc
        }
        public enum AllotmentType
        {
            Buy, Sell, Cross, CrossSell, All

        }
        public enum GLInstrumentType
        {
            //C -- Cash Q -- Clearing Cheque H -- Cash Cheque N -- NIBSS R -- TRANSFER
            //U-- Cheque S--Internal Customer Transfer O-- Others V-- Dividend M- Commission
            [Description("Cash")]
            C,
            [Description("Clearing Cheque")]
            Q, 
            [Description("Cash Cheque")]
            H,
            [Description("NIBSS")]
            N, 
            [Description("TRANSFER")]
            R,
            [Description("Cheque")]
            U,
            [Description("Internal Customer Transfer")]
            S,
            [Description("Others")]
            O,
            [Description("Dividend")]
            V,
            [Description("Commission")]
            M
            
        }
        public enum InterestType
        {
            //B--Backend U-- Upfront 
            B, U
        }
        public enum InterestPeriod
        {
            //Y-- Yearly M--Flat/Monthly 
            Y, M
        }
        
        public enum StockInstrumentType
        {
            QUOTEDEQUITY, NASD, BOND, OTHERS
        }

        public enum CustomerTypeCode
        {
            KYCINDIVIDUAL, KYCCORP, KYCESTATE
        }
        #endregion
    }
}
