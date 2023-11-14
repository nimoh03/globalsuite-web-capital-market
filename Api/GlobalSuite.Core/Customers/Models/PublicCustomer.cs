namespace GlobalSuite.Core.Customers.Models
{
    public class PublicCustomer
    {
        public string CustAID { get; set; }
        public string Title { get; set; } = string.Empty;
         public string FirstName { get; set; } = string.Empty;
         public string Surname { get; set; } = string.Empty;
        public string Othername { get; set; } = string.Empty;
        public string FullName => $"{Surname}, {FirstName} {Othername}".Trim();
        public string MobPhone { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;


    }
}