using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
public  class RecordBuilder
{
    //[Key] public int Id  { get; set; }
    //[Key] public int? Id2  { get; set; }

    public RecordBuilder AddProperty(Type type, string name)
    {

        return this;
    }
    public RecordBuilder AddProperty(ITypeSymbol type, string name)
    {

        return this;
    }

    public RecordDeclarationSyntax Build()
    {
        return RecordDeclaration();
    }
}