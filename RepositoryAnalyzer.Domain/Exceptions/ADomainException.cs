using System;
using System.Net;

namespace RepositoryAnalyzer.Domain.Exceptions;

public abstract class ADomainException : Exception
{
    protected readonly string _message;
    protected readonly HttpStatusCode _statusCode;

    public ADomainException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
    {
        _message = message;
        _statusCode = statusCode;
    }

    public HttpStatusCode GetHttpStatusCode()
    {
        return _statusCode;
    }
}
