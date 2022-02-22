using System;

namespace Core.Errors.Interfaces;

public interface IExceptionHandler
{
    public Error HandleException(Exception exception);
}