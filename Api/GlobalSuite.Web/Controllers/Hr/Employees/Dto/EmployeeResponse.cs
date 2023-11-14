namespace GlobalSuite.Web.Controllers.Employees.Dto
{
    public class EmployeeResponse:EmployeeRequest
    {
        public string TransNo { get; set; }
        public bool DoNotChargeNHF { get; set; }
        public bool DoNotChargeNSITF { get; set; }
    }
}