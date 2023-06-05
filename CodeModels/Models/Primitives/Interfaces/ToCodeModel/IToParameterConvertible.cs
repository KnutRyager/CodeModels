using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToParameterConvertible : IToParameterListConvertible
{
    Parameter ToParameter();
}
