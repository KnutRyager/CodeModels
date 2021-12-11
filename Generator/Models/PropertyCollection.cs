using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Generic;

public class PropertyCollection
{
    public List<Property> Properties { get; set; }

    public PropertyCollection(IEnumerable<Property> properties)
    {
        Properties = properties.ToList();
    }
}