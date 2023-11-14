export const ROUTES = {
  dashboard: () => ({
    title: "Dashboard",
    path: "/user/dashboard",
  }),
  login: () => ({
    title: "Login",
    path: "/login",
  }),
  accountingCustomerDeposit: () => ({
    title: "Accounting",
    path: "/user/accounting/customer-deposit",
    label: "Deposits",
    children: [
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit",
        label: "Deposits",
      },
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit/payments",
        label: "Payments",
      },
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit/credit-note",
        label: "Credit Note",
      },
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit/debit-note",
        label: "Debit Note",
      },
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit/customer-opening-balance",
        label: "Customer Opening Balance",
      },
      {
        title: "Accounting",
        path: "/user/accounting/customer-deposit/customer-transfer",
        label: "Customer Transfer",
      },
    ],
  }),
  accountingCustomerDepositPaymentCreate: () => ({
    title: "Accounting",
    path: "/user/accounting/customer-deposit/payments/add-payment",
    label: "Add Payment",
  }),

  accountingJournalPosting: () => ({
    title: "Accounting",
    path: "/user/accounting/journal-posting/batch-posting",
    label: "Batch Posting",
    children: [
      {
        title: "Accounting",
        path: "/user/accounting/journal-posting/batch-posting",
        label: "Batch Posting",
      },
      {
        title: "Accounting",
        path: "/user/accounting/journal-posting/self-balancing",
        label: "Self Balancing",
      },
    ],
  }),
  accountingMaintain: () => ({
    title: "Accounting",
    path: "/user/accounting/maintain/chart-of-accounts",
    children: [
      {
        title: "Accounting",
        path: "/user/accounting/maintain/chart-of-accounts",
        label: "Chart of Accounts",
      },
      {
        title: "Accounting",
        path: "/user/accounting/maintain/gl-parameter-settings",
        label: "GL Parameter Settings",
      },
    ],
  }),
  accountingReport: () => ({
    title: "Accounting",
    path: "/user/accounting/report/chart-of-accounts",
    children: [
      {
        title: "Accounting",
        path: "/user/accounting/report/chart-of-accounts",
        label: "Account Setup-Chart of Accounts",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/customer-statement",
        label: "Customer Statement",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/gl-account-statement",
        label: "GL Account Statement",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/trial-balance",
        label: "Management Report-Trial Balance",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/customer-product-balance",
        label: "Management Report-Customer Product Balance",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/customer-account-turnover",
        label: "Management Report-Customer Account Turnover",
      },
      {
        title: "Accounting",
        path: "/user/accounting/report/profit-loss",
        label: "Management Report-Profit/Loss (Normal)",
      },
    ],
  }),
  capitalMarketJobbing: () => ({
    title: "Capital Market",
    path: "/user/capital-market/jobbing/job-order",
    children: [
      {
        title: "Capital Market",
        path: "/user/capital-market/jobbing/job-order",
        label: "Job Order",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/jobbing/job-mandate-document",
        label: "Job Mandate Document",
      },
    ],
  }),
  capitalMarketStockMarketTrading: () => ({
    title: "Capital Market",
    path: "/user/capital-market/stock-market-trading/allotment-reversal",
    children: [
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/allotment-reversal",
        label: "Allotment Reversal",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/ats-allotment-deals",
        label: "ATS Allotment Deals",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/daily-trade-automation-posting",
        label: "Daily Trade Automation Posting",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/daily-trade-pullout",
        label: "Daily Trade Pullout",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/end-of-day-fix",
        label: "End of Day Fix",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/fix-trade-pullout",
        label: "Fix Trade Pullout",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/nasd-trade-posting",
        label: "NASD Trade Posting",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/nasd-trade-pullout",
        label: "NASD Trade Pullout",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/stock-market-trading/trade-evaluation",
        label: "Trade Evaluation",
      },
    ],
  }),
  capitalMarketPortfolioManagement: () => ({
    title: "Capital Market",
    path: "/user/capital-market/portfolio-management/cscs-stock-position",
    children: [
      {
        title: "Capital Market",
        path: "/user/capital-market/portfolio-management/cscs-stock-position",
        label: "CSCS Stock Position",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/portfolio-management/customer-stock-position",
        label: "Customer Stock Position",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/portfolio-management/porfolio-holding-balance",
        label: "Porfolio Holding Balance",
      },
    ],
  }),
  capitalMarketMaintain: () => ({
    title: "Capital Market",
    path: "/user/capital-market/maintain/capital-market-parameters",
    children: [
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/capital-market-parameters",
        label: "Capital Market Parameters",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/create-customer-account",
        label: "Create Customer Account",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/market-exchange",
        label: "Market Exchange",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/merge-customer-account",
        label: "Merge Customer Account",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/securities-assets",
        label: "Securities Assets",
      },
      {
        title: "Capital Market",
        path: "/user/capital-market/maintain/stock-brokers",
        label: "Stock Brokers",
      },
    ],
  }),
  capitalMarketReport: () => ({
    title: "Capital Market",
    path: "/user/capital-market/report",
    children: [],
  }),
  administratorUserMaintenance: () => ({
    title: "Administrator",
    path: "/user/administrator/user-maintenance/users",
    children: [
      {
        title: "Administrator",
        path: "/user/administrator/user-maintenance/users",
        label: "Users",
      },
      {
        title: "Administrator",
        path: "/user/administrator/user-maintenance/user-role-profile",
      },
      {
        title: "Administrator",
        path: "/user/administrator/user-maintenance/user-level",
        label: "User Level",
      },
      {
        title: "Administrator",
        path: "/user/administrator/user-maintenance/level-role-profile",
        label: "Level Role Profile",
      },
    ],
  }),
  administratorMaintain: () => ({
    title: "Administrator",
    path: "/user/administrator/maintain/bank",
    children: [
      {
        title: "Administrator",
        path: "/user/administrator/maintain/bank",
        label: "Bank",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/branch",
        label: "Branch",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/country",
        label: "Country",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/geo-zone",
        label: "Geo Zone",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/lga",
        label: "LGA",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/occupation",
        label: "Occupation",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/religion",
        label: "Religion",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/state",
        label: "State",
      },
      {
        title: "Administrator",
        path: "/user/administrator/maintain/title",
        label: "Title",
      },
    ],
  }),
  administratorCompanySetup: () => ({
    title: "Administrator",
    path: "/user/administrator/company-setup",
  }),
  administratorEndOfPeriodProcessing: () => ({
    title: "Administrator",
    path: "/user/administrator/end-period-processing/run-start-month",
    children: [
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/run-start-month",
        label: "Run Start of Month",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/run-end-month",
        label: "Run End of Month",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/run-start-year",
        label: "Run Start of Year",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/run-end-year",
        label: "Run End of Year",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/close-closed-period",
        label: "Close Closed of Period",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/open-closed-year",
        label: "Open Closed of Year",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/close-closed-period-in-current-year-period",
        label: "Close Closed of Period in Current Year Period",
      },
      {
        title: "Administrator",
        path: "/user/administrator/end-period-processing/open-closed-period-in-current-year-period",
        label: "Open Closed of Period in Current Year Period",
      },
    ],
  }),
  administratorReport: () => ({
    title: "Administrator",
    path: "/user/administrator/report",
    children: [],
  }),
  customerManagementCustomerSetup: () => ({
    title: "Customer Management",
    path: "/user/customer-management/customer-setup",
  }),
  customerManagementMaintain: () => ({
    title: "Customer Management",
    path: "/user/customer-management/maintain/country",
    children: [
      {
        title: "Customer Management",
        path: "/user/customer-management/maintain/country",
        label: "Country",
      },
      {
        title: "Customer Management",
        path: "/user/customer-management/maintain/customer-type",
        label: "Customer Type",
      },
      {
        title: "Customer Management",
        path: "/user/customer-management/maintain/kyc-document-type",
        label: "KYC Document Type",
      },
      {
        title: "Customer Management",
        path: "/user/customer-management/maintain/compulsory-customer-kyc",
        label: "Compulsory Customer KYC",
      },
    ],
  }),
  customerManagementReport: () => ({
    title: "Customer Management",
    path: "/user/customer-management/report",
  }),
  customerManagementEnquiry: () => ({
    title: "Customer Management",
    path: "/user/customer-management/enquiry/customer-data-page",
    children: [
      {
        title: "Customer Management",
        path: "/user/customer-management/enquiry/customer-data-page",
        label: "Customer Data Page",
      },
    ],
  }),
  humanResourcesEmployeeManagement: () => ({
    title: "Human Resources",
    path: "/user/human-resources/employee-management",
  }),
};

//get route title from path segment
// e.g. /user/accounting/customer-deposit => Customer Deposit
export const getRouteTitle = (route: string) => {
  const words = route.split("/");
  const lastWord = words[words.length - 1];
  const transformedLastWord = lastWord
    .split("-")
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(" ");
  return transformedLastWord;
};
