namespace GlobalSuite.Core.Accounting.Models
{
    public class IfrsAnnualResponse
    {
        public long TransNo { get; set; }
        public string ItemName { get; set; }
        public int ItemLevel { get; set; }
        public string IsParent { get; set; }
        public string CreditDebitBalance { get; set; }
        public int ReportPosition { get; set; }
        public int ReportPosition2 { get; set; }
        public int ReportPosition3 { get; set; }
        public int ReportPosition4 { get; set; }
        public int ReportPosition5 { get; set; }
    }
}