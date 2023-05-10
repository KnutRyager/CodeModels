using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Context;
using CodeModels.Models;
using Common.Reflection;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.ProgramModels;

public record ApiModel(NamedValueCollection Model, IProgramContext? Context = null) : ProgramModel<ClassModel>(Context ?? new ProgramContext())
{
    public override ClassModel Render()
    {
        var dbContext = Type("Microsoft.EntityFrameworkCore.DbContext");
        var firstOrDefault = Method(ReflectionUtil.GetMethodInfo(typeof(Queryable), nameof(Queryable.FirstOrDefault), new[] { typeof(object) }));
        var invocation = null as Block ?? throw new NotImplementedException(); // firstOrDefault.Invoke(Context.GetSingleton(dbContext), Literal(0));    // TODO: ID Lambda
        var getMethod = Method("Get", NamedValues(NamedValue(Type("int"), "id")), Model.Type, invocation);
        var model = InstanceClass($"{Model.ToIdentifier()}Api");
        return model;
    }

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Model.Properties) yield return property;
    }

    public override ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new NotImplementedException();
    }
}

public record CrudMethods(bool Get = false, bool Post = false, bool Put = false, bool Delete = false, bool Search = false);
