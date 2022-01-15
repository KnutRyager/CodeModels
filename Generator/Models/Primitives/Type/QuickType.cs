using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record QuickType(string Identifier, bool Required = true, bool IsMulti = false, TypeSyntax? SourceSyntax = null, Type? Type = null)
        : AbstractType(Identifier, Required, IsMulti, SourceSyntax, Type)
    {
    }
}