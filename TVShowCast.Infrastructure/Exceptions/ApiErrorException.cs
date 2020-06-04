using System;
using System.Net;

namespace TVShowCast.Infrastructure.Exceptions
{
    public class ApiErrorException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ApiErrorException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
