using Common.Interfaces;

namespace CodeAnalyzation.Models;

public interface INamedValue :
    IToTypeConvertible,
    IToExpresisonConvertible,
    IToSimpleNameConvertible,
    IToParameterConvertible,
    IToTupleElementConvertible
{
    string Name { get; }
}
