namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class DepositRequest:CreditNoteRequest
    {
        public int InstrumentType { get; set; }
    }
}