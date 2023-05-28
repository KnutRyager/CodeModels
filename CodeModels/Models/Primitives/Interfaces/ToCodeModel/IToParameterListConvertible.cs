using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToParameterListConvertible
{
    ParameterList ToParameterList();
}
