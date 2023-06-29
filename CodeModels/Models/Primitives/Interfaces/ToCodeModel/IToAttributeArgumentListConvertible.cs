using CodeModels.Models.Primitives.Attribute;

namespace CodeModels.Models;

public interface IToAttributeArgumentListConvertible
{
    AttributeArgumentList ToAttributeArgumentList();
}
