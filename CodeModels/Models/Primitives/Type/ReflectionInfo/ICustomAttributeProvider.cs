namespace CodeModels.Models.Reflection;

public interface ICustomAttributeProvider
{
    object[] GetCustomAttributes(bool inherit);
    object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit);
    bool IsDefined(ITypeInfo attributeType, bool inherit);
}
