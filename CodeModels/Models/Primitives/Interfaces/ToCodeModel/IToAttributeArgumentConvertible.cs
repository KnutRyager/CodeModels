using CodeModels.Models.Primitives.Attribute;

namespace CodeModels.Models;

public interface IToAttributeArgumentConvertible : IToAttributeArgumentListConvertible
{
    AttributeArgument ToAttributeArgument();
}
