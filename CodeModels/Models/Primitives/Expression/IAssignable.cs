﻿using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;

namespace CodeModels.Models;

public interface IAssignable
{
    void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes);
}
