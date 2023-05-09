using System;

namespace CodeModels.Execution;

public class ProgramModelExecutionException : Exception
{
    public ProgramModelExecutionException(string message) : base(message) { }
}
