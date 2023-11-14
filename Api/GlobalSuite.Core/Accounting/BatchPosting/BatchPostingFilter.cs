using System;
using GlobalSuite.Core.Accounting.Models;

namespace GlobalSuite.Core.Accounting.BatchPosting
{
    public class BatchPostingFilter:StatusFilter
    {
        public string TxnFrom { get; set; } = null;
        public string TxnTo { get; set; } = null;
    }
}