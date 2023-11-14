using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GlobalSuite.Core.Customers.Models;

namespace GlobalSuite.Web.Controllers.Customers.Dto
{
    public class CustomerRequest:PublicCustomer
    {
        
        public string Contact { get; set; } = string.Empty;
        public DateTime ContactDate { get; set; }  
        public DateTime BirthDate { get; set; }  
        public string AcctOfficer { get; set; } = string.Empty;
        public string POBox { get; set; } = string.Empty;
        public int ClientType { get; set; }  
        public int Nationality { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int Country { get; set; }  
        public string Fax { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string RCNo { get; set; } = string.Empty;
        public long LGA { get; set; }  
        public string MotherMaidenName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string InvestmentObjective { get; set; } = string.Empty;
        public string BVNNumber { get; set; } = string.Empty;
        public int NumberOfDirector { get; set; }
        public bool DeactivateSMSAlert { get; set; } = true;
        public bool DeactivateEmailAlert { get; set; }
        public bool ForeignCustomer { get; set; } 
        public int SOrigin { get; set; } 
         public List<CustomerBankRequest> Banks { get; set; } = new List<CustomerBankRequest>();
        public NextOfKinRequest NextOfKin { get; set; }
        public CustomerEmployerRequest Employer { get; set; }
        public CustomerExtraInformationRequest ExtraInformation { get; set; }
    }
}