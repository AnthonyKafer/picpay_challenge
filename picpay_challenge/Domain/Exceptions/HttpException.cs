using System.Net;

namespace picpay_challenge.Domain.Exceptions
{
    public class HttpException : Exception
    {
        public HttpStatusCode _status;

        public HttpException(HttpStatusCode status, string message) : base(message)
        {
            _status = status;
        }
    }
}
