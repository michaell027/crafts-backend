using crafts_api.interfaces;
using Newtonsoft.Json;
using System.Net;

namespace crafts_api.exceptions
{
    public class DefaultException : Exception
    {
        public HttpStatusCode? StatusCode { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }

        public EventId EventId => new EventId(100, "CustomError");

        public string ToJson()
        {
            return JsonConvert.SerializeObject(new
            {
                StatusCode,
                ErrorCode,
                Message
            });
        }
    }
}
