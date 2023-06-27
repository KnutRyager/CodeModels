using CodeModels.Models.Primitives.Attribute;

namespace CodeModels.Models;

public interface IToAttributeConvertible : IToAttributeListConvertible
{
    Attribute ToAttribute();
}
