using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToPropertyConvertible
{
    Property ToProperty();
}
