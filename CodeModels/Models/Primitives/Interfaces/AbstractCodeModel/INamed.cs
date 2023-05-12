namespace CodeModels.Models;

public interface INamed :
    ICodeModel,
    IToTypeConvertible,
    IToExpresisonConvertible,
    IToParameterConvertible,
    IToTupleElementConvertible
{
    string Name { get; }
}
