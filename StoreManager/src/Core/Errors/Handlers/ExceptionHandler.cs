using System;
using Core.Errors.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Errors.Handlers;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public Error HandleException(Exception exception)
    {
        var error = exception switch
        {
            _ => HandleUnexpectedExceptions(exception)
        };

        return error;
    }

    private Error HandleUnexpectedExceptions(Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        return new Error
        {
            Message = exception.Message,
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}