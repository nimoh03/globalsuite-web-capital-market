#pragma warning disable CS8618
using Newtonsoft.Json;

namespace GlobalSuite.Core.Exceptions
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; }
        public string Reason { get; }

        public ValidationError(string name, string reason)
        {
            Name = name != string.Empty ? name : null;
            Reason = reason;
        }
    }
}