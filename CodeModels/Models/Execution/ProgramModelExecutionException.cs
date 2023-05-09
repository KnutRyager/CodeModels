using System;

namespace CodeModels.Models;

public class ProgramModelExecutionException : Exception
{
    public ProgramModelExecutionException(string message) : base(message) { }
}
