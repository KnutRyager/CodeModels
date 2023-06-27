using CodeModels.Models.Primitives.Attribute;

namespace CodeModels.Models;

public interface IToAttributeListConvertible : IToAttributeListListConvertible
{
    AttributeList ToAttributeList();
}
