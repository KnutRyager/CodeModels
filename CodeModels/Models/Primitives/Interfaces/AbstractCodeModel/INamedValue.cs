using Common.Interfaces;

namespace CodeModels.Models;

public interface INamedValue :
    IToTypeConvertible,
    IToExpresisonConvertible,
    IToSimpleNameConvertible,
    IToParameterConvertible,
    IToTupleElementConvertible
{
    string Name { get; }
}
