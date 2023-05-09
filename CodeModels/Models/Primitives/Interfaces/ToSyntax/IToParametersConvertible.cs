﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToParametersConvertible
{
    ParameterListSyntax ToParameters();
}
