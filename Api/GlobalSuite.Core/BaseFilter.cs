using System;

namespace GlobalSuite.Core
{
    public class BaseFilter
    {
        public virtual int PageNo { get; set; } = 1;
        public virtual int PageSize { get; set; } = 50;
        public int Skip => (PageNo - 1) * PageSize;
        public string Keyword { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}