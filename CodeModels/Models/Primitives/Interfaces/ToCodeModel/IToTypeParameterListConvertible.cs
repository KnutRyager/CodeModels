using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToTypeParameterListConvertible
{
    TypeParameterList ToTypeParameterList();
}
