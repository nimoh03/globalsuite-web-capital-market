using BaseUtility.Business;

namespace GlobalSuite.Core.Accounting.Models
{
    public class StatusFilter:BaseFilter
    {
        public StatusFilter()
        {
            Status = DataGeneral.PostStatus.UnPosted.ToString();
        }
        public string Status { get; set; }
    }
}