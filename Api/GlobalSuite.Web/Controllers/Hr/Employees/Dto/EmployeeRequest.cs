using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HR.Business;

namespace GlobalSuite.Web.Controllers.Employees.Dto
{
    public class EmployeeRequest
    {
        public DateTime DOB { get; set; }
        [Required]    public string Surname { get; set; }
        [Required]   public string FirstName { get; set; }
        public string OtherName { get; set; } =string.Empty;
        public string Addr1 { get; set; } =string.Empty;
        public string Addr2 { get; set; } =string.Empty;
        public int State { get; set; } 
        public string Religion { get; set; } =string.Empty;
        public string Country { get; set; } =string.Empty;
        public string Qualification { get; set; } =string.Empty;
        public string Telephone { get; set; } =string.Empty;
        public string Deficiency { get; set; } =string.Empty;
        public DateTime LicExpDate { get; set; }
        public DateTime ResumeDate { get; set; }  
        public DateTime RetireDate { get; set; }  
        public int Occupation { get; set; }  
        public string OccupLevel { get; set; } =string.Empty;
        public int PensionMngr { get; set; }  
        public string PensionNo { get; set; } =string.Empty;
        public string BankName { get; set; } =string.Empty;
        public string BankAC { get; set; } =string.Empty;
        public string HMOType { get; set; } =string.Empty;
        public string Branch { get; set; } =string.Empty;
        public string SalaryRateType { get; set; } =string.Empty;
        public decimal BasicSalary { get; set; }  
        public bool DoNotGenerateSalary { get; set; }
        public List<SalaryStruct> SalStruct { get; set; } = new List<SalaryStruct>();
    }
}