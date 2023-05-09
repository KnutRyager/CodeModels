using System;

namespace CodeModels.Execution;

public class CodeModelExecutionException : Exception
{
    public CodeModelExecutionException(string message) : base(message) { }
}
