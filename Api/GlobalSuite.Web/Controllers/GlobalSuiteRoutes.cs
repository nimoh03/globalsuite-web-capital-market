namespace GlobalSuite.Web.Controllers
{
    public static class GlobalSuiteRoutes
    {
        internal static class CustomerRoutes
        {
            internal const string Customers = "api/customers";
            internal const string CustomerKyc = "api/customers/kyc";
            internal const string CustomerTypes= "api/customers/types";
        } 
        internal static class HrRoutes
        {
            internal const string Employees = "api/hr/employees";
        }

        internal static class AdministratorRoutes
        {
            internal const string Users = "api/administrator/users";
            internal static class MaintainRoutes
            {
                internal const string Banks = "api/administrator/maintain/banks";
                internal const string Branches = "api/administrator/maintain/branches";
                internal const string Countries = "api/administrator/maintain/countries";
                internal const string States = "api/administrator/maintain/state";
                internal const string Lgas = "api/administrator/maintain/lgas";
                internal const string GeoPoliticalZones = "api/administrator/maintain/zones";
                internal const string Occupations = "api/administrator/maintain/occupations";
                internal const string Religions = "api/administrator/maintain/religions";
                internal const string Titles = "api/administrator/maintain/titles";
                internal const string PostingStatus = "api/administrator/maintain/posting-status";
                internal const string AllotmentTypes = "api/administrator/maintain/allotment-types";
                internal const string GlInstrumentTypes = "api/administrator/maintain/gl-instrument-types";
                internal const string InterestTypes = "api/administrator/maintain/interest-types";
                internal const string InterestPeriods = "api/administrator/maintain/interest-periods";
                internal const string StockInstrumentTypes = "api/administrator/maintain/stock-instrument-types";
                internal const string CustomerTypeCodes = "api/administrator/maintain/customer-codes";
            } internal static class CompanyRoutes
            {
                internal const string Companies = "api/administrator/companies";
                internal const string RunEndOfDay = "api/administrator/run/eod";
                internal const string RunEndOfMonth= "api/administrator/run/eom";
                internal const string RunStartOfMonth = "api/administrator/run/som";
                internal const string RunEndOfYear = "api/administrator/run/eoy";
                internal const string RunStartOfYear = "api/administrator/run/soy";
                internal const string OpenClosedPeriod = "api/administrator/run/ocp";
                internal const string CloseClosedPeriod = "api/administrator/run/ccp";
                internal const string OpenClosedPeriodYear = "api/administrator/run/ocp-year";
                internal const string CloseClosedPeriodYear = "api/administrator/run/ccp-year";
            } 
            
        }

        internal static class AccountingRoutes
        {
            internal const string Deposits = "api/accounting/deposits";
            internal const string Payments = "api/accounting/payments";
            internal const string CreditNote = "api/accounting/credit-notes";
            internal const string DebitNote = "api/accounting/debit-notes";
            internal const string Transfer = "api/accounting/transfers";
            internal const string OpeningBalance = "api/accounting/opening-balances";
            internal const string Products = "api/accounting/products";
            internal const string ProductAccounts = "api/accounting/accounts/subsidiary";
            internal const string CustomerBalance = "api/accounting/customer/balance";

            internal static class JournalPostingRoutes
            {
                internal const string BatchPosting = "api/accounting/journal/batch";
                internal const string SelfBalancing = "api/accounting/journal/self";
            } 
            internal static class MaintainRoutes
            {
                internal const string ChartOfAccount = "api/accounting/maintain/chart-of-accounts";
                internal const string GlParamSetting = "api/accounting/maintain/params";
            }

            internal static class ReportRoutes
            {
                    internal const string AccountStatement = "api/accounting/reports/account-statement";
                    internal const string CreditNote = "api/accounting/reports/credit-note";
                    internal const string DebitNote = "api/accounting/reports/debit-note";
                    internal const string Deposit = "api/accounting/reports/deposit";
                    internal const string ChartOfAccount = "api/accounting/reports/chart-of-account";
            }
        }

        internal static class CapitalMarketRoutes
        {
            internal static class JobbingRoutes
            {
                internal const string JobOrders = "api/capital-market/jobbing/job-orders";
            }

            internal static class TradingRoutes
            {
                internal const string Buy = "api/capital-market/allotment/buy";
                internal const string Sell = "api/capital-market/allotment/sell";
                internal const string CrossDeals = "api/capital-market/allotment/cross-deal";
                
                internal const string DailyPosting = "api/capital-market/daily-posting";
                internal const string DailyPostingPullout = "api/capital-market/daily-posting/reverse";
                internal const string FixEndOfDay = "api/capital-market/fix-end-of-day";
                internal const string FixEndOfDayPullout = "api/capital-market/fix-end-of-day/reverse";
                internal const string NasdPosting = "api/capital-market/nasd";
                internal const string NasdPostingPullout = "api/capital-market/nasd/reverse";
                internal const string TradeValuation = "api/capital-market/trade-valuation";
            }

            internal static class PortfolioRoutes
            {
                internal const string AddHoldingBalance = "api/capital-market/portfolio/holding-balance";
                internal const string PostHoldingBalance = "api/capital-market/portfolio/holding-balance/post";
                internal const string AddHoldingBalanceInternal = "api/capital-market/portfolio/holding-balance/internal";
                internal const string DeductHoldingBalanceInternal = "api/capital-market/portfolio/holding-balance/internal/deduct";
                internal const string PostHoldingBalanceInternal = "api/capital-market/portfolio/holding-balance/internal/post";
                internal const string CsCsStockPosition = "api/capital-market/portfolio/cscs/position";
                internal const string SendPortfolioToEmail = "api/capital-market/portfolio/cscs/send";
            }

            internal static class MaintainRoutes
            {
                internal const string CustomerAccount = "api/capital-market/maintain/customer";
                internal const string CapitalMarketParams = "api/capital-market/maintain/param";
                internal const string MergeCustomer = "api/capital-market/maintain/customer/merge";
                internal const string Stocks = "api/capital-market/maintain/stock";
                internal const string Brokers = "api/capital-market/maintain/brokers";
            }
        }
        
       
    }
}