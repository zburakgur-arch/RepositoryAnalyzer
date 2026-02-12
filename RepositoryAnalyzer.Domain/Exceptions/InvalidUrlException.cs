using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.Exceptions
{
    public class InvalidUrlException : Exception
    {
        readonly HttpStatusCode _statusCode = HttpStatusCode.BadRequest;

        public InvalidUrlException() : base("The provided URL is invalid.")
        {
        }

        public HttpStatusCode GetHttpStatusCode()
        {
            return _statusCode;
        }
    }
}
