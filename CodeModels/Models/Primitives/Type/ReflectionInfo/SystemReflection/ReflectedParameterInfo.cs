using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record ReflectedParameterInfo(ParameterInfo Info) : IParameterInfo
{
    public bool IsLcid => Info.IsLcid;
    public bool IsOptional => Info.IsOptional;
    public bool IsOut => Info.IsOut;
    public bool IsRetval => Info.IsRetval;
    public bool IsIn => Info.IsIn;
    public ITypeInfo ParameterType => Map(Info.ParameterType);
    public IMemberInfo Member => Map(Info.Member);
    public bool HasDefaultValue => Info.HasDefaultValue;
    public object RawDefaultValue => Info.RawDefaultValue;
    public ParameterAttributes Attributes => Info.Attributes;
    public int Position => Info.Position;
    public object DefaultValue => Info.DefaultValue;
    public string Name => Info.Name;
    public int MetadataToken => Info.MetadataToken;
    public IEnumerable<CustomAttributeData> CustomAttributes => Info.CustomAttributes;
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => Info.GetCustomAttributes(Map(attributeType), inherit);
    public object[] GetCustomAttributes(bool inherit) => Info.GetCustomAttributes(inherit);
    public IList<CustomAttributeData> GetCustomAttributesData() => Info.GetCustomAttributesData();
    public ITypeInfo[] GetOptionalCustomModifiers() => Map(Info.GetOptionalCustomModifiers());
    public object GetRealObject(StreamingContext context) => Info.GetRealObject(context);
    public ITypeInfo[] GetRequiredCustomModifiers() => Map(Info.GetRequiredCustomModifiers());
    public bool IsDefined(ITypeInfo attributeType, bool inherit) => Info.IsDefined(Map(attributeType), inherit);
}
