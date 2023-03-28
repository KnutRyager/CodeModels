using System;
using System.Collections.Generic;
using System.Linq;
using Common.Reflection;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.ProgramModels;

public record ApiModel(PropertyCollection Model, IProgramContext? Context = null) : ProgramModel<ClassModel>(Context ?? new ProgramContext())
{
    public override ClassModel Render()
    {
        var dbContext = Type("Microsoft.EntityFrameworkCore.DbContext");
        var firstOrDefault = Method(ReflectionUtil.GetMethodInfo(typeof(Queryable), nameof(Queryable.FirstOrDefault), new[] { typeof(object) }));
        var invocation = null as Block ?? throw new NotImplementedException(); // firstOrDefault.Invoke(Context.GetSingleton(dbContext), Literal(0));    // TODO: ID Lambda
        var getMethod = Method("Get", PropertyCollection(Property(Type("int"), "id")), Model.Type, invocation);
        var model = InstanceClass($"{Model.Identifier}Api");
        return model;
    }

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Model.Properties) yield return property;
    }

    public override ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new System.NotImplementedException();
    }
}

public record CrudMethods(bool Get = false, bool Post = false, bool Put = false, bool Delete = false, bool Search = false);
