﻿using System;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Core.Accounting.Models
{
    public class DebitNoteResponse : StatusResponse
    {
        public string DebitNo { get; set; }
        public string AcctMasBank { get; set; }
        public string AcctSubBank { get; set; }
        public DateTime RNDate { get; set; }
        public string TranDesc { get; set; }
        public decimal Amount { get; set; }
        public string Ref { get; set; }
        public string Custno { get; set; }
        public string Amtword { get; set; }
        public bool ChargeVAT { get; set; }
        public bool VATIsInclusive { get; set; }
        public PublicCustomer Customer { get; set; }
        public ProductDetailResponse ProductDetail { get; set; }
        public BranchResponse Branch { get; set; }
        public ChartOfAccountResponse SubBank { get; set; }
    }
}