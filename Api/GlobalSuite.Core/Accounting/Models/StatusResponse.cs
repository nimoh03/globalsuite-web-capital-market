namespace GlobalSuite.Core.Accounting.Models
{
    public  class StatusResponse
    {
        public string Source { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
        public string Status
        {
            get
            {
                if (Reversed)
                    return "Reversed";  
                switch (Posted)
                {
                    case false when !Reversed:
                        return "UnPosted";
                    case true when !Reversed:
                        return "Posted";
                    default:
                        return "UnPosted";
                }
            }
        }
    }
}