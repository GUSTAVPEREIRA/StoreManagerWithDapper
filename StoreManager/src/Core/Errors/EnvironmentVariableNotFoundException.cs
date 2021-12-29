using System;

namespace Core.Errors;

public class EnvironmentVariableNotFoundException : Exception
{
    public EnvironmentVariableNotFoundException(string variable) : base($"environment variable {variable} has been not found")
    {
    }
}