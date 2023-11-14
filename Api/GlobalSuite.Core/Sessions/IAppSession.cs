namespace GlobalSuite.Core.Sessions
{
    public interface IAppSession
    {
          string UserName { get; }
          int CompanyNumber { get; }
          string UserBranchNumber { get; }
          string DefaultBranch { get; }
        string ReportName { get; }
        string FormModuleName { get; }
    }
}
