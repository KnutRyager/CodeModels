using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace CodeModels.Models.Reflection;

public interface IParameterInfo
{
    bool IsLcid { get; }
    bool IsOptional { get; }
    bool IsOut { get; }
    bool IsRetval { get; }
    string Name { get; }
    int MetadataToken { get; }
    bool IsIn { get; }
    ITypeInfo ParameterType { get; }
    IMemberInfo Member { get; }
    bool HasDefaultValue { get; }
    object RawDefaultValue { get; }
    IEnumerable<CustomAttributeData> CustomAttributes { get; }
    ParameterAttributes Attributes { get; }
    int Position { get; }
    object DefaultValue { get; }
    object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit);
    object[] GetCustomAttributes(bool inherit);
    IList<CustomAttributeData> GetCustomAttributesData();
    ITypeInfo[] GetOptionalCustomModifiers();
    object GetRealObject(StreamingContext context);
    ITypeInfo[] GetRequiredCustomModifiers();
    bool IsDefined(ITypeInfo attributeType, bool inherit);
    string ToString();
}
