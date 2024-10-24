using System.Net;

namespace Learnings.Application.ResponseBase
{
    public class ResponseBase<T>
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }

        public ResponseBase(T data, string message, HttpStatusCode status)
        {
            Data = data;
            Message = message;
            Status = status;
            Errors = new List<string>();
        }

        //public ResponseBase(List<string> errors, string message, HttpStatusCode status)
        //{
        //    Errors = errors;
        //    Message = message;
        //    Status = status;
        //    Data = default;
        //}
    }
}
