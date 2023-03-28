﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToTupleElementConvertible
{
    TupleElementSyntax ToTupleElement();
}
