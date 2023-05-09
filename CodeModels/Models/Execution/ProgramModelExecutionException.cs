using System;

namespace CodeAnalyzation.Models;

public class ProgramModelExecutionException : Exception
{
    public ProgramModelExecutionException(string message) : base(message) { }
}
