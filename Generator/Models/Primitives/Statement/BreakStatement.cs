﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record BreakStatement() : AbstractStatement<BreakStatementSyntax>
    {
        public override BreakStatementSyntax Syntax() => BreakCustom();

        public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
    }
}