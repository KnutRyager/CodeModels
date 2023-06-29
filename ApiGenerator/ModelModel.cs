namespace ApiGenerator;

using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models;
using static CodeModels.Factory.AbstractCodeModelFactory;

public record ModelModel(string Name, NamedValueCollection Properties)
    : IToTypeConvertible
{
    public static ModelModel Create(string name, NamedValueCollection? properties = default)
        => new(name, properties ?? NamedValues());

    public IType ToType() => CodeModelFactory.QuickType(Name);
}