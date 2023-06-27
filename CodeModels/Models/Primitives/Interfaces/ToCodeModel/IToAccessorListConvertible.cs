using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToAccessorListConvertible
{
    AccessorList ToAccessorList();
}
