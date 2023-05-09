using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record SymbolParameterInfo(IParameterSymbol Info, IProgramContext Context) : IParameterInfo
{
    public bool IsLcid => throw new NotImplementedException();
    public bool IsOptional => Info.IsOptional;
    public bool IsOut => throw new NotImplementedException();
    public bool IsRetval => throw new NotImplementedException();
    public bool IsIn => throw new NotImplementedException();
    public ITypeInfo ParameterType => Map(Info.Type, Context);
    public IMemberInfo Member => throw new NotImplementedException();
    //=> Map(Info.Member);
    public bool HasDefaultValue => Info.HasExplicitDefaultValue;
    public object RawDefaultValue => Info.ExplicitDefaultValue!;
    public ParameterAttributes Attributes => MapParameterAttributes(Info.GetAttributes(), Context);
    public int Position => Info.Ordinal;
    public object DefaultValue => Info.ExplicitDefaultValue!;
    public string Name => Info.Name;
    public int MetadataToken => Info.MetadataToken;
    public IEnumerable<CustomAttributeData> CustomAttributes => Map(Info.GetAttributes(), Context);
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => throw new NotImplementedException();
    public object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
    public IList<CustomAttributeData> GetCustomAttributesData() => throw new NotImplementedException();
    public ITypeInfo[] GetOptionalCustomModifiers() => MapCustomModifier(Info.CustomModifiers.Where(x => x.IsOptional), Context);
    public object GetRealObject(StreamingContext context) => throw new NotImplementedException();
    public ITypeInfo[] GetRequiredCustomModifiers() => MapCustomModifier(Info.CustomModifiers.Where(x => !x.IsOptional), Context);
    public bool IsDefined(ITypeInfo attributeType, bool inherit) => throw new NotImplementedException();
}
