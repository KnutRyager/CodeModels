using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToParameterConvertible
{
    Parameter ToParameter();
}
