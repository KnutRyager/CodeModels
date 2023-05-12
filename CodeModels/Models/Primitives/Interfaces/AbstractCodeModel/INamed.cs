namespace CodeModels.Models;

public interface INamed :
    ICodeModel,
    IToTypeConvertible,
    IToExpresisonConvertible,
    IToSimpleNameConvertible,
    IToParameterConvertible,
    IToTupleElementConvertible
{
    string Name { get; }
}
