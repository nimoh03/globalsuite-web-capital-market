namespace GlobalSuite.Core.Accounting.Models
{
    public class ProductAccountResponse
    {
        private string _custAID;
        public string CustAID
        {
            get => _custAID.Trim();
            set => _custAID = value;
        } 
        public long TransNo { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string FullName => $"{Surname} {Firstname} {Othername} - {CustAID.Trim()} - {CscsAccount}".Trim();
        public string Othername { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CscsAccount { get; set; }
        public string CscsReg { get; set; }
        public int Comm { get; set; }
        public int SellCommission { get; set; }

        
    }
}