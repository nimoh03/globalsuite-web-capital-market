namespace GlobalSuite.Core
{
    public class Constants
    {
        public class GClaimTypes
        {
            public const string CompanyName = "companyname";
            public const string BranchNumber = "branchnumber";
        }
        public class Caching
        {
            public const string Cache1Hour = nameof(Cache1Hour);
            public const string Cache1Day = nameof(Cache1Day);
        }
        public class SaveType
        {
            public const string ADDS = nameof(ADDS);
            public const string EDIT = nameof(EDIT);
            public const string DELETE= nameof(DELETE);
        }
        public class RunType
        {
            public const string E = nameof(E);
            public const string S = nameof(S);
        }

        public class ParamTable
        {
            public const string POSTCOMMSHARED=nameof(POSTCOMMSHARED);
            public const string CALCPROPGAINORLOSS=nameof(CALCPROPGAINORLOSS);
            public const string BRANCHBUYSELLCHARGESDIFFERENT=nameof(BRANCHBUYSELLCHARGESDIFFERENT);
            public const string POSTINVPLCTRLSECACCT=nameof(POSTINVPLCTRLSECACCT);
            public const string COMMISSIONSEPERATED=nameof(COMMISSIONSEPERATED);
            public const string TITLE = nameof(TITLE);
            public const string BANK = nameof(BANK);
            public const string RELIGION = nameof(RELIGION);
            public const string STATE = nameof(STATE);
            public const string OCCUPATIONLEVEL = nameof(OCCUPATIONLEVEL);
            public const string GEOPOLITICAL = nameof(GEOPOLITICAL);
            public const string TRADEBOOKADDTOGLOBALSUITE = nameof(TRADEBOOKADDTOGLOBALSUITE);
            
        }

        public class TableName
        {
            public const string DEPOSIT=nameof(DEPOSIT);
            public const string PAYMENT=nameof(PAYMENT);
            public const string DBNOTE=nameof(DBNOTE);
            public const string CRNOTE=nameof(CRNOTE);
        }
    }
}
