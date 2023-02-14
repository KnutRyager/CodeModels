using System.Collections.Generic;
using System.Reflection;
using static CodeAnalyzation.Models.Reflection.TypeReflectionUtil;

namespace CodeAnalyzation.Models.Reflection;

public record ReflectedMemberInfo<T>(T Info) : IMemberInfo where T : MemberInfo
{
    public IEnumerable<CustomAttributeData> CustomAttributes => Info.CustomAttributes;
    public ITypeInfo DeclaringType => Map(Info.DeclaringType);
    public MemberTypes MemberType => Info.MemberType;
    public int MetadataToken => Info.MetadataToken;
    public Module Module => Info.Module;
    public string Name => Info.Name;
    public ITypeInfo ReflectedType => Map(Info.ReflectedType);
    public object[] GetCustomAttributes(bool inherit) => Info.GetCustomAttributes(inherit);
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => Info.GetCustomAttributes(Map(attributeType), inherit);
    public IList<CustomAttributeData> GetCustomAttributesData() => Info.GetCustomAttributesData();
    public bool IsDefined(ITypeInfo attributeType, bool inherit) => Info.IsDefined(Map(attributeType), inherit);

    //public static bool operator ==(ReflectedMemberInfo left, ReflectedMemberInfo right) => left.Equals(right);
    //public static bool operator !=(ReflectedMemberInfo left, ReflectedMemberInfo right) => !left.Equals(right);
}
