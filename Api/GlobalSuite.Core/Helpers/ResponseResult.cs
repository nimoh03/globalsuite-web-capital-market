using System.Collections.Generic;
using System.Text;

namespace GlobalSuite.Core.Helpers
{
    public class ResponseResult
    {
        public ResponseResult()
        {
        }

        public ResponseResult(string message, bool isSuccess = false)
        {
            Messages.Add(message);
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public object Data { get; set; }

        public static ResponseResult Success() => new ResponseResult("Data saved successfully",true);
        public static ResponseResult Success(string message) => new ResponseResult(message,true);
        public static ResponseResult Success(object data) => new ResponseResult { IsSuccess = true, Data = data};
        public static ResponseResult Error() => new ResponseResult("Unknown Error has occured.");
         
        public static ResponseResult Error(string message) => new ResponseResult(message);
        public static ResponseResult Error(string message, object data) => new ResponseResult(message){Data = data};
        public static ResponseResult Error(object data) => new ResponseResult{Data = data};

        public override string ToString()
        {
            var sb = new StringBuilder();
            Messages.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }
    }
}
