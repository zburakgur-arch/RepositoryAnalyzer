using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.Exceptions
{
    public class InvalidUrlException : ADomainException
    {
        public InvalidUrlException() : base("The provided URL is invalid.", HttpStatusCode.BadRequest)
        {
        }
    }
}
