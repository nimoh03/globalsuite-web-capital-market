using System;
using System.Collections.Generic;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;

namespace GlobalSuite.Core.Accounting.BatchPosting
{
    public class BatchPostingDetail : BatchPostingResponse
    {
        public List<BatchTransactionResponse> Transactions { get; set; } = new List<BatchTransactionResponse>();
    }
    public class BatchPostingResponse:StatusResponse
    {
        public long BatchSpreadSheetMasterId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string InputBy { get; set; }
        public string ApprovedBy { get; set; }
        public string ReversedBy { get; set; }
        // public string Batchno { get; set; }
        // public string TransNo { get; set; }
        // public string AccountID { get; set; }
        // public string Description { get; set; }
        // public DateTime VDate { get; set; }
        // public DateTime TxnDate { get; set; }
        // public decimal Debit { get; set; }
        // public decimal Credit { get; set; }
        // public string TransType { get; set; }
        // public string Approved { get; set; }
        // public string Approvedby { get; set; }
        // public string UserID { get; set; }
        // public string Branch { get; set; }
        // public string ChequeNo { get; set; }
        // public string InstrumentType { get; set; }
        // public ProductResponse Account { get; set; }
        // public BranchResponse BranchDetail { get; set; }
    }
    
    

    public class BatchTransactionResponse
    {
        public string AccountId { get; set; }
        public ChartOfAccountResponse Account { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}