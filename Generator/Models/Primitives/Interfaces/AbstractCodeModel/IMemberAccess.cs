﻿namespace CodeAnalyzation.Models;

public interface IMemberAccess
{
    IBaseTypeDeclaration? Owner { get; }
    IExpression? Instance { get; }
}
