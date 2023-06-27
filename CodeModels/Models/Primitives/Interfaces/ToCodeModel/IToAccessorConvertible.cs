using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToAccessorConvertible : IToAccessorListConvertible
{
    Accessor ToAccessor();
}
