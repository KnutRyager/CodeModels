using static CodeModels.Models.Reflection.TypeReflectionUtil;

namespace CodeModels.Models.Reflection;

public record ReflectedCustomAttributeProvider(System.Reflection.ICustomAttributeProvider Info) : ICustomAttributeProvider
{
    public object[] GetCustomAttributes(bool inherit) => Info.GetCustomAttributes(inherit);
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => Info.GetCustomAttributes(Map(attributeType), inherit);
    public bool IsDefined(ITypeInfo attributeType, bool inherit) => Info.IsDefined(Map(attributeType), inherit);
}
