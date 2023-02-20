﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IPattern : IExpressionOrPattern
{
    new PatternSyntax Syntax();
}

public interface IPattern<T> : IPattern, IExpressionOrPattern<T>
    where T : PatternSyntax
{
    new T Syntax();
}