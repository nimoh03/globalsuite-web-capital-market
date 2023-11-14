using AutoMapper;
using BaseUtility.Business.Extensions;
using GL.Business;
using GlobalSuite.Core.Accounting;
using GlobalSuite.Core.Accounting.BatchPosting;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Web.Controllers.Accounting.Dto
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<DepositRequest, Deposit>().ForMember(x => x.TransDesc,
                y => y.MapFrom(z => z.Trandesc));
            CreateMap<DepositUpdateRequest, Deposit>().ForMember(x => x.TransDesc,
                y => y.MapFrom(z => z.Trandesc));
            CreateMap<Deposit, DepositResponse>();
            CreateMap<Deposit, DepositDetailResponse>();
            CreateMap<DepositResponse, DepositDetailResponse>();
            CreateMap<PaymentRequest, Payment>().ForMember(x=>x.ChqueNo, 
                y=>y.MapFrom(x=>x.ChequeNo));
            CreateMap<PaymentUpdateRequest, Payment>().ForMember(x=>x.ChqueNo, 
                y=>y.MapFrom(x=>x.ChequeNo));
            CreateMap<Payment, PaymentResponse>().ForMember(x=>x.PaymentNo, 
                y=>y.MapFrom(x=>x.Code));
            CreateMap<ProductClass, ProductClassResponse>();
            CreateMap<ProductType, ProductTypeResponse>();
            CreateMap<CustomerTransferRequest, CustomerTransfer>();
            CreateMap<CreditNoteRequest, CNote>().ForMember(x=>x.CreditNo,
                y=>y.MapFrom(z=>z.Code));
            CreateMap<CNote, CreditNoteResponse>();
            CreateMap<DNote, DebitNoteResponse>();
            CreateMap<DebitNoteRequest, DNote>().ForMember(x=>x.DebitNo,
                y=>y.MapFrom(z=>z.Code));
            CreateMap<OpeningBalanceRequest, CustOBal>().ForMember(x=>x.TransNo,
                y=>y.MapFrom(z=>z.Code));
            CreateMap<Product, ProductDetailResponse>()
                .ForMember(x=>x.ProductCode, 
                    y=>y.MapFrom(z=>z.TransNo))
                .ForMember(x=>x.ModuleName, 
                y=>y.MapFrom(z=>z.ModName));
            CreateMap<Product, ProductResponse>().ForMember(x=>x.ProductCode, 
                y=>y.MapFrom(z=>z.TransNo))
                .ForMember(x=>x.ProductName, 
                y=>y.MapFrom(z=>z.Description));

            CreateMap<AccountRequest, Account>();
            CreateMap<Account, AccountResponse>();
            CreateMap<Account, ChartOfAccountResponse>();
            CreateMap<CustomerTransfer, TransferResponse>();
            CreateMap<CustOBal, OpeningBalanceResponse>();
            CreateMap<SelfBalanceRequest, SelfBal>();
            CreateMap<SelfBal, SelfBalanceResponse>().ForMember(x=>x.Posted, 
                y=>y.MapFrom(z=>z.Posted=="Y"))
                .ForMember(x=>x.Reversed, 
                y=>y.MapFrom(z=>z.Reversed=="Y"));
            CreateMap<BatchPostingRequest, Batch>();
            CreateMap<BatchPostingRequest, BatchSpreadSheet>();
            CreateMap<BatchSpreadSheetMaster, BatchPostingResponse>();
            CreateMap<BatchSpreadSheetMaster, BatchPostingDetail>();
            CreateMap<BatchSpreadSheet, BatchTransactionResponse>();
            CreateMap<GLParam, GlParamResponse>();
            CreateMap<GlParamRequest, GLParam>();
        }
        
        
    }
    
    }