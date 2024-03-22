using System.Net;

namespace crafts_api.interfaces
{
    public interface IEventSchedulerException
    {
        public EventId EventId { get; }
        public HttpStatusCode? StatusCode { get; set; }

        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public string ToJson();
    }
}
