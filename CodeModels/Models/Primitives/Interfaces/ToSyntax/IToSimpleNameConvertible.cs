﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToSimpleNameConvertible
{
    SimpleNameSyntax NameSyntax();
}
