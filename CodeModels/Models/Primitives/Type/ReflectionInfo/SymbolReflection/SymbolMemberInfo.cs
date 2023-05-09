using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public abstract record SymbolMemberInfo<T>(T Info, IProgramContext Context) : IMemberInfo where T : ISymbol
{
    public IEnumerable<CustomAttributeData> CustomAttributes => throw new NotImplementedException();
    public ITypeInfo DeclaringType => Map(Info.ContainingType, Context);
    public abstract MemberTypes MemberType { get; }
    public int MetadataToken => Info.MetadataToken;
    public Module Module => throw new NotImplementedException();
    public string Name => Info.Name;
    public ITypeInfo ReflectedType => throw new NotImplementedException();
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => throw new NotImplementedException();
    public object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
    public IList<CustomAttributeData> GetCustomAttributesData() => Info.GetAttributes().Select(x => Map(x,Context)).ToList();
    public bool IsDefined(ITypeInfo attributeType, bool inherit) => throw new NotImplementedException();
}
